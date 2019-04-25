using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using System.Threading;

using NETCDF_CDL;//调用netcdf

public class NetCDF_Handler {

    [NonSerialized]
    public Dynamic_GeneralData newGddContents = new Dynamic_GeneralData();
    public static bool bActionGdd = false;
    public bool inReadingState = false;
	//confirm:
	//height is as 0.1m as from-to-top
	//from-to-top is 34, as 3.4m in VR

    //构建单例模式
    //不考虑多进程
    private static NetCDF_Handler uniqueInstance;

    private NetCDF_Handler() { 
        //防止默认实例
    }

    public static NetCDF_Handler GetInstance(){
        if(uniqueInstance == null){
            uniqueInstance = new NetCDF_Handler();
        }
        return uniqueInstance;
    }


    /// <summary>
    /// 
    /// 每一个文件只有一帧（时间刻）数据
    /// 每次只能读取一个文件
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="model"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public IEnumerator NetCDFOperation(string path,int model, string[] names) 
	{
		int ncidp;
		int openflag = _netcdf.nc_open(path, _netcdf.CreateMode.NC_NOWRITE, out ncidp);
        //Debug.Log("openflag : " + openflag + " -> ncidp : " + ncidp);

		int ndims;//dimension
		int nvars;//变量长度
		int ngatts;
		int unlimdimid;
		int inqflag = _netcdf.nc_inq(ncidp, out ndims, out nvars, out ngatts, out unlimdimid);

		Debug.Log("result: " + inqflag + " -> ndims : " + ndims + " // nvars : " + nvars + " // ngatts : " + ngatts + " // unlimdimid : " + unlimdimid);

		//dimension
        //StringBuilder[] dimNames = new StringBuilder[ndims];
        //for (int i = 0; i < ndims; i++)
        //{
        //    //dimNames[i] = dimname;
        //    //Debug.Log("dimData : " + dimNames[i] + "  " + dimData[i]);
        //}
		//variable
        //StringBuilder[] varNames = new StringBuilder[nvars];
        //_netcdf.NcType[] varTypes = new _netcdf.NcType[nvars];
        //int varid = 0;
        //StringBuilder varname = new StringBuilder();
        //for (int i = 0; i < nvars; i++)
        //{
        //    _netcdf.nc_inq_varname(ncidp, i, varname);									//Name
        //    _netcdf.nc_inq_vartype(ncidp, i, out varTypes[i]);							//type
        //    _netcdf.nc_inq_varid(ncidp, varname.ToString(), out varid);					//ID
        //    varNames[i] = varname;
        //    Debug.Log("varname 指针 : " + varid + "  " + varNames[i] + "  " + varTypes[i]);
        //}
        int[] dimData = new int[ndims];
        StringBuilder dimname = new StringBuilder();
        for (int i = 0; i < ndims; i++)
        {
            _netcdf.nc_inq_dim(ncidp, i, dimname, out dimData[i]);
        }
		// data length maust be 420*450*34*3, otherwise read fails(*************IMPORTANT)
		int timeCount = dimData [0];
		int fromtopCount = dimData [4];
		int latlogCount = dimData [2] * dimData [3];//每一层的定点数
        int allDataCount = dimData[2] * dimData[3] * dimData[4] * timeCount;
        		
        Debug.Log("数据长度为： " + dimData[0] + " , " + dimData[1] + " , " + dimData[2] + " , " + dimData[3] + " , " + dimData[4]);

        if (model != 1) {
            if (newGddContents.NodeCoord.Count > 0) {
                newGddContents.NodeCoord.Clear();
            }
            newGddContents.NodeCoord = ReadSlnPoints(ncidp, allDataCount, latlogCount);
            newGddContents.NodeNumber = newGddContents.NodeCoord.Count;
            yield return new WaitForEndOfFrame();
        }

        float[] curdata_cloud = null;
        float[] curdata_rain = null;
        float[] curdata_ice = null;
        float[] curdata_snow = null;
        float[] curdata_graup = null;

        if (model != 0) {
            float maxVal = 0;
            float minVal = 0;
            for (int i = 0; i < names.Length; i++) {
                List<float[]> temp_val = new List<float[]>();
                List<float> temp_max = new List<float>();
                List<float> temp_min = new List<float>();
                if (names[i] == "CLOUD") {
                    //cloud data
                    curdata_cloud = new float[allDataCount];
                    getData(ncidp, 3, ref curdata_cloud);
                    temp_val.Add(curdata_cloud);
                    FindMaxMinValue(curdata_cloud, ref maxVal, ref minVal);
                    temp_max.Add(maxVal);
                    temp_min.Add(minVal);
                    if (newGddContents.nSlnTypeNames.Contains("CLOUD"))
                    {
                        int _idx = newGddContents.nSlnTypeNames.IndexOf("CLOUD", 0, newGddContents.nSlnTypeNames.Count);
                        newGddContents.nSlnTypeValues.RemoveAt(_idx);
                        newGddContents.nSlnTypeValues.Insert(_idx, temp_val);
                        newGddContents.nSlnMaxValues.RemoveAt(_idx);
                        newGddContents.nSlnMaxValues.Insert(_idx, temp_max);
                        newGddContents.nSlnMinValues.RemoveAt(_idx);
                        newGddContents.nSlnMinValues.Insert(_idx, temp_min);

                    }
                    else {
                        newGddContents.nSlnTypeNames.Add("CLOUD");
                        newGddContents.nSlnTypeValues.Add(temp_val);
                        newGddContents.nSlnMaxValues.Add(temp_max);
                        newGddContents.nSlnMinValues.Add(temp_min);
                    }
                    
                    yield return new WaitForEndOfFrame();
                }
                if (names[i] == "RAIN")
                {
                    //rain data
                    curdata_rain = new float[allDataCount];
                    getData(ncidp, 4, ref curdata_rain);
                    temp_val.Add(curdata_rain);
                    FindMaxMinValue(curdata_rain, ref maxVal, ref minVal);
                    temp_max.Add(maxVal);
                    temp_min.Add(minVal);
                    if (newGddContents.nSlnTypeNames.Contains("RAIN"))
                    {
                        int _idx = newGddContents.nSlnTypeNames.IndexOf("RAIN", 0, newGddContents.nSlnTypeNames.Count);
                        newGddContents.nSlnTypeValues.RemoveAt(_idx);
                        newGddContents.nSlnTypeValues.Insert(_idx, temp_val);
                        newGddContents.nSlnMaxValues.RemoveAt(_idx);
                        newGddContents.nSlnMaxValues.Insert(_idx, temp_max);
                        newGddContents.nSlnMinValues.RemoveAt(_idx);
                        newGddContents.nSlnMinValues.Insert(_idx, temp_min);

                    }
                    else
                    {
                        newGddContents.nSlnTypeNames.Add("RAIN");
                        newGddContents.nSlnTypeValues.Add(temp_val);
                        newGddContents.nSlnMaxValues.Add(temp_max);
                        newGddContents.nSlnMinValues.Add(temp_min);
                    }
                    yield return new WaitForEndOfFrame();
                }
                if (names[i] == "ICE") {
                    //ice data
                    curdata_ice = new float[allDataCount];
                    getData(ncidp, 5, ref curdata_ice);
                    temp_val.Add(curdata_ice);
                    FindMaxMinValue(curdata_ice, ref maxVal, ref minVal);
                    temp_max.Add(maxVal);
                    temp_min.Add(minVal);
                    if (newGddContents.nSlnTypeNames.Contains("ICE"))
                    {
                        int _idx = newGddContents.nSlnTypeNames.IndexOf("ICE", 0, newGddContents.nSlnTypeNames.Count);
                        newGddContents.nSlnTypeValues.RemoveAt(_idx);
                        newGddContents.nSlnTypeValues.Insert(_idx, temp_val);
                        newGddContents.nSlnMaxValues.RemoveAt(_idx);
                        newGddContents.nSlnMaxValues.Insert(_idx, temp_max);
                        newGddContents.nSlnMinValues.RemoveAt(_idx);
                        newGddContents.nSlnMinValues.Insert(_idx, temp_min);

                    }
                    else
                    {
                        newGddContents.nSlnTypeNames.Add("ICE");
                        newGddContents.nSlnTypeValues.Add(temp_val);
                        newGddContents.nSlnMaxValues.Add(temp_max);
                        newGddContents.nSlnMinValues.Add(temp_min);
                    }
                    yield return new WaitForEndOfFrame();
                }
                if (names[i] == "SNOW")
                {
                    //snow data
                    curdata_snow = new float[allDataCount];
                    getData(ncidp, 6, ref curdata_snow);
                    temp_val.Add(curdata_snow);
                    FindMaxMinValue(curdata_snow, ref maxVal, ref minVal);
                    temp_max.Add(maxVal);
                    temp_min.Add(minVal);
                    if (newGddContents.nSlnTypeNames.Contains("SNOW"))
                    {
                        int _idx = newGddContents.nSlnTypeNames.IndexOf("SNOW", 0, newGddContents.nSlnTypeNames.Count);
                        newGddContents.nSlnTypeValues.RemoveAt(_idx);
                        newGddContents.nSlnTypeValues.Insert(_idx, temp_val);
                        newGddContents.nSlnMaxValues.RemoveAt(_idx);
                        newGddContents.nSlnMaxValues.Insert(_idx, temp_max);
                        newGddContents.nSlnMinValues.RemoveAt(_idx);
                        newGddContents.nSlnMinValues.Insert(_idx, temp_min);

                    }
                    else
                    {
                        newGddContents.nSlnTypeNames.Add("SNOW");
                        newGddContents.nSlnTypeValues.Add(temp_val);
                        newGddContents.nSlnMaxValues.Add(temp_max);
                        newGddContents.nSlnMinValues.Add(temp_min);
                    }
                    yield return new WaitForEndOfFrame();
                }
                if (names[i] == "GRAUP")
                {
                    //graup data
                    curdata_graup = new float[allDataCount];
                    getData(ncidp, 7, ref curdata_graup);
                    temp_val.Add(curdata_graup);
                    FindMaxMinValue(curdata_graup, ref maxVal, ref minVal);
                    temp_max.Add(maxVal);
                    temp_min.Add(minVal);
                    if (newGddContents.nSlnTypeNames.Contains("GRAUP"))
                    {
                        int _idx = newGddContents.nSlnTypeNames.IndexOf("GRAUP", 0, newGddContents.nSlnTypeNames.Count);
                        newGddContents.nSlnTypeValues.RemoveAt(_idx);
                        newGddContents.nSlnTypeValues.Insert(_idx, temp_val);
                        newGddContents.nSlnMaxValues.RemoveAt(_idx);
                        newGddContents.nSlnMaxValues.Insert(_idx, temp_max);
                        newGddContents.nSlnMinValues.RemoveAt(_idx);
                        newGddContents.nSlnMinValues.Insert(_idx, temp_min);

                    }
                    else
                    {
                        newGddContents.nSlnTypeNames.Add("GRAUP");
                        newGddContents.nSlnTypeValues.Add(temp_val);
                        newGddContents.nSlnMaxValues.Add(temp_max);
                        newGddContents.nSlnMinValues.Add(temp_min);
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

		int closeflag = _netcdf.nc_close(ncidp);//close file
		if (closeflag == (int)_netcdf.ResultCode.NC_NOERR) {
			Debug.Log("file indexer release sucessfully! ");
		}
		yield return new WaitForEndOfFrame ();

        curdata_cloud = null;
        curdata_rain = null;
        curdata_ice = null;
        curdata_snow = null;
        curdata_graup = null;

        inReadingState = true;
        bActionGdd = true;//用于传递数据到响应
        
	}

    /// <summary>
    /// 顶点结构
    /// </summary>
    /// <param name="ncidp"></param>
    /// <param name="allDataCount"></param>
    /// <param name="latlogCount"></param>
    /// <returns></returns>
    List<float[]> ReadSlnPoints(int ncidp, int allDataCount, int latlogCount)
    { 
        List<float[]> vertexs = new List<float[]>();
        //lat data
        float[] latdata = new float[allDataCount];
        int getlat = _netcdf.nc_get_var_float(ncidp, 1, latdata);

        if (getlat != 0)
            Debug.Log("latdata: read with error: " + getlat);
        //log data
        float[] logdata = new float[allDataCount];
        int getlog = _netcdf.nc_get_var_float(ncidp, 2, logdata);
        if (getlog != 0)
            Debug.Log("logdata: read with error: " + getlog);
        //construct virtual points
        int topCount = 0;
        for (int i = 0; i < allDataCount; i++)
        {
            topCount = i / latlogCount;
            float[] point = new float[3];
            point[0] = latdata[i];
            point[1] = logdata[i];
            point[2] = topCount * 0.1f; //height can be define
            vertexs.Add(point);
        }
        latdata = null;
        logdata = null;
        return vertexs;
    }

    /// <summary>
    /// 读取物理量数据
    /// </summary>
    /// <param name="ncidp"></param>
    /// <param name="index"></param>
    /// <param name="data"></param>
    void getData(int ncidp, int index, ref float[] data)
    {
        int getcloud = _netcdf.nc_get_var_float(ncidp, index, data);
        if (getcloud != 0)
            Debug.Log("data: read with error: " + getcloud);
    }

    /// <summary>
    /// 获取最大值和最小值
    /// </summary>
    /// <param name="data"></param>
    /// <param name="max"></param>
    /// <param name="min"></param>
    void FindMaxMinValue(float[] data, ref float max, ref float min) {
        max = float.MinValue;
        min = float.MinValue;
        for (int i = 0; i < data.Length; i++) {
            max = data[i] > max ? data[i] : max;
            min = data[i] < min ? data[i] : min;
        }
    }

}
