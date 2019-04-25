using System.Collections.Generic;
using UnityEngine;

public class Data_Manipulation : MonoBehaviour
{
    /// <summary>
    /// 返回鼠标点击处的物理参量
    /// </summary>
    /// <param name="gdd">数据结构体</param>
    public static string Return_Contour_Click_Values(GeneralDataOld gdd, int _idx)
    {
        //摄像机发出到点击坐标的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        int index0 = -1,
            index1 = -1,
            index2 = -1;
        string _show = null;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log("hit_triangle: " + hitInfo.triangleIndex);
            MeshCollider mc = hitInfo.collider as MeshCollider;
            Mesh ms = mc.sharedMesh;

            Vector3[] vertices = ms.vertices;

            int[] triangles = ms.triangles;
            int tIndex = hitInfo.triangleIndex;
            //取回本地坐标

            Vector3 p0 = vertices[triangles[tIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[tIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[tIndex * 3 + 2]];

            Debug.Log(p0.x + "," + p0.y + "," + p0.z);
            Debug.Log(p1.x + "," + p1.y + "," + p1.z);
            Debug.Log(p2.x + "," + p2.y + "," + p2.z);

            for (int i = 0; i < gdd.NodeNumber; ++i)
            {
                if (gdd.NodeCoord[i][0].ToString("G7").Contains(p0.x.ToString()))   //  判断原始数据是否包含所获取的数据
                {
                    Debug.Log("p0 Contains! Index is " + i);
                    index0 = i;
                }
                if (gdd.NodeCoord[i][0].ToString("G7").Contains(p1.x.ToString()))
                {
                    Debug.Log("p1 Contains! Index is " + i);
                    index1 = i;
                }
                if (gdd.NodeCoord[i][0].ToString("G7").Contains(p2.x.ToString()))
                {
                    Debug.Log("p2 Contains! Index is " + i);
                    index2 = i;
                }
            }

            if (index0 == -1 || index1 == -1 || index2 == -1)
            {
                Debug.Log("Can't search the index!");
                return null;
            }
            else
            {
                float CoutValue = (float)(gdd.nSlnTypeValue[_idx][index0] + gdd.nSlnTypeValue[_idx][index1] + gdd.nSlnTypeValue[_idx][index2]) / 3.0f;   //  取三点平均值作为输出
                string s = gdd.nSlnTypeNames[_idx] + ": " + CoutValue.ToString("G9");
                Debug.Log(s);
                _show = s;
            }
        }
        return _show;
        //for (int j = 0; j < gdd.nSlnTypeNames.Count; ++j)
        //{
        //    Debug.Log(gdd.nSlnTypeNames[j] + ": p0 " + gdd.nSlnTypeValue[j][index0] + ", p1 " + gdd.nSlnTypeValue[j][index1] + ", p2 " + gdd.nSlnTypeValue[j][index2]);
        //}
    }

    public static float[] ExtrmeValue(GeneralDataOld gd)
    {
        float[] Values = new float[6];
        Values[0] = float.MaxValue;  //寻找x最小
        Values[1] = float.MinValue;  //寻找x最大
        Values[2] = float.MaxValue;  //寻找y最小
        Values[3] = float.MinValue;  //寻找y最大
        Values[4] = float.MaxValue;  //寻找z最小
        Values[5] = float.MinValue;  //寻找z最大
        for (int i = 0; i < gd.NodeCoord.Count; ++i)
        {
            if (gd.NodeCoord[i][0] < Values[0])
                Values[0] = (float)gd.NodeCoord[i][0];  //寻找x最小
            if (gd.NodeCoord[i][0] > Values[1])
                Values[1] = (float)gd.NodeCoord[i][0];  //寻找x最大
            if (gd.NodeCoord[i][1] < Values[2])
                Values[2] = (float)gd.NodeCoord[i][1];  //寻找y最小
            if (gd.NodeCoord[i][1] > Values[3])
                Values[3] = (float)gd.NodeCoord[i][1];  //寻找y最大
            if (gd.NodeCoord[i][2] < Values[4])
                Values[4] = (float)gd.NodeCoord[i][2];  //寻找z最小
            if (gd.NodeCoord[i][2] > Values[5])
                Values[5] = (float)gd.NodeCoord[i][2];  //寻找z最大
        }
        return Values;
    }
    /// <summary>
    /// 自动填充GenaralData里缺损的物理量最大最小值
    /// </summary>
    /// <param name="gdd"></param>
    /// <returns></returns>
    public static GeneralDataOld FillMaxMin(GeneralDataOld gdd)
    {
        GeneralDataOld _gd = new GeneralDataOld()
        {
            ModelName = gdd.ModelName,
            nSlnTypeNames = gdd.nSlnTypeNames,
            NodeCoord = gdd.NodeCoord,
            nSlnTypeValue = gdd.nSlnTypeValue
        };

        int SlnLen = _gd.nSlnTypeNames.Count;
        _gd.nSlnMaxValue = new double[SlnLen];
        _gd.nSlnMinValue = new double[SlnLen];

        for (int j = 0; j < SlnLen; ++j)
        {
            _gd.nSlnMaxValue[j] = double.MinValue;
            _gd.nSlnMinValue[j] = double.MaxValue;
        }

        for (int i = 0; i < gdd.NodeCoord.Count; ++i)
        {
            for (int j = 0; j < gdd.nSlnTypeNames.Count; ++j)
            {
                if (gdd.nSlnTypeValue[j][i] > _gd.nSlnMaxValue[j])
                    _gd.nSlnMaxValue[j] = gdd.nSlnTypeValue[j][i];      //判断是否为最大
                if (gdd.nSlnTypeValue[j][i] < _gd.nSlnMinValue[j])
                    _gd.nSlnMinValue[j] = gdd.nSlnTypeValue[j][i];      //判断是否为最小值
            }

            if (gdd.NodeCoord[i][0] > _gd.Coor_Xmax)     // x最大值
                _gd.Coor_Xmax = gdd.NodeCoord[i][0];
            if (gdd.NodeCoord[i][0] < _gd.Coor_Xmin)     // x最小值
                _gd.Coor_Xmin = gdd.NodeCoord[i][0];
            if (gdd.NodeCoord[i][1] > _gd.Coor_Ymax)     // y最大值
                _gd.Coor_Ymax = gdd.NodeCoord[i][1];
            if (gdd.NodeCoord[i][1] < _gd.Coor_Ymin)     // y最小值
                _gd.Coor_Ymin = gdd.NodeCoord[i][1];
            if (gdd.NodeCoord[i][2] > _gd.Coor_Zmax)     // z最大值
                _gd.Coor_Zmax = gdd.NodeCoord[i][2];
            if (gdd.NodeCoord[i][2] < _gd.Coor_Zmin)     // z最小值
                _gd.Coor_Zmin = gdd.NodeCoord[i][2];
        }

        _gd.NodeNumber = _gd.NodeCoord.Count;
        _gd.NodeExteriorNumber = _gd.NodeNumber;
        if (gdd.Elem_NodesIndex != null)
        {
            _gd.Elem_NodesIndex = gdd.Elem_NodesIndex;
            _gd.ElemNumber = _gd.Elem_NodesIndex.Count;
            _gd.ElemExteriorNumber = _gd.ElemNumber;
        }

        return _gd;
    }

    public static GeneralDataOld Select_Horizon_gd(GeneralDataOld gdd, double z)
    {
        GeneralDataOld _gd = new GeneralDataOld
        {
            ModelName = gdd.ModelName,  //设置模型名称
            nSlnTypeNames = gdd.nSlnTypeNames  //设置物理量名称
        };

        int SlnLen = _gd.nSlnTypeNames.Count;
        _gd.nSlnTypeValue = new List<double>[SlnLen];
        _gd.nSlnMaxValue = new double[SlnLen];
        _gd.nSlnMinValue = new double[SlnLen];

        for (int j = 0; j < SlnLen; ++j)
        {
            _gd.nSlnTypeValue[j] = new List<double>();
            _gd.nSlnMaxValue[j] = double.MinValue;
            _gd.nSlnMinValue[j] = double.MaxValue;
        }
        _gd.NodeCoord = new List<double[]>();   //初始化 _gd的物理量数据

        for (int i = 0; i < gdd.NodeCoord.Count; ++i)
        {
            if (System.Math.Abs(gdd.NodeCoord[i][2] - z) < 0.05)    //取与所需数据差值在小范围内的数据
            {
                _gd.NodeCoord.Add(gdd.NodeCoord[i]);
                for (int j = 0; j < gdd.nSlnTypeNames.Count; j++)
                {
                    _gd.nSlnTypeValue[j].Add(gdd.nSlnTypeValue[j][i]);      //添加第j+4列物理量数据
                    if (gdd.nSlnTypeValue[j][i] > _gd.nSlnMaxValue[j])
                        _gd.nSlnMaxValue[j] = gdd.nSlnTypeValue[j][i];      //判断是否为最大
                    if (gdd.nSlnTypeValue[j][i] < _gd.nSlnMinValue[j])
                        _gd.nSlnMinValue[j] = gdd.nSlnTypeValue[j][i];      //判断是否为最小值
                }
            }
        }
        _gd.NodeNumber = _gd.NodeCoord.Count;

        _gd.NodeExteriorNumber = _gd.NodeNumber;

        if (gdd.Elem_NodesIndex != null)
        {
            _gd.Elem_NodesIndex = gdd.Elem_NodesIndex;
            _gd.ElemNumber = _gd.Elem_NodesIndex.Count;
            _gd.ElemExteriorNumber = _gd.ElemNumber;
        }
        else
            Debug.Log("数据中单元索引为Null");

        return _gd;
    }

    public static GeneralDataOld Reduce_gd(GeneralDataOld gdd)        //优化前的精简数据方法，时间复杂度为n^3
    {
        Debug.Log("精简前节点数量: " + gdd.NodeCoord.Count);
        GeneralDataOld re_gd = new GeneralDataOld()
        {
            ModelName = gdd.ModelName,
            nSlnTypeNames = gdd.nSlnTypeNames,
            NodeCoord = new List<double[]>(),
            nSlnTypeValue = new List<double>[gdd.nSlnTypeNames.Count]
        };
        float[] _ExValue = ExtrmeValue(gdd);
        int count;
        float block_size = 0.5f;
        int xn = (int)((_ExValue[1] - _ExValue[0]) / block_size);
        int yn = (int)((_ExValue[3] - _ExValue[2]) / block_size);
        int zn = (int)((_ExValue[5] - _ExValue[4]) / block_size);
        Debug.Log("X_Min: " + _ExValue[0]);
        Debug.Log("X_Max: " + _ExValue[1]);
        Debug.Log("Y_Min: " + _ExValue[2]);
        Debug.Log("Y_Max: " + _ExValue[3]);
        Debug.Log("Z_Min: " + _ExValue[4]);
        Debug.Log("Z_Max: " + _ExValue[5]);
        List<double> vl = new List<double>();
        for (int n = 0; n < re_gd.nSlnTypeNames.Count; ++n)       //初始化
        {
            vl.Add(0);
            re_gd.nSlnTypeValue[n] = new List<double>();
        }
        for (int i = 0; i < xn; ++i)
        {
            for (int j = 0; j < yn; ++j)
            {
                for (int k = 0; k < zn; ++k)
                {
                    count = 0;
                    double[] cd = new double[3] { 0, 0, 0 };
                    for (int n = 0; n < re_gd.nSlnTypeNames.Count; ++n)       //初始化
                        vl[n] = 0;
                    for (int m = 0; m < gdd.NodeNumber; ++m)
                    {
                        bool condx = _ExValue[0] + i * block_size <= gdd.NodeCoord[m][0] && gdd.NodeCoord[m][0] < _ExValue[0] + (i + 1) * block_size;
                        if (condx)
                        {
                            bool condy = _ExValue[2] + j * block_size <= gdd.NodeCoord[m][1] && gdd.NodeCoord[m][1] < _ExValue[2] + (j + 1) * block_size;
                            if (condy)
                            {
                                bool condz = _ExValue[4] + k * block_size <= gdd.NodeCoord[m][2] && gdd.NodeCoord[m][2] < _ExValue[4] + (k + 1) * block_size;
                                if (condz)
                                {
                                    ++count;
                                    cd[0] += gdd.NodeCoord[m][0];
                                    cd[1] += gdd.NodeCoord[m][1];
                                    cd[2] += gdd.NodeCoord[m][2];
                                    for (int v = 0; v < gdd.nSlnTypeNames.Count; ++v)
                                        vl[v] += gdd.nSlnTypeValue[v][m];
                                }
                            }
                        }
                    }
                    if (count > 0)
                    {
                        cd[0] /= count;
                        cd[1] /= count;
                        cd[2] /= count;
                        re_gd.NodeCoord.Add(cd);
                        for (int v = 0; v < re_gd.nSlnTypeNames.Count; ++v)
                        {
                            vl[v] /= count;
                            re_gd.nSlnTypeValue[v].Add(vl[v]);
                        }

                    }


                }
            }
        }
        re_gd = FillMaxMin(re_gd);      //填充物理量最大最小值
        if (re_gd.NodeCoord.Count > 0)
            Debug.Log("精简后节点数量: " + re_gd.NodeCoord.Count);
        else
            Debug.Log("Error: Wrong reduction algorithm");

        return re_gd;

    }

    /// <summary>
    /// 优化之后的精简数据方法，遍历数据节点，按节点坐标xyz分类节点属于哪个方格，时间复杂度为n
    /// </summary>
    /// <param name="gdd">数据结构体</param>
    /// <param name="s">精简方法(ratio 按比例)</param>
    /// <param name="ratio">精简率</param>
    /// <returns></returns>
    public static GeneralDataOld Reduce_gd(GeneralDataOld gdd, string s, float ratio)
    {
        if (s.Trim().ToLower() == "ratio")
        {
            Debug.Log("精简前节点数量: " + gdd.NodeCoord.Count);
            GeneralDataOld re_gd = new GeneralDataOld()
            {
                ModelName = gdd.ModelName,
                nSlnTypeNames = gdd.nSlnTypeNames,
                NodeCoord = new List<double[]>(),
                nSlnTypeValue = new List<double>[gdd.nSlnTypeNames.Count]
            };
            int nu = (int)(gdd.NodeCoord.Count * ratio);        //设定精简后点数量
            Debug.Log("nu " + nu);
            int nu_x = (int)Mathf.Ceil((float)(gdd.Coor_Xmax - gdd.Coor_Xmin) * Mathf.Pow((float)(nu / ((gdd.Coor_Xmax - gdd.Coor_Xmin) * (gdd.Coor_Ymax - gdd.Coor_Ymin) * (gdd.Coor_Zmax - gdd.Coor_Zmin))), 1.0f / 3)),
                nu_y = (int)Mathf.Ceil((float)(gdd.Coor_Ymax - gdd.Coor_Ymin) * Mathf.Pow((float)(nu / ((gdd.Coor_Xmax - gdd.Coor_Xmin) * (gdd.Coor_Ymax - gdd.Coor_Ymin) * (gdd.Coor_Zmax - gdd.Coor_Zmin))), 1.0f / 3)),
                nu_z = (int)Mathf.Ceil((float)(gdd.Coor_Zmax - gdd.Coor_Zmin) * Mathf.Pow((float)(nu / ((gdd.Coor_Xmax - gdd.Coor_Xmin) * (gdd.Coor_Ymax - gdd.Coor_Ymin) * (gdd.Coor_Zmax - gdd.Coor_Zmin))), 1.0f / 3));
            float block_x = (float)(gdd.Coor_Xmax - gdd.Coor_Xmin) / nu_x,
                   block_y = (float)(gdd.Coor_Ymax - gdd.Coor_Ymin) / nu_y,
                   block_z = (float)(gdd.Coor_Zmax - gdd.Coor_Zmin) / nu_z;
            Debug.Log("coorx " + (gdd.Coor_Xmax - gdd.Coor_Xmin)); Debug.Log("coory " + (gdd.Coor_Ymax - gdd.Coor_Ymin)); Debug.Log("coorz " + (gdd.Coor_Zmax - gdd.Coor_Zmin));
            Debug.Log("nu_x " + nu_x); Debug.Log("nu_y " + nu_y); Debug.Log("nu_z " + nu_z);

            //string[,,] num = new string[nu_x, nu_z, nu_y];
            int[,,] num = new int[nu_x, nu_y, nu_z];
            //for (int ux = 0; ux < nu_x; ++ux)
            //    for (int uy = 0; uy < nu_y; ++uy)
            //        for (int uz = 0; uz < nu_z; ++uz)
            //            num[ux, uy, uz] = -1;
            Debug.Log("数组初始化" + num[0, 0, 0]);

            //int count;

            List<double> vl = new List<double>();
            for (int n = 0; n < re_gd.nSlnTypeNames.Count; ++n)       //初始化
            {
                vl.Add(0);
                re_gd.nSlnTypeValue[n] = new List<double>();
            }

            int bx = 0,
                by = 0,
                bz = 0;

            for (int i = 0; i < gdd.NodeCoord.Count; ++i)
            {
                bx = (int)Mathf.Floor((float)(gdd.NodeCoord[i][0] - gdd.Coor_Xmin) / block_x);
                if (bx >= nu_x)
                    bx = nu_x - 1;
                by = (int)Mathf.Floor((float)(gdd.NodeCoord[i][1] - gdd.Coor_Ymin) / block_y);
                if (by >= nu_y)
                    by = nu_y - 1;
                bz = (int)Mathf.Floor((float)(gdd.NodeCoord[i][2] - gdd.Coor_Zmin) / block_z);
                if (bz >= nu_z)
                    bz = nu_z - 1;
                if (num[bx, by, bz] < 3)
                {
                    num[bx, by, bz]++;
                    re_gd.NodeCoord.Add(gdd.NodeCoord[i]);
                    for (int v = 0; v < gdd.nSlnTypeNames.Count; ++v)
                        re_gd.nSlnTypeValue[v].Add(gdd.nSlnTypeValue[v][i]);

                }
            }

            //for (int i = 0; i < xn; ++i)
            //{
            //    for (int j = 0; j < yn; ++j)
            //    {
            //        for (int k = 0; k < zn; ++k)
            //        {
            //            count = 0;
            //            double[] cd = new double[3] { 0, 0, 0 };
            //            for (int n = 0; n < re_gd.nSlnTypeNames.Count; ++n)       //初始化
            //                vl[n] = 0;
            //            for (int m = 0; m < gdd.NodeNumber; ++m)
            //            {
            //                bool condx = _ExValue[0] + i * block_size <= gdd.NodeCoord[m][0] && gdd.NodeCoord[m][0] < _ExValue[0] + (i + 1) * block_size;
            //                if (condx)
            //                {
            //                    bool condy = _ExValue[2] + j * block_size <= gdd.NodeCoord[m][1] && gdd.NodeCoord[m][1] < _ExValue[2] + (j + 1) * block_size;
            //                    if (condy)
            //                    {
            //                        bool condz = _ExValue[4] + k * block_size <= gdd.NodeCoord[m][2] && gdd.NodeCoord[m][2] < _ExValue[4] + (k + 1) * block_size;
            //                        if (condz)
            //                        {
            //                            ++count;
            //                            cd[0] += gdd.NodeCoord[m][0];
            //                            cd[1] += gdd.NodeCoord[m][1];
            //                            cd[2] += gdd.NodeCoord[m][2];
            //                            for (int v = 0; v < gdd.nSlnTypeNames.Count; ++v)
            //                                vl[v] += gdd.nSlnTypeValue[v][m];
            //                        }
            //                    }
            //                }
            //            }
            //            if (count > 0)
            //            {
            //                cd[0] /= count;
            //                cd[1] /= count;
            //                cd[2] /= count;
            //                re_gd.NodeCoord.Add(cd);
            //                for (int v = 0; v < re_gd.nSlnTypeNames.Count; ++v)
            //                {
            //                    vl[v] /= count;
            //                    re_gd.nSlnTypeValue[v].Add(vl[v]);
            //                }

            //            }
            //        }
            //    }
            //}
            re_gd = FillMaxMin(re_gd);      //填充物理量最大最小值
            if (re_gd.NodeCoord.Count > 0)
                Debug.Log("精简后节点数量: " + re_gd.NodeCoord.Count);
            else
                Debug.Log("Error: Wrong reduction algorithm");
            num = null;

            return re_gd;
        }
        else
        {
            Debug.Log("未正确选择方法组");
            return null;
        }
    }
    public static Dynamic_GeneralData Trans_gd_To_dgd(GeneralDataOld _gd)
    {
        Dynamic_GeneralData _dgd = new Dynamic_GeneralData()
        {
            ModelName = _gd.ModelName,

            //节点信息
            NodeNumber = _gd.NodeNumber,
            NodeInteriorNumber = _gd.NodeInteriorNumber,
            NodeExteriorNumber = _gd.NodeExteriorNumber,

            NodeCoord = _gd.NodeCoord,
            Coor_Xmax = _gd.Coor_Xmax,
            Coor_Xmin = _gd.Coor_Xmin,
            Coor_Ymax = _gd.Coor_Ymax,
            Coor_Ymin = _gd.Coor_Ymin,
            Coor_Zmax = _gd.Coor_Zmax,
            Coor_Zmin = _gd.Coor_Zmin,

            //单元信息
            ElemNumber = _gd.ElemNumber,
            ElemInteriorNumber = _gd.ElemInteriorNumber,
            ElemExteriorNumber = _gd.ElemExteriorNumber,
            Elem_NodesIndex = _gd.Elem_NodesIndex,     //单元索引序号

            //节点解
            nSlnTypeNames = _gd.nSlnTypeNames  //物理量名称
        };

        return _dgd;
    }
    public static GeneralDataOld Linear_Interpolate_gd(Dynamic_GeneralData dgd, float t)
    {
        int index = 0;
        float ratio = 0f;


        GeneralDataOld _gd = new GeneralDataOld()
        {
            ModelName = dgd.ModelName,

            //节点信息
            NodeNumber = dgd.NodeNumber,
            NodeInteriorNumber = dgd.NodeInteriorNumber,
            NodeExteriorNumber = dgd.NodeExteriorNumber,

            NodeCoord = dgd.NodeCoord,
            Coor_Xmax = dgd.Coor_Xmax,
            Coor_Xmin = dgd.Coor_Xmin,
            Coor_Ymax = dgd.Coor_Ymax,
            Coor_Ymin = dgd.Coor_Ymin,
            Coor_Zmax = dgd.Coor_Zmax,
            Coor_Zmin = dgd.Coor_Zmin,

            //单元信息
            ElemNumber = dgd.ElemNumber,
            ElemInteriorNumber = dgd.ElemInteriorNumber,
            ElemExteriorNumber = dgd.ElemExteriorNumber,
            Elem_NodesIndex = dgd.Elem_NodesIndex,     //单元索引序号

            //节点解
            nSlnTypeNames = dgd.nSlnTypeNames,  //物理量名称
            nSlnTypeValue = new List<double>[dgd.nSlnTypeValues[0].Length],
            nSlnMaxValue = new double[dgd.nSlnTypeValues[0].Length],
            nSlnMinValue = new double[dgd.nSlnTypeValues[0].Length]
        };


        if (t < dgd.TimeSteps[0] || t > dgd.TimeSteps[dgd.TimeSteps.Count - 1])
        {
            Debug.Log("Check input T");
            return null;
        }
        //找到t对用index 索引
        for (int i = 0; i < dgd.TimeSteps.Count; ++i)
        {
            if (t == dgd.TimeSteps[i])
            {
                index = i;
                break;
            }
            else if (t > dgd.TimeSteps[i] && t < dgd.TimeSteps[i + 1])
            {
                index = i;
                break;
            }
        }

        //防止 index = count-1 使 index + 1 超出索引
        if (index == dgd.TimeSteps.Count - 1)
        {
            index -= 1;
            ratio = 0f;
        }
        else
        {
            ratio = (dgd.TimeSteps[index + 1] - t) / (dgd.TimeSteps[index + 1] - dgd.TimeSteps[index]);
        }

        //插值 dgd 信息
        for (int i = 0; i < dgd.nSlnTypeValues[index].Length; ++i)
        {
            _gd.nSlnTypeValue[i] = new List<double>();
            _gd.nSlnMaxValue[i] = double.MinValue;
            _gd.nSlnMinValue[i] = double.MaxValue;

            for (int j = 0; j < dgd.nSlnTypeValues[index][i].Count; ++j)
            {
                _gd.nSlnTypeValue[i].Add(ratio * dgd.nSlnTypeValues[index][i][j] + (1 - ratio) * dgd.nSlnTypeValues[index + 1][i][j]);

                //判断是否为最值
                _gd.nSlnMaxValue[i] = _gd.nSlnMaxValue[i] < _gd.nSlnTypeValue[i][j] ? _gd.nSlnTypeValue[i][j] : _gd.nSlnMaxValue[i];
                _gd.nSlnMinValue[i] = _gd.nSlnMinValue[i] > _gd.nSlnTypeValue[i][j] ? _gd.nSlnTypeValue[i][j] : _gd.nSlnMinValue[i];
            }
        }

        return _gd;

    }
}
