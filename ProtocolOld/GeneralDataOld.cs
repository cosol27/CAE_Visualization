using System.Collections.Generic;

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

    public List<double[]> NodeCoord;    //节点坐标
    public double Coor_Xmax = double.MinValue,
                  Coor_Xmin = double.MaxValue,
                  Coor_Ymax = double.MinValue,
                  Coor_Ymin = double.MaxValue,
                  Coor_Zmax = double.MinValue,
                  Coor_Zmin = double.MaxValue;

    //时间步
    public List<float> TimeSteps;

    //单元信息
    public int ElemNumber;
    public int ElemInteriorNumber;
    public int ElemExteriorNumber;
    public List<int[]> Elem_NodesIndex; //单元索引序号

    //节点解
    public List<string> nSlnTypeNames;  //物理量名称
    public List<List<double>[]> nSlnTypeValues;    //物理量结果（这里暂时不区分标量矢量，留待后续需要时扩展）
    public List<double[]> nSlnMaxValues;
    public List<double[]> nSlnMinValues;

    //单元解 （暂时不用单元解，单元解能呈现的云图，节点解也可以呈现）
    //public List<string> eSlnTypeNames;
    //public List<List<double>> eSlnTypeValue;
    //public List<List<double[]>> eSlnTypeValues;
    //public List<double[]> eSlnMaxValue;
    //public List<double[]> eSlnMinValue;
    public Dynamic_GeneralData()
    {
        TimeSteps = new List<float>();
        //nSlnTypeNames = new List<string>();
        nSlnTypeValues = new List<List<double>[]>();
        nSlnMaxValues = new List<double[]>();
        nSlnMinValues = new List<double[]>();
    }

}

//暂时没有对NodeDataStructure的引用
public class NodeDataStructure
{
    public double[] NodeCoord;
    public double[] nSlnTypeValues;    //物理量结果（这里暂时不区分标量矢量，留待后续需要时扩展）
    public byte[] BoolTime;
}
