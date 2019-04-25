using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class ReadData
{
    private static string[] SplitFlag = new string[] { "Name", "Data", "Faces" };
    public static GeneralDataOld CFD_Post_Static_Data(string path)
    {
        if (File.Exists(path))
        {
            GeneralDataOld gddd = new GeneralDataOld();
            StreamReader sr = new StreamReader(path, System.Text.Encoding.ASCII);

            string line;
            string[] arg;
            int icount = 0;

            do
            {
                line = sr.ReadLine();
                arg = RemoveComma(line);
                ++icount;
                if (icount > 10)
                    break;
            } while (arg.Length == 0);     //除去空行

            if (arg.Length!=0)
                Debug.Log("标志位1: " + arg[0]);   //  读取到第一行非空行

            if (arg.Length != 0 && arg[0].Contains(SplitFlag[0]))
            {
                line = sr.ReadLine();
                arg = RemoveComma(line);
                gddd.ModelName = arg[0];    //添加模型名称
                Debug.Log("模型名称:" + arg[0]);
            }

            icount = 0;
            do
            {
                line = sr.ReadLine();
                arg = RemoveComma(line);
                if (icount > 10)
                    break;
            } while (arg.Length == 0);     //除去空行
            
            if(arg.Length!=0)
                Debug.Log("标志位2: " + arg[0]);   //  读取到标志位[Data]

            if (arg.Length != 0 && arg[0].Contains(SplitFlag[1]))
            {
                line = sr.ReadLine();
                arg = RemoveComma(line);
                if (arg.Length > 4)
                {
                    gddd.nSlnTypeNames = new List<string>();
                    for (int i = 4; i < arg.Length; ++i)
                        gddd.nSlnTypeNames.Add(arg[i]);
                }

                //初始化数组
                int SlnLen = gddd.nSlnTypeNames.Count;
                gddd.nSlnTypeValue = new List<double>[SlnLen];
                gddd.nSlnMaxValue = new double[SlnLen];
                gddd.nSlnMinValue = new double[SlnLen];

                for(int j = 0; j < SlnLen; ++j)
                {
                    gddd.nSlnTypeValue[j] = new List<double>();
                    gddd.nSlnMaxValue[j] = double.MinValue;
                    gddd.nSlnMinValue[j] = double.MaxValue;
                }
                gddd.NodeCoord = new List<double[]>();

                //gddd.Coor_Xmax = double.MinValue;
                //gddd.Coor_Xmin = double.MaxValue;
                //gddd.Coor_Ymax = double.MinValue;
                //gddd.Coor_Ymin = double.MaxValue;
                //gddd.Coor_Zmax = double.MinValue;
                //gddd.Coor_Zmin = double.MaxValue;       //初始化节点坐标极值

                do
                {
                    line = sr.ReadLine();
                    arg = RemoveComma(line);
                    if (arg.Length >= 4)
                    {
                        double[] cd = new double[3];
                        double.TryParse(arg[1], out cd[0]);
                        double.TryParse(arg[2], out cd[1]);
                        double.TryParse(arg[3], out cd[2]);
                        gddd.NodeCoord.Add(cd);     //添加节点坐标

                        if (cd[0] > gddd.Coor_Xmax)     // x最大值
                            gddd.Coor_Xmax = cd[0];
                        if (cd[0] < gddd.Coor_Xmin)     // x最小值
                            gddd.Coor_Xmin = cd[0];
                        if (cd[1] > gddd.Coor_Ymax)     // y最大值
                            gddd.Coor_Ymax = cd[1];
                        if (cd[1] < gddd.Coor_Ymin)     // y最小值
                            gddd.Coor_Ymin = cd[1];
                        if (cd[2] > gddd.Coor_Zmax)     // z最大值
                            gddd.Coor_Zmax = cd[2];
                        if (cd[2] < gddd.Coor_Zmin)     // z最小值
                            gddd.Coor_Zmin = cd[2];


                        if (arg.Length - 4 == SlnLen)
                        {
                            for (int j = 0; j < SlnLen; ++j)
                            {
                                double db;
                                if (double.TryParse(arg[j + 4], out db))
                                {
                                    gddd.nSlnTypeValue[j].Add(db);      //添加第j+4列物理量数据
                                    if (db > gddd.nSlnMaxValue[j])
                                        gddd.nSlnMaxValue[j] = db;      //判断是否为最大值
                                    if (db < gddd.nSlnMinValue[j])
                                        gddd.nSlnMinValue[j] = db;      //判断是否为最小值
                                }
                                else
                                    Debug.Log("错误转换：" + arg[j + 4] + ", " + arg[1] + ", " + arg[2] + ", " + arg[3]);
                            }
                        }
                        else
                        {
                            Debug.Log("错误：物理量数据与结构不一致");
                        }

                    }
                    else
                    {
                        if (arg.Length != 0 && arg.Length != 1)
                            Debug.Log("提示：当前文件不包含完整物理量数据！");
                    }
                } while (arg.Length != 0);      //读取[Data]里的数据
                
            }
            gddd.NodeNumber = gddd.NodeCoord.Count;
            gddd.NodeExteriorNumber = gddd.NodeNumber;

            icount = 0;
            do
            {
                line = sr.ReadLine();
                arg = RemoveComma(line);
                ++icount;
                if (arg == null)
                {
                    Debug.Log("读取到行Null");
                    break;
                }
                if (icount > 10)
                    break;
                Debug.Log("正在循环……" + icount);
            } while (arg.Length == 0);     //除去空行

            if (arg.Length != 0)
                Debug.Log("标志位3: " + arg[0]);   //  读取到标志位[Faces]

            if (arg.Length != 0 && arg[0].Contains(SplitFlag[2]))
            {
                Debug.Log("正在设置单元……");
                gddd.Elem_NodesIndex = new List<int[]>();
                do
                {
                    line = sr.ReadLine();
                    arg = RemoveComma(line);
                    if (arg.Length >= 3)
                    {
                        int[] cd = new int[3];
                        int.TryParse(arg[0], out cd[0]);
                        int.TryParse(arg[1], out cd[1]);
                        int.TryParse(arg[2], out cd[2]);
                        gddd.Elem_NodesIndex.Add(cd);
                    }
                    else
                    {
                        if (arg.Length != 0)
                            Debug.Log("位置信息不足：" + arg);
                    }
                } while (arg.Length != 0);
                gddd.ElemNumber = gddd.Elem_NodesIndex.Count;
                gddd.ElemExteriorNumber = gddd.ElemNumber;
            }

            sr.Close();
            return gddd;
        }
        
        else
        {
            Debug.Log("Wrong Path:" + path + ", file doesn't exist.");
            return null;
        }
    }

    internal  static string[] RemoveComma(string str)
    {
        if (str == "" || str == null || str.Trim() == "")
        {
            return new string[] { };
        }
        return Regex.Split(str, ",", RegexOptions.IgnoreCase);
    }
    /// <summary>
    /// 合并行字符串中的空格，并返回以空格为分割的字符串数组
    /// </summary>
    /// <param name="str">待分割的字符串</param>
    /// <returns></returns>
    internal static string[] RemoveSpace(string str)
    {
        if (str == "" || str == null || str.Trim() == "")
        {
            return new string[] { };
        }
        str = new Regex("[\\s]+").Replace(str.Trim(), " ");
        return Regex.Split(str, " ", RegexOptions.IgnoreCase);
    }



    /// <summary>
    /// 从导出的动态数据中读取节点和单元组成顺序和结构，存储到通用数据结构中
    /// </summary>
    /// <param name="path">文件夹路径，包含NLIST、ELIST及子步数据</param>
    /// <param name="NodeKeyWord">读取节点数据时所需要的关键字</param>
    /// <param name="ElemKeyWord">读取单元构成序列时所需要的关键字</param>
    /// <returns></returns>
    public static GeneralDataOld CFD_Post_Dynamic_Data_CoorAndIndex(string path, string NodeKeyWord, string ElemKeyWord)
    {
        GeneralDataOld _gd = CFD_Post_Dynamic_Data_ReadNodeCoor(path + @"/NLIST.lis", NodeKeyWord);     //节点数据文件路径
        _gd = CFD_Post_Dynamic_Data_ReadElemIndex(_gd, path + @"/ELIST.lis", ElemKeyWord);           //单元数据文件路径
        Debug.Log("节点坐标与单元构造序列读取完成！");
        
        //_gd = CFD_Post_Dynamic_Data_ReadNodeStreSol(_gd, path + @"/1/nsol-s-1-5.lis", "NODE");
        //_gd = CFD_Post_Dynamic_Data_ReadNodeStreSol(_gd, path + @"/11/nsol-s-11-21.lis", "NODE");
        //_gd = CFD_Post_Dynamic_Data_ReadNodeDisSol(_gd, path + @"/1/nsol-d-1-5.lis", "NODE");
        
        return _gd;
    }

    /// <summary>
    /// 从传入的节点文件路径中读取节点坐标，并存储到通用数据结构中
    /// </summary>
    /// <param name="NodePath">节点文件路径</param>
    /// <param name="NodeKeyWord">读取节点行关键字</param>
    /// <returns></returns>
    public static GeneralDataOld CFD_Post_Dynamic_Data_ReadNodeCoor(string NodePath, string NodeKeyWord)
    {

        string line;
        string[] arg;
        int lcount = 0;
        GeneralDataOld _gd = new GeneralDataOld()
        {
            NodeCoord = new List<double[]>()
        };
        if (!File.Exists(NodePath))
        {
            Debug.Log("节点数据不存在");
            return null;
        }

        StreamReader sr = new StreamReader(NodePath, System.Text.Encoding.ASCII);

        while (sr.Peek() > 0)       //确定read的文件是否结束了,如果结束了会返回int型 -1 
        {
            do{
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                ++lcount;
                if (lcount > 100)
                {
                    lcount = 0;
                    break;
                }
            } while (arg.Length == 0 || arg[0] != NodeKeyWord);     //除去空行 及过滤无效行

            do{
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                if (arg.Length > 3)
                {
                    double[] cd = new double[3];
                    double.TryParse(arg[1], out cd[0]);
                    double.TryParse(arg[2], out cd[1]);
                    double.TryParse(arg[3], out cd[2]);
                    _gd.NodeCoord.Add(cd);
                }
            } while (arg.Length > 0);     //读取数据行

        }
        _gd.NodeNumber = _gd.NodeCoord.Count;
        Debug.Log("节点个数: " + _gd.NodeNumber);
        //foreach (double d in _gd.NodeCoord[_gd.NodeNumber - 1])
        //    Debug.Log(d);

        sr.Close();
        return _gd;
    }

    /// <summary>
    /// 读取节点构造顺序，并存储到通用数据结构中
    /// </summary>
    /// <param name="_gdd">通用数据结构</param>
    /// <param name="ElemPath">单元文件路径</param>
    /// <param name="ElemKeyWord">读取关键字</param>
    /// <returns></returns>
    public static GeneralDataOld CFD_Post_Dynamic_Data_ReadElemIndex(GeneralDataOld _gdd, string ElemPath, string ElemKeyWord)
    {
        string line;
        string[] arg;
        int lcount = 0;

        if (!File.Exists(ElemPath))
        {
            Debug.Log("单元数据不存在");
            return null;
        }

        StreamReader sr = new StreamReader(ElemPath, System.Text.Encoding.ASCII);
        _gdd.Elem_NodesIndex = new List<int[]>();

        while (sr.Peek() > 0)       //确定read的文件是否结束了,如果结束了会返回int型 -1 
        {
            do
            {
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                ++lcount;
                if (lcount > 100)
                {
                    lcount = 0;
                    break;
                }
            } while (arg.Length == 0 || arg[0] != ElemKeyWord);     //除去空行 及过滤无效行
            arg = RemoveSpace(sr.ReadLine());       //除去关键字与数据行之间的空行
            do
            {
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                //Debug.Log(arg.Length);
                //if (arg.Length > 9)
                //    Debug.Log(arg[0] + ": " + arg[6] + " " + arg[7] + " " + arg[8] + " " + arg[9]);
                if (arg.Length > 9)
                {

                    int[] cd1 = new int[3];
                    int.TryParse(arg[6], out cd1[0]);
                    int.TryParse(arg[8], out cd1[1]);
                    int.TryParse(arg[7], out cd1[2]);

                    //文件存储序列从1开始，所以减1
                    for (int id = 0; id < cd1.Length; ++id)
                        cd1[id] -= 1;
                    _gdd.Elem_NodesIndex.Add(cd1);
                    
                    int[] cd2 = new int[3];
                    int.TryParse(arg[6], out cd2[0]);
                    int.TryParse(arg[9], out cd2[1]);
                    int.TryParse(arg[8], out cd2[2]);
                    
                    //文件存储序列从1开始，所以减1
                    for (int id = 0; id < cd2.Length; ++id)
                        cd2[id] -= 1;
                    _gdd.Elem_NodesIndex.Add(cd2);
                }       // 写入单元信息
                else
                {
                    if (arg.Length != 0)
                        Debug.Log("位置信息不足：" + arg);
                }
            } while (arg.Length > 0);     //读取数据行
        }
        _gdd.ElemNumber = _gdd.Elem_NodesIndex.Count;
        Debug.Log("单元个数: " + _gdd.Elem_NodesIndex.Count);
        sr.Close();
        return _gdd;
    }

    /// <summary>
    /// 读取节点解，并赋值给通用数据
    /// </summary>
    /// <param name="_gdd">通用数据结构</param>
    /// <param name="NodeSolPath">节点解文件路径</param>
    /// <param name="NodeSolKeyWord">读取关键字</param>
    /// <returns></returns>
    public static GeneralDataOld CFD_Post_Dynamic_Data_ReadNodeDisSol(GeneralDataOld _gdd, string NodeSolPath, string NodeSolKeyWord)
    {
        string line;
        string[] arg;
        int lcount = 0, syb;
        bool stop_flag = false;

        if (!File.Exists(NodeSolPath))
        {
            Debug.Log("节点数据不存在");
            return null;
        }

        StreamReader sr = new StreamReader(NodeSolPath, System.Text.Encoding.ASCII);

        _gdd.nSlnTypeNames = new List<string>();
        _gdd.nSlnTypeNames.Add("Node Solution");
        //初始化 节点数据链表
        _gdd.nSlnTypeValue = new List<double>[_gdd.nSlnTypeNames.Count];
        _gdd.nSlnMaxValue = new double[_gdd.nSlnTypeNames.Count];
        _gdd.nSlnMinValue = new double[_gdd.nSlnTypeNames.Count];
        for (int j = 0; j < _gdd.nSlnTypeNames.Count; ++j)
        {
            _gdd.nSlnTypeValue[j] = new List<double>();
            _gdd.nSlnMaxValue[j] = double.MinValue;
            _gdd.nSlnMinValue[j] = double.MaxValue;
        }
        while (sr.Peek() > 0)       //确定read的文件是否结束了,如果结束了会返回int型 -1 
        {
            do
            {
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                ++lcount;
                if (lcount > 1000)
                {
                    lcount = 0;
                    break;
                }
            } while (arg.Length == 0 || arg[0] != NodeSolKeyWord);     //除去空行 及过滤无效行

            do
            {
                line = sr.ReadLine();
                if (line != null && line.Trim() != "")
                {
                    double cd = new double();
                    if (line.Length < 57)
                    {
                        Debug.Log("行过短: " + line);
                        continue;
                    }
                    if (int.TryParse(line.Substring(0, 9).Trim(), out syb))        //判断是否为数据行
                    {
                        double.TryParse(line.Substring(45, 12).Trim(), out cd);
                        for (int j = 0; j < _gdd.nSlnTypeNames.Count; ++j)
                        {
                            _gdd.nSlnTypeValue[j].Add(cd);
                            if (cd > _gdd.nSlnMaxValue[j])
                                _gdd.nSlnMaxValue[j] = cd;
                            else if (cd < _gdd.nSlnMaxValue[j])
                                _gdd.nSlnMinValue[j] = cd;
                        }
                    }
                    else
                    {
                        arg = RemoveSpace(line);
                        Debug.Log("非数据行，标志位: " + arg[0] + ",已读数据长度: " + _gdd.nSlnTypeValue[0].Count);
                        //stop_flag = true;
                        break;
                    }
                    
                    //Debug.Log("行数: " + line.Substring(0, 9).Trim());
                    //Debug.Log("UX: " + line.Substring(9, 12).Trim());
                    //Debug.Log("UY: " + line.Substring(21, 12).Trim());
                    //Debug.Log("UZ: " + line.Substring(33, 12).Trim());
                    //Debug.Log("USUM: " + line.Substring(45, 12).Trim());

                }
                else
                {
                    if (line.Trim() != "")
                        Debug.Log("节点数据量不充足" + line);
                }
            } while (line.Trim() != "");     //读取数据行

            //if (stop_flag)
            //{
            //    Debug.Log("暂停，行号标志位: " + arg[0]);
            //    break;
            //}
        }
        Debug.Log("数据长度: " + _gdd.nSlnTypeValue[0].Count);

        sr.Close();
        return _gdd;
    }


    /// <summary>
    /// 读取节点解，并赋值给通用数据
    /// </summary>
    /// <param name="_gdd">通用数据结构</param>
    /// <param name="NodeSolPath">节点解文件路径</param>
    /// <param name="NodeSolKeyWord">读取关键字</param>
    /// <returns></returns>
    public static GeneralDataOld CFD_Post_Dynamic_Data_ReadNodeStreSol(GeneralDataOld _gdd, string NodeSolPath, string NodeSolKeyWord)
    {
        string line;
        string[] arg;
        int lcount = 0, syb;
        //bool stop_flag = false;

        if (!File.Exists(NodeSolPath))
        {
            Debug.Log("节点数据不存在");
            return null;
        }

        StreamReader sr = new StreamReader(NodeSolPath, System.Text.Encoding.ASCII);

        _gdd.nSlnTypeNames = new List<string>
        {
            "Node Solution"
        };
        //初始化 节点数据链表
        _gdd.nSlnTypeValue = new List<double>[_gdd.nSlnTypeNames.Count];
        _gdd.nSlnMaxValue = new double[_gdd.nSlnTypeNames.Count];
        _gdd.nSlnMinValue = new double[_gdd.nSlnTypeNames.Count];
        for (int j = 0; j < _gdd.nSlnTypeNames.Count; ++j)
        {
            _gdd.nSlnTypeValue[j] = new List<double>();
            _gdd.nSlnMaxValue[j] = double.MinValue;
            _gdd.nSlnMinValue[j] = double.MaxValue;
        }
        while (sr.Peek() > 0)       //确定read的文件是否结束了,如果结束了会返回int型 -1 
        {
            do
            {
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                //if (arg.Length > 2 && arg[0] == TsKeyWord)
                //    _gdd.Time = Convert.ToSingle(arg[1]);
                ++lcount;
                if (lcount > 1000)
                {
                    lcount = 0;
                    break;
                }
            } while (arg.Length == 0 || arg[0] != NodeSolKeyWord);     //除去空行 及过滤无效行

            do
            {
                line = sr.ReadLine();
                if (line != null && line.Trim() != "")
                {
                    double cd = new double();
                    if (line.Length < 57)
                    {
                        Debug.Log("行过短: " + line);
                        continue;
                    }
                    if (int.TryParse(line.Substring(0, 9).Trim(), out syb))        //判断是否为数据行
                    {
                        double.TryParse(line.Substring(line.Length - 12, 12).Trim(), out cd);
                        for (int j = 0; j < _gdd.nSlnTypeNames.Count; ++j)
                        {
                            _gdd.nSlnTypeValue[j].Add(cd);
                            if (cd > _gdd.nSlnMaxValue[j])
                                _gdd.nSlnMaxValue[j] = cd;
                            else if (cd < _gdd.nSlnMaxValue[j])
                                _gdd.nSlnMinValue[j] = cd;
                        }
                    }
                    else
                    {
                        //arg = RemoveSpace(line);
                        //Debug.Log("过滤非数据行，标志位: " + arg[0] + ",已读数据长度: " + _gdd.nSlnTypeValue[0].Count);
                        //stop_flag = true;
                        break;
                    }

                    //Debug.Log("行数: " + line.Substring(0, 9).Trim());
                    //Debug.Log("UX: " + line.Substring(9, 12).Trim());
                    //Debug.Log("UY: " + line.Substring(21, 12).Trim());
                    //Debug.Log("UZ: " + line.Substring(33, 12).Trim());
                    //Debug.Log("USUM: " + line.Substring(45, 12).Trim());

                }
                else
                {
                    //Debug.Log("节点数据量不充足" + line);
                }
                if (line == null)
                    break;
            } while (line.Trim() != "");     //读取数据行

            //if (stop_flag)
            //{
            //    Debug.Log("暂停，行号标志位: " + arg[0]);
            //    break;
            //}
        }
        Debug.Log("数据长度: " + _gdd.nSlnTypeValue[0].Count);

        sr.Close();
        return _gdd;
    }

    public static List<float> CFD_Post_Dynamic_Data_TimeStep(string path, int LimitStep, string TsKeyWord)
    {
        string line;
        string[] arg;
        int lcount = 0;
        List<float> time = new List<float>();

        if (!File.Exists(path))
        {
            Debug.Log("时间步文件不存在: " + path);
            return null;
        }

        StreamReader sr = new StreamReader(path, System.Text.Encoding.ASCII);

        while (sr.Peek() > 0)       //确定read的文件是否结束了,如果结束了会返回int型 -1 
        {
            do
            {
                line = sr.ReadLine();
                arg = RemoveSpace(line);
                ++lcount;
                if (lcount > 1000)
                {
                    lcount = 0;
                    break;
                }
            } while (arg.Length == 0 || arg[0] != TsKeyWord);     //除去空行 及过滤无效行

            do
            {
                line = sr.ReadLine();
                arg = RemoveSpace(line);

                if (arg.Length > 4)
                {
                    float cd = new float();

                    if (float.TryParse(arg[1].Trim(), out cd))        //判断是否为数据行
                    {
                        time.Add(cd);
                        --LimitStep;
                        if (LimitStep == 0)
                            return time;
                    }
                    else
                    {
                        //arg = RemoveSpace(line);
                        //Debug.Log("过滤非数据行，标志位: " + arg[0] + ",已读数据长度: " + _gdd.nSlnTypeValue[0].Count);
                        //stop_flag = true;
                        break;
                    }

                }
                else
                {
                    Debug.Log("行过短: " + line);
                    continue;
                }
                if (line == null)
                    break;
            } while (line.Trim() != "");     //读取数据行

        }
        return time;
    }

}
