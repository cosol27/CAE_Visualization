using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Blueprint.framework.api;
using Blueprint.framework.impl;
using Blueprint.Data;

namespace Blueprint.extension.Action 
{
    public class Blue_Action_netcdf : BlueActionBehaviour
    {

        private BlueString dataPath = "";
        private BlueInt netModel = 2;
        private string[] netSlnNames;

        private bool _switch = false;

        NetCDF_Handler ncdfh;

        public override void Init(StateVariable state_variable)
        {
            _switch = (bool)StateVar.GetValue();

            if (_switch) {
                //单例实例化
                ncdfh = NetCDF_Handler.GetInstance();
                FunctionBoay();
            }

            

        }

        public override void Active()
        {
            _switch = (bool)StateVar.GetValue();

            if (_switch)
            {
                //单例实例化
                ncdfh = NetCDF_Handler.GetInstance();
                FunctionBoay();
            }
        }

        public override void OnUpdate()
        {
            if (_switch)
            {
                //if (NetCDF_Handler.bActionGdd )
                //{
                //    //dgddContents = ncdfh.newGddContents;

                //    Debug.Log("fully done");
                //    Debug.Log("结束时间：" + System.DateTime.Now.Minute);

                //    NetCDF_Handler.bActionGdd = false;
                //} 
            }
           
        }

        public override bool ParaParser(string[] para_array)
        {
            if (para_array.Length != 3)
            {
                Debug.LogError("NETCDFHANDLER|气象数据读取 响应需要3个传入参数，当前参数个数为" + para_array.Length + "个，请检查！");
                return false;
            }
            else
            {
                # region 第1个参数转化
                if (para_array[0].StartsWith("*"))
                {
                    if (BlueVarManager.IsCreated(para_array[0].Substring(1)))
                    {
                        try
                        {
                            dataPath = (BlueString)BlueVarManager.GetBlueVar(para_array[0].Substring(1));
                        }
                        catch
                        {
                            Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[0] + "，该BlueVar变量不是BlueString类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[0] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                else
                {
                    dataPath.Value = para_array[0];
                }
                #endregion

                # region 第2个参数转化
                if (para_array[1].StartsWith("*"))
                {
                    if (BlueVarManager.IsCreated(para_array[1].Substring(1)))
                    {
                        try
                        {
                            netModel = (BlueInt)BlueVarManager.GetBlueVar(para_array[1].Substring(1));
                        }
                        catch
                        {
                            Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[1] + "，该BlueVar变量不是BlueInt类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[1] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                else
                {
                    netModel.Value = System.Convert.ToInt16( para_array[1] );
                }
                #endregion

                # region 第3个参数转化
                if (para_array[2].StartsWith("*"))
                {
                    if (BlueVarManager.IsCreated(para_array[2].Substring(1)))
                    {
                        try
                        {
                            BlueString bs = (BlueString)BlueVarManager.GetBlueVar(para_array[2].Substring(1));
                            string str = bs.ToString();
                            string[] temp = str.Split(new char[] { ',' });
                            if (temp.Length != 0)
                            {
                                netSlnNames = new string[temp.Length];
                                for (int i = 0; i < temp.Length; i++) {
                                    netSlnNames[i] = temp[i];
                                }
                            }
                            else {
                                Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[2] + "，该BlueVar变量不是BlueString[]类型！");
                            }
                        }
                        catch
                        {
                            Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[2] + "，该BlueVar变量不是BlueString[]类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFHANDLER|气象数据读取 响应传入参数格式错误： " + para_array[2] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                #endregion

                return true;
            }
        }

        public void FunctionBoay() 
        {

            

            if (string.IsNullOrEmpty(dataPath.Value))
            {
                Debug.Log("path is null or empty!");
                return;
            }

            dataPath.Value = Application.streamingAssetsPath + @"/" + dataPath.Value;

            Debug.Log("DataPath -> " + dataPath.Value);
            

            for (int i = 1; i <= 1; i++)
            {
                ncdfh.inReadingState = true;
                //txtOperation.txtReader(dataPath.Value + @"_" + "001" + @".txt");
                Debug.Log("开始时间：" + System.DateTime.Now.Minute);
                //读取数据
                StartCoroutine(ncdfh.NetCDFOperation(dataPath.Value, netModel.Value, netSlnNames));

            }
        }
       
    }
}

