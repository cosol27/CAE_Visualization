using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralData
{
    public string ModelName;

    //节点信息
    public int NodeNumber;  //节点总数
    public int NodeInteriorNumber;  //内部点总数
    public int NodeExteriorNumber;  //外部点总数

    public List<double[]> NodeCoord;    //节点坐标
    public double Coor_Xmax = double.MinValue,
                  Coor_Xmin = double.MaxValue,
                  Coor_Ymax = double.MinValue,
                  Coor_Ymin = double.MaxValue,
                  Coor_Zmax = double.MinValue,
                  Coor_Zmin = double.MaxValue;
    //时间步信息
    public float Time;

    //单元信息
    public int ElemNumber;
    public int ElemInteriorNumber;
    public int ElemExteriorNumber;
    public List<int[]> Elem_NodesIndex; //单元索引序号

    //节点解
    public List<string> nSlnTypeNames;  //物理量名称
    public List<double>[] nSlnTypeValue;    //物理量结果（标量）
    public List<double[]>[] nSlnTypeValues; //物理量结果（矢量）
    public double[] nSlnMaxValue;
    public double[] nSlnMinValue;
    //新增
    public Mesh[] Mesh_NodesIndex; //网格mesh（考虑到一个物体具有多个网格,气象数据）

    //单元解
    public List<string> eSlnTypeNames;
    public List<double> eSlnTypeValue;
    public List<double[]> eSlnTypeValues;
    public double[] eSlnMaxValue;
    public double[] eSlnMinValue;
	
}

public class Dynamic_GeneralData      //动态通用数据结构
{
    public string ModelName;

    //节点信息
    public int NodeNumber;  //节点总数
    public int NodeInteriorNumber;  //内部点总数
    public int NodeExteriorNumber;  //外部点总数

    public List<float[]> NodeCoord;    //节点坐标
    public double Coor_Xmax = float.MinValue,
                  Coor_Xmin = float.MaxValue,
                  Coor_Ymax = float.MinValue,
                  Coor_Ymin = float.MaxValue,
                  Coor_Zmax = float.MinValue,
                  Coor_Zmin = float.MaxValue;

    //时间步
    public List<float> TimeSteps;

    //单元信息
    public int ElemNumber;
    public int ElemInteriorNumber;
    public int ElemExteriorNumber;
    public List<int[]> Elem_NodesIndex; //单元索引序号

    //mesh
    public List<Mesh[]> ObjectMeshes;

    //节点解
    public List<string> nSlnTypeNames;  //物理量名称
    public List<List<float[]>> nSlnTypeValues;    //物理量结果（这里暂时不区分标量矢量，留待后续需要时扩展）
    public List<List<float>> nSlnMaxValues;
    public List<List<float>> nSlnMinValues;

    //单元解 （暂时不用单元解，单元解能呈现的云图，节点解也可以呈现）
    //public List<string> eSlnTypeNames;
    //public List<List<double>> eSlnTypeValue;
    //public List<List<double[]>> eSlnTypeValues;
    //public List<double[]> eSlnMaxValue;
    //public List<double[]> eSlnMinValue;
    public Dynamic_GeneralData()
    {
        ObjectMeshes = new List<Mesh[]>();
        NodeNumber = 0;
        NodeCoord = new List<float[]>();
        TimeSteps = new List<float>();// always 1
        nSlnTypeNames = new List<string>();
        nSlnTypeValues = new List<List<float[]>>();
        nSlnMaxValues = new List<List<float>>();
        nSlnMinValues = new List<List<float>>();
    }

}

public class NodeDataStructure
{
    public double[] NodeCoord;
    public double[] nSlnTypeValues;    //物理量结果（这里暂时不区分标量矢量，留待后续需要时扩展）
    public byte[] BoolTime;
}
