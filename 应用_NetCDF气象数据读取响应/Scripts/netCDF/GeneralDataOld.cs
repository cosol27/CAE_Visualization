using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDataOld
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
    //位移解
    public List<double[]> NodeDisplacement;    //节点位移解

    //节点解
    public List<string> nSlnTypeNames;  //物理量名称
    public List<double>[] nSlnTypeValue;    //物理量结果（标量）
    public List<double[]>[] nSlnTypeValues; //物理量结果（矢量）
    public double[] nSlnMaxValue;
    public double[] nSlnMinValue;

    //单元信息
    public int ElemNumber;
    public int ElemInteriorNumber;
    public int ElemExteriorNumber;
    public List<int[]> Elem_NodesIndex; //单元索引序号
    //新增
    public Mesh[]  Mesh_NodesIndex; //网格mesh（考虑到一个物体具有多个网格,气象数据）


    //单元解
    public List<string> eSlnTypeNames;
    public List<double> eSlnTypeValue;
    public List<double[]> eSlnTypeValues;
    public double[] eSlnMaxValue;
    public double[] eSlnMinValue;

}
