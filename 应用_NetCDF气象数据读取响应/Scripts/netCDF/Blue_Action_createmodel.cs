using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Blueprint.framework.api;
using Blueprint.framework.impl;
using Blueprint.Data;

namespace Blueprint.extension.Action 
{
    public class Blue_Action_createmodel : BlueActionBehaviour
    {

        private BlueMaterial mat;
        private BlueGameObject father;
        private BlueString slnName;
        private BlueBool bReCreate;

        private bool _switch = false;

        NetCDF_CreatModel ncdcm = new NetCDF_CreatModel();
        NetCDF_Handler ncdfh;

        public override void Init(StateVariable state_variable)
        {
            _switch = (bool)StateVar.GetValue();

            if (_switch) {
                

            }

            

        }

        public override void Active()
        {
            _switch = (bool)StateVar.GetValue();

            if (_switch)
            {


            }
        }

        public override void OnUpdate()
        {
            if (_switch)
            {

            }
           
        }

        public override bool ParaParser(string[] para_array)
        {
            if (para_array.Length != 3)
            {
                Debug.LogError("NETCDFCREATER|气象数据显示 响应需要4个传入参数，当前参数个数为" + para_array.Length + "个，请检查！");
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
                            mat = (BlueMaterial)BlueVarManager.GetBlueVar(para_array[0].Substring(1));
                        }
                        catch
                        {
                            Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[0] + "，该BlueVar变量不是BlueMaterial类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[0] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                #endregion

                # region 第2个参数转化
                if (para_array[1].StartsWith("*"))
                {
                    if (BlueVarManager.IsCreated(para_array[1].Substring(1)))
                    {
                        try
                        {
                            father = (BlueGameObject)BlueVarManager.GetBlueVar(para_array[1].Substring(1));
                        }
                        catch
                        {
                            Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[1] + "，该BlueVar变量不是BlueGameObject类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[1] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                #endregion

                # region 第3个参数转化
                if (para_array[2].StartsWith("*"))
                {
                    if (BlueVarManager.IsCreated(para_array[2].Substring(1)))
                    {
                        try
                        {
                            slnName = (BlueString)BlueVarManager.GetBlueVar(para_array[2].Substring(1));
                        }
                        catch
                        {
                            Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[2] + "，该BlueVar变量不是BlueString类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[2] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                else {
                    slnName.Value = para_array[2];
                }
                #endregion

                # region 第4个参数转化
                if (para_array[3].StartsWith("*"))
                {
                    if (BlueVarManager.IsCreated(para_array[3].Substring(1)))
                    {
                        try
                        {
                            bReCreate = (BlueBool)BlueVarManager.GetBlueVar(para_array[3].Substring(1));
                        }
                        catch
                        {
                            Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[3] + "，该BlueVar变量不是BlueBool类型！");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogError("NETCDFCREATER|气象数据显示 响应传入参数格式错误： " + para_array[3] + "，系统中没有该BlueVar变量！");
                        return false;
                    }
                }
                #endregion
                return true;
            }
        }

        public void FunctionBoay() 
        {
            ncdcm.floorMat = mat.Value;
            ncdcm.father = father.Value;
            ncdfh = NetCDF_Handler.GetInstance();

            int idx = 0;
            if (ncdfh.newGddContents.nSlnTypeNames.Contains(slnName.Value))
            {
                idx = ncdfh.newGddContents.nSlnTypeNames.IndexOf(slnName.Value);
            }
            else {
                Debug.Log("this type of values is not been read " + slnName.Value + "\n");
                return;
            }

            if (bReCreate.Value)
            {
                ncdcm.CreatVisualObjects(ncdfh.newGddContents.NodeCoord, ncdfh.newGddContents.nSlnTypeValues[0][idx], ncdfh.newGddContents.nSlnMaxValues[0][idx], ncdfh.newGddContents.nSlnMinValues[0][idx]);
            }
            else {
                ncdcm.CreatVisualObjects(ncdfh.newGddContents.nSlnTypeValues[0][idx], ncdfh.newGddContents.nSlnMaxValues[0][idx], ncdfh.newGddContents.nSlnMinValues[0][idx]);
            }
         
        }
       
    }
}

