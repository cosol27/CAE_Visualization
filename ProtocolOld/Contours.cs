using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 包含静态显示函数ShowColors(单层、双层);
/// 物理量显示标尺CreatPrefabStaff
/// </summary>

public class Contours : MonoBehaviour
{
    /// <summary>
    /// 获得根据有限元数据值对应的模型云图，单层模型
    /// </summary>
    /// <param name="obj">原始模型.</param>
    /// <param name="gdd">有限元数据值.</param>
    /// <param name="indexer">物理量编号.</param>
    /// <param name="isVerse">I <c>true</c>默认为true，用于设置面片的外法线.</param>
    /// <param name="Scale">模型数据缩放倍数.</param>
    public static void ShowColors(GameObject obj, GeneralDataOld gdd, int indexer, double max, double min, bool isVerse, float Scale)
    {
        if (obj == null)
        {
            return;
        }
        int NodeNumber = gdd.NodeCoord.Count;
        int ElemNumber = gdd.Elem_NodesIndex.Count;
        //获取模型顶点坐标值
        Vector3[] pos = new Vector3[NodeNumber];
        for (int i = 0; i < NodeNumber; i++)
        {
            pos[i] = new Vector3((float)gdd.NodeCoord[i][0] * Scale, (float)gdd.NodeCoord[i][1] * Scale, (float)gdd.NodeCoord[i][2] * Scale);
        }

        if (pos.Length < 65000)
        {
            //获取顶点的检索值
            int[] idx = new int[ElemNumber * 3];
            for (int j = 0; j < ElemNumber; j++)
            {
                idx[j * 3 + 0] = gdd.Elem_NodesIndex[j][0];
                //用于在法向方向的变换
                if (isVerse)
                {
                    idx[j * 3 + 1] = gdd.Elem_NodesIndex[j][2];
                    idx[j * 3 + 2] = gdd.Elem_NodesIndex[j][1];
                }
                else
                {
                    idx[j * 3 + 1] = gdd.Elem_NodesIndex[j][1];
                    idx[j * 3 + 2] = gdd.Elem_NodesIndex[j][2];
                }
            }
            //设置顶点的颜色
            Color[] cor = new Color[NodeNumber];
            if (indexer >= gdd.nSlnTypeNames.Count)
            {
                Debug.Log("错误： 物理量序号超出范围，请确认！");
                for (int i = 0; i < NodeNumber; ++i)
                {
                    cor[i] = Color.HSVToRGB(0, 1, 1);//将HSV的颜色值转为RGB的颜色值
                }
            }
            else
            {
                double fenmu = max - min;
                for (int i = 0; i < NodeNumber; ++i)
                {
                    double value = (max - gdd.nSlnTypeValue[indexer][i]) / fenmu;
                    value = value < 0 ? 0 : value;
                    value = value > 1 ? 1 : value;
                    cor[i] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
                }
            }
            Mesh mr = obj.GetComponent<MeshFilter>().mesh;//获取物体的mesh
            mr.Clear();//清空当前mesh
            mr.vertices = pos;//设置顶点
            mr.triangles = idx;//设置顶点将所致
            mr.colors = cor;//设置顶点颜色
            mr.RecalculateBounds();//
            mr.RecalculateNormals();//计算顶点的法向值
        }
        else
        {
            //修改格式为：
            //Tristan：012345678
            //vertice:点012345678
            //保证每个点索引都有一个点坐标，保证与索引一一对应。
            //以上方法的缺点是点数据有很多的冗余
            //以上方法便于模型的分割
            int[] tris = new int[ElemNumber * 3];
            Vector3[] poss = new Vector3[ElemNumber * 3];
            Color[] cors = new Color[ElemNumber * 3];

            double fenmu = max - min;
            double value = 0;
            for (int i = 0; i < ElemNumber; i++)
            {
                tris[3 * i + 0] = 3 * i + 0;
                tris[3 * i + 1] = 3 * i + 1;
                tris[3 * i + 2] = 3 * i + 2;
                poss[3 * i + 0] = new Vector3((float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][0]][0] * Scale,
                    (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][0]][2] * Scale, (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][0]][1] * Scale);
                poss[3 * i + 1] = new Vector3((float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][1]][0] * Scale,
                    (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][1]][2] * Scale, (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][1]][1] * Scale);
                poss[3 * i + 2] = new Vector3((float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][2]][0] * Scale,
                    (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][2]][2] * Scale, (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][2]][1] * Scale);
                value = (max - gdd.nSlnTypeValue[indexer][gdd.Elem_NodesIndex[i][0]]) / fenmu;
                value = value < 0 ? 0 : value;
                value = value > 1 ? 1 : value;
                cors[3 * i + 0] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
                value = (max - gdd.nSlnTypeValue[indexer][gdd.Elem_NodesIndex[i][1]]) / fenmu;
                value = value < 0 ? 0 : value;
                value = value > 1 ? 1 : value;
                cors[3 * i + 1] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
                value = (max - gdd.nSlnTypeValue[indexer][gdd.Elem_NodesIndex[i][2]]) / fenmu;
                value = value < 0 ? 0 : value;
                value = value > 1 ? 1 : value;
                cors[3 * i + 2] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
            }
            int maxObjLen = ElemNumber / 21666;
            int lastObjLen = ElemNumber % 21666;
            if (lastObjLen != 0)
                maxObjLen++;

            int MaxVerticeCount = 21666 * 3;//最大顶点数
            //Debug.Log(" :  " + MaxVerticeCount);

            int[] m_tri = new int[] { };//声明检索
            Vector3[] m_ver = new Vector3[] { };//声明顶点
            Color[] m_cor = new Color[] { };//声明颜色
            int VerticeCount = 0;//声明当前顶点数
            GameObject[] objShow = new GameObject[maxObjLen];
            for (int i = 0; i < maxObjLen; i++)
            {
                objShow[i] = GameObject.Instantiate(obj);
                objShow[i].name = obj.name + "_" + i.ToString();
                if (objShow[i].GetComponent<MeshFilter>() == null)
                {
                    objShow[i].AddComponent<MeshFilter>();
                }
                if (objShow[i].GetComponent<MeshRenderer>() == null)
                {
                    objShow[i].AddComponent<MeshRenderer>();
                }
                Mesh mr = objShow[i].GetComponent<MeshFilter>().mesh;//获取物体的mesh
                mr.Clear();//清空当前mesh
                VerticeCount = i < maxObjLen - 1 ? MaxVerticeCount : lastObjLen * 3;
                m_tri = new int[VerticeCount];
                m_ver = new Vector3[VerticeCount];
                m_cor = new Color[VerticeCount];
                for (int j = 0; j < VerticeCount; j++)
                {
                    m_tri[j] = tris[i * MaxVerticeCount + j] - i * MaxVerticeCount;
                    m_ver[j] = poss[i * MaxVerticeCount + j];
                    m_cor[j] = cors[i * MaxVerticeCount + j];
                }
                mr.vertices = m_ver;//设置顶点
                mr.triangles = m_tri;//设置顶点将所致
                mr.colors = m_cor;//设置顶点颜色
                mr.RecalculateBounds();//
                mr.RecalculateNormals();//计算顶点的法向值
            }
            for (int i = 0; i < maxObjLen; i++)
            {
                objShow[i].transform.parent = obj.transform;
            }
        }

    }


    /// <summary>
    /// 获得根据有限元数据值对应的模型云图，双层模型
    /// 1.区别于第一种，该种用于显示双层模型，防止从内部观察时出现看不到物体的现象
    /// 2.原理，用两个物体构建基于同一套坐标数据的正反法向的模型
    /// </summary>
    /// <param name="obj">原始模型.</param>
    /// <param name="obj_1">原始模型1.</param>
    /// <param name="gdd">有限元数据值.</param>
    /// <param name="max">颜色为红色（标尺上端）对应的最大值.</param>
    /// <param name="min">颜色为蓝色（标尺下端）对应的最小值.</param>
    /// <param name="indexer">物理量编号.</param>
    /// <param name="isVerse">I <c>true</c>默认为true，用于设置面片的外法线.</param>
    /// <param name="Scale">模型数据缩放倍数.</param>
    public static void ShowColors(GameObject obj, GameObject obj_1, GeneralDataOld gdd, int indexer, double max, double min, bool isVerse, float Scale)
    {
        if (obj == null)
        {
            Debug.Log("obj null");
            return;
        }
        else if (obj_1 == null)
        {
            Debug.Log("obj_1 null");
            return;
        }
        //vertices的数量大于65000
        int NodeNumber = gdd.NodeCoord.Count;
        int ElemNumber = gdd.Elem_NodesIndex.Count;
        if (NodeNumber == 0)
        {
            Debug.Log("NodeNumber null");
            return;
        }
        else if (ElemNumber == 0)
        {
            Debug.Log("ElemNumber null");
            return;
        }
        //获取模型顶点坐标值
        Vector3[] pos = new Vector3[NodeNumber];
        for (int i = 0; i < NodeNumber; i++)
        {
            pos[i] = new Vector3((float)gdd.NodeCoord[i][0] * Scale, (float)gdd.NodeCoord[i][2] * Scale, (float)gdd.NodeCoord[i][1] * Scale);
        }

        //判断顶点的个数是否超过65000
        if (pos.Length < 65000)
        {
            //获取顶点的检索值
            int[] idx = new int[ElemNumber * 3];
            for (int j = 0; j < ElemNumber; j++)
            {
                idx[j * 3 + 0] = gdd.Elem_NodesIndex[j][0];
                //用于在法向方向的变换
                //用于在法向方向的变换
                if (isVerse)
                {
                    idx[j * 3 + 1] = gdd.Elem_NodesIndex[j][2];
                    idx[j * 3 + 2] = gdd.Elem_NodesIndex[j][1];
                }
                else
                {
                    idx[j * 3 + 1] = gdd.Elem_NodesIndex[j][1];
                    idx[j * 3 + 2] = gdd.Elem_NodesIndex[j][2];
                }
            }
            //设置顶点的颜色
            Color[] cor = new Color[NodeNumber];
            if (indexer >= gdd.nSlnTypeNames.Count)
            {
                Debug.Log("错误： 物理量序号超出范围，请确认！");
                for (int i = 0; i < NodeNumber; i++)
                {
                    cor[i] = Color.HSVToRGB(0, 1, 1);//将HSV的颜色值转为RGB的颜色值//全为红色
                }
            }
            else
            {
                double fenmu_c = max - min;
                double value_c = 0;
                for (int i = 0; i < NodeNumber; i++)
                {
                    value_c = (max - gdd.nSlnTypeValue[indexer][i]) / fenmu_c;
                    value_c = value_c < 0 ? 0 : value_c;
                    value_c = value_c > 1 ? 1 : value_c;
                    cor[i] = Color.HSVToRGB((float)value_c * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
                }
            }
            Mesh mr = obj.GetComponent<MeshFilter>().mesh;//获取物体的mesh
            mr.Clear();//清空当前mesh
            mr.vertices = pos;//设置顶点
            mr.triangles = idx;//设置顶点将所致
            mr.colors = cor;//设置顶点颜色
            mr.RecalculateBounds();//
            mr.RecalculateNormals();//计算顶点的法向值

            obj.GetComponent<MeshCollider>().sharedMesh = mr;


            //for (int i = 0; i < 30; i++)
            //{
            //    Debug.Log(i + " :  " + mr.vertices[mr.triangles[i]].x + "  " + mr.vertices[mr.triangles[i]].y + "  " + mr.vertices[mr.triangles[i]].z);
            //    Debug.Log(i + " :  " + mr.colors[mr.triangles[i]].r + "  " + mr.colors[mr.triangles[i]].g + "  " + mr.colors[mr.triangles[i]].b);
            //}

            Mesh mr_1 = obj_1.GetComponent<MeshFilter>().mesh;//获取物体的mesh
            mr_1.Clear();//清空当前mesh
            //获取顶点的检索值
            int[] idx_1 = new int[ElemNumber * 3];
            for (int j = 0; j < ElemNumber; j++)
            {
                idx_1[j * 3 + 0] = gdd.Elem_NodesIndex[j][0];
                //用于在法向方向的变换
                //用于在法向方向的变换
                if (isVerse)
                {
                    idx_1[j * 3 + 1] = gdd.Elem_NodesIndex[j][1];
                    idx_1[j * 3 + 2] = gdd.Elem_NodesIndex[j][2];
                }
                else
                {
                    idx_1[j * 3 + 1] = gdd.Elem_NodesIndex[j][2];
                    idx_1[j * 3 + 2] = gdd.Elem_NodesIndex[j][1];
                }
            }

            mr_1.vertices = pos;//设置顶点
            mr_1.triangles = idx_1;//设置顶点将所致
            mr_1.colors = cor;//设置顶点颜色
            mr_1.RecalculateBounds();//
            mr_1.RecalculateNormals();//计算顶点的法向值

            obj_1.GetComponent<MeshCollider>().sharedMesh = mr_1;
        }
        else
        {
            //修改格式为：
            //Tristan：012345678
            //vertice:点012345678
            //保证每个点索引都有一个点坐标，保证与索引一一对应。
            //以上方法的缺点是点数据有很多的冗余
            //以上方法便于模型的分割
            int[] tris = new int[ElemNumber * 3];
            Vector3[] poss = new Vector3[ElemNumber * 3];
            Color[] cors = new Color[ElemNumber * 3];

            double fenmu = max - min;
            double value = 0;
            for (int i = 0; i < ElemNumber; i++)
            {
                tris[3 * i + 0] = 3 * i + 0;
                tris[3 * i + 1] = 3 * i + 1;
                tris[3 * i + 2] = 3 * i + 2;
                poss[3 * i + 0] = new Vector3((float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][0]][0] * Scale,
                    (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][0]][2] * Scale, (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][0]][1] * Scale);
                poss[3 * i + 1] = new Vector3((float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][1]][0] * Scale,
                    (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][1]][2] * Scale, (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][1]][1] * Scale);
                poss[3 * i + 2] = new Vector3((float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][2]][0] * Scale,
                    (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][2]][2] * Scale, (float)gdd.NodeCoord[gdd.Elem_NodesIndex[i][2]][1] * Scale);
                value = (max - gdd.nSlnTypeValue[indexer][gdd.Elem_NodesIndex[i][0]]) / fenmu;
                value = value < 0 ? 0 : value;
                value = value > 1 ? 1 : value;
                cors[3 * i + 0] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
                value = (max - gdd.nSlnTypeValue[indexer][gdd.Elem_NodesIndex[i][1]]) / fenmu;
                value = value < 0 ? 0 : value;
                value = value > 1 ? 1 : value;
                cors[3 * i + 1] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
                value = (max - gdd.nSlnTypeValue[indexer][gdd.Elem_NodesIndex[i][2]]) / fenmu;
                value = value < 0 ? 0 : value;
                value = value > 1 ? 1 : value;
                cors[3 * i + 2] = Color.HSVToRGB((float)value * 2 / 3, 1, 1);//将HSV的颜色值转为RGB的颜色值
            }
            //for (int i = 0; i < poss.Length; i++)
            //{
            //    Debug.Log(i + " :  " + poss[i].x + "  " + poss[i].y + "  " + poss[i].z);
            //    Debug.Log(i + " :  " + cors[i].r + "  " + cors[i].g + "  " + cors[i].b);
            //}
            int maxObjLen = ElemNumber / 21666;
            int lastObjLen = ElemNumber % 21666;
            if (lastObjLen != 0)
                maxObjLen++;

            int MaxVerticeCount = 21666 * 3;//最大顶点数
            Debug.Log(" :  " + MaxVerticeCount);

            int[] m_tri = new int[] { };//声明检索
            Vector3[] m_ver = new Vector3[] { };//声明顶点
            Color[] m_cor = new Color[] { };//声明颜色
            int VerticeCount = 0;//声明当前顶点数
            GameObject[] objShow = new GameObject[maxObjLen];
            for (int i = 0; i < maxObjLen; i++)
            {
                objShow[i] = GameObject.Instantiate(obj);
                objShow[i].name = obj.name + "_" + i.ToString();
                if (objShow[i].GetComponent<MeshFilter>() == null)
                {
                    objShow[i].AddComponent<MeshFilter>();
                }
                if (objShow[i].GetComponent<MeshRenderer>() == null)
                {
                    objShow[i].AddComponent<MeshRenderer>();
                }
                if (objShow[i].GetComponent<MeshCollider>() == null)
                {
                    objShow[i].AddComponent<MeshCollider>();
                }
                Mesh mr = objShow[i].GetComponent<MeshFilter>().mesh;//获取物体的mesh
                mr.Clear();//清空当前mesh
                VerticeCount = i < maxObjLen - 1 ? MaxVerticeCount : lastObjLen * 3;
                m_tri = new int[VerticeCount];
                m_ver = new Vector3[VerticeCount];
                m_cor = new Color[VerticeCount];
                for (int j = 0; j < VerticeCount; j++)
                {
                    m_tri[j] = tris[i * MaxVerticeCount + j] - i * MaxVerticeCount;
                    m_ver[j] = poss[i * MaxVerticeCount + j];
                    m_cor[j] = cors[i * MaxVerticeCount + j];
                }
                mr.vertices = m_ver;//设置顶点
                mr.triangles = m_tri;//设置顶点将所致
                mr.colors = m_cor;//设置顶点颜色
                mr.RecalculateBounds();//
                mr.RecalculateNormals();//计算顶点的法向值

                objShow[i].GetComponent<MeshCollider>().sharedMesh = mr;
            }
            //for (int i = 0; i < maxObjLen; i++)
            //{
            //    objShow[i].transform.parent = obj.transform;
            //}

            GameObject[] objShow_1 = new GameObject[maxObjLen];
            for (int i = 0; i < maxObjLen; i++)
            {
                objShow_1[i] = GameObject.Instantiate(obj_1);
                objShow_1[i].name = obj_1.name + "_" + i.ToString();
                if (objShow_1[i].GetComponent<MeshFilter>() == null)
                {
                    objShow_1[i].AddComponent<MeshFilter>();
                }
                if (objShow_1[i].GetComponent<MeshRenderer>() == null)
                {
                    objShow_1[i].AddComponent<MeshRenderer>();
                }
                if (objShow_1[i].GetComponent<MeshCollider>() == null)
                {
                    objShow_1[i].AddComponent<MeshCollider>();
                }
                Mesh mr = objShow_1[i].GetComponent<MeshFilter>().mesh;//获取物体的mesh
                mr.Clear();//清空当前mesh
                VerticeCount = i < maxObjLen - 1 ? MaxVerticeCount : lastObjLen * 3;
                m_tri = new int[VerticeCount];
                m_ver = new Vector3[VerticeCount];
                m_cor = new Color[VerticeCount];
                for (int j = 0; j < VerticeCount; j++)
                {
                    //设置外法向的方向
                    m_tri[j] =
                        j % 3 == 1 ? tris[i * MaxVerticeCount + j] - i * MaxVerticeCount + 1 :
                        j % 3 == 2 ? tris[i * MaxVerticeCount + j] - i * MaxVerticeCount - 1 :
                        tris[i * MaxVerticeCount + j] - i * MaxVerticeCount;
                    m_ver[j] = poss[i * MaxVerticeCount + j];
                    m_cor[j] = cors[i * MaxVerticeCount + j];
                }
                mr.vertices = m_ver;//设置顶点
                mr.triangles = m_tri;//设置顶点将所致
                mr.colors = m_cor;//设置顶点颜色
                mr.RecalculateBounds();//
                mr.RecalculateNormals();//计算顶点的法向值

                objShow_1[i].GetComponent<MeshCollider>().sharedMesh = mr;
            }
            for (int i = 0; i < maxObjLen; i++)
            {
                objShow[i].transform.parent = obj.transform;
                objShow_1[i].transform.parent = obj_1.transform;
            }
        }
    }


    public static void Dynamic_ShowColor(GameObject obj, GameObject obj_1, GeneralDataOld gdd, List<string> NodeSubSol_FilePath_List, int indexer, bool isVerse, float Scale)
    {
        if (NodeSubSol_FilePath_List == null)
        {
            Debug.Log("子步文件路径序列为空");
            return;
        }
        //StopAllCoroutines();
        //StartCoroutine("DEAL");
        //IEnumerator DEAL()
        //{
        //    foreach (string st in NodeSubSol_FilePath_List)
        //    {
        //        Sub_Dynamic_ShowColor(obj, obj_1, gdd, st, indexer, isVerse, Scale);
        //        yield return new WaitForEndOfFrame();
        //    }
        //}

    }

    /// <summary>
    /// 动态显示每一子步每一帧的表面标量云图
    /// </summary>
    /// <param name="obj">原始模型</param>
    /// <param name="obj_1">原始模型1</param>
    /// <param name="gdd">通用数据结构</param>
    /// <param name="NodeSubSol_FilePath">相对Application.dataPath的文件路径</param>
    /// <param name="indexer">选择显示的序号</param>
    /// <param name="isVerse">默认为true，用于设置面片的外法线</param>
    /// <param name="Scale">模型数据缩放倍数</param>
    public static void Sub_Dynamic_ShowColor(GameObject obj, GameObject obj_1, GeneralDataOld gdd, string NodeSubSol_FilePath, int indexer, bool isVerse, float Scale)
    {
        if (!System.IO.File.Exists(Application.dataPath + NodeSubSol_FilePath))
        {
            Debug.Log("子步文件 " + Application.dataPath + NodeSubSol_FilePath + " 不存在！");
            return;
        }
        gdd = ReadData.CFD_Post_Dynamic_Data_ReadNodeStreSol(gdd, Application.dataPath + NodeSubSol_FilePath, "NODE");
        ShowColors(obj, obj_1, gdd, indexer, gdd.nSlnMaxValue[indexer], gdd.nSlnMinValue[indexer], isVerse, Scale);
    }



    /// <summary>
    /// 创建显示物理量标尺的显示
    /// 1.根据最大值和最小值得差值均分成七个区间
    /// 2.物理量精确到小数点后面三位
    /// 3.颜色渐变规则为HSV(0，1,1)到HSV(2/3,1,1)(直接通过贴图实现)
    /// </summary>
    /// <param name="Objname">标尺的名称.</param>
    /// <param name="Typename">物理量名称.</param>
    /// <param name="max">最大值.</param>
    /// <param name="min">最小值.</param>
    public static void CreatePrefabStaff(string Objname, string Typename, double max, double min, Vector3 pos)
    {
        GameObject Biaochi = (GameObject)Resources.Load("Flt_Assets/Staff_8");
        GameObject Target = GameObject.Find(Objname);
        if (Target == null)
        {
            Target = GameObject.Instantiate(Biaochi);
            Target.name = Objname;
            Target.transform.parent = GameObject.Find("Canvas").transform;
            Target.transform.localPosition = pos;
        }
        GameObject tf0 = GameObject.Find("Text (0)");
        if (tf0.transform.parent == Target.transform)
        {
            tf0.GetComponent<Text>().text = Typename;
        }

        //计算标值
        double step = (max - min) / 7;
        string[] values = new string[8];
        for (int i = 6; i >= 0; i--)
        {
            values[i] = (max - step * i).ToString("0.000");
            int j = i + 1;
            GameObject tf = GameObject.Find("Text (" + j + ")");
            if (tf.transform.parent == Target.transform)
            {
                tf.GetComponent<Text>().text = values[i];
            }
        }
        values[0] = min.ToString("0.000");
        GameObject tf8 = GameObject.Find("Text (8)");
        if (tf8.transform.parent == Target.transform)
        {
            tf8.GetComponent<Text>().text = values[0];
        }
    }


    /// <summary>
    /// 绘制空间流体矢量箭头,_idx存放物理量序号，分别为矢量的模、x分量、y分量、z分量
    /// </summary>
    /// <param name="_gd">标准格式协议</param>
    /// <param name="idx">矢量箭头x方向</param>
    /// <param name="idy">矢量箭头y方向</param>
    /// <param name="idz">矢量箭头z方向</param>
    public static void Debug_DrawArrow(GeneralDataOld _gd, int[] _idx, int t)
    {
        //for (int i = 0; i < 10; ++i)
        //{
        //    Debug.DrawLine(new Vector3(Random.Range(-10, 10), Random.Range(-20, 20), 0), new Vector3(Random.Range(-15, 30), Random.Range(-30, 100), 3), Color.red, 1000);
        //}

        //Vector3 pos1 = new Vector3(-18.2f, 12.9f, 256.5f);
        //Vector3 direction1 = new Vector3(0.3f, 0, 0);

        //Debug.DrawLine(pos1, pos1 + direction1, Color.red, 100);
        //Vector3 left1 = Quaternion.LookRotation(direction1) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
        //Vector3 right1 = Quaternion.LookRotation(direction1) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
        //Debug.DrawLine(pos1 + direction1, pos1 + direction1 + left1 * 0.45f * direction1.magnitude, Color.red, 100);
        //Debug.DrawLine(pos1 + direction1, pos1 + direction1 + right1 * 0.45f * direction1.magnitude, Color.red, 100);
        if (_gd != null)
        {
            int count = 0;//,
                          //t = 10;     //  持续时间
            float deno = (float)(_gd.nSlnMaxValue[_idx[0]] - _gd.nSlnMinValue[_idx[0]]);
            float vect_scale = 0.45f;

            for (int i = 0; i < _gd.NodeNumber; ++i)
            {
                if (_gd.nSlnTypeValue[_idx[0]][i] == 0)
                    continue;
                if (Random.Range(0f, 1f) > 1)
                    continue;
                Vector3 pos = new Vector3((float)_gd.NodeCoord[i][0], (float)_gd.NodeCoord[i][2], (float)_gd.NodeCoord[i][1]);      // 矢量点位置
                Vector3 direction = new Vector3((float)_gd.nSlnTypeValue[_idx[1]][i], (float)_gd.nSlnTypeValue[_idx[3]][i], (float)_gd.nSlnTypeValue[_idx[2]][i]);      //方向及大小
                float col_value = (float)(_gd.nSlnMaxValue[_idx[0]] - direction.magnitude) / deno;
                //float col_value = (float)(direction.magnitude - _gd.nSlnMinValue[_idx[0]]) / deno;
                Debug.DrawLine(pos, pos + direction * vect_scale, Color.HSVToRGB(col_value * 2 / 3, 1, 1), t);
                //Vector3 direction = new Vector3(0.1f, 0.1f, 0.1f);

                //Debug.Log(pos);
                //Debug.Log(direction);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
                Debug.DrawLine(pos + direction * vect_scale, pos + (direction + left * direction.magnitude * 0.45f) * vect_scale, Color.HSVToRGB(col_value * 2 / 3, 1, 1), t);
                Debug.DrawLine(pos + direction * vect_scale, pos + (direction + right * direction.magnitude * 0.45f) * vect_scale, Color.HSVToRGB(col_value * 2 / 3, 1, 1), t);
                ++count;
            }
            Debug.Log("速度最大值: " + _gd.nSlnMaxValue[_idx[0]]);
            Debug.Log("速度最小值: " + _gd.nSlnMinValue[_idx[0]]);
            Debug.Log("总计绘制箭头数量: " + count);
        }
        else
        {
            Debug.Log("所选绘制箭头的数据结构为空");
        }

    }

    /// <summary>
    /// 绘制空间流体矢量箭头,_idx存放物理量序号，分别为矢量的模、x分量、y分量、z分量,region为选定区域
    /// </summary>
    /// <param name="_gd">标准格式协议</param>
    /// <param name="idx">矢量箭头x方向</param>
    /// <param name="idy">矢量箭头y方向</param>
    /// <param name="idz">矢量箭头z方向</param>
    public static void Debug_DrawArrow(GeneralDataOld _gd, int[] _idx, float[] _region, int t)
    {
        int count = 0;
        //t = 10;     //  持续时间
        float deno = (float)(_gd.nSlnMaxValue[_idx[0]] - _gd.nSlnMinValue[_idx[0]]);
        float vect_scale = 0.45f;

        for (int i = 0; i < _gd.NodeNumber; ++i)
        {
            if (_gd.nSlnTypeValue[_idx[0]][i] == 0)
                continue;
            if (Random.Range(0f, 1f) > 0.25)
                continue;
            if ((_region[0] < _gd.NodeCoord[i][0] && _gd.NodeCoord[i][0] < _region[1]) && (_region[2] < _gd.NodeCoord[i][1] && _gd.NodeCoord[i][1] < _region[3]) && (_region[4] < _gd.NodeCoord[i][2] && _gd.NodeCoord[i][2] < _region[5]))
            {
                Vector3 pos = new Vector3((float)_gd.NodeCoord[i][0], (float)_gd.NodeCoord[i][2], (float)_gd.NodeCoord[i][1]);      // 矢量点位置
                Vector3 direction = new Vector3((float)_gd.nSlnTypeValue[_idx[1]][i], (float)_gd.nSlnTypeValue[_idx[3]][i], (float)_gd.nSlnTypeValue[_idx[2]][i]);      //方向及大小
                float col_value = (float)(_gd.nSlnMaxValue[_idx[0]] - direction.magnitude) / deno;
                //(direction.magnitude - _gd.nSlnMinValue[_idx[0]]) / deno;
                Debug.DrawLine(pos, pos + direction * vect_scale, Color.HSVToRGB(col_value * 2 / 3, 1, 1), t);
                //Vector3 direction = new Vector3(0.1f, 0.1f, 0.1f);

                //Debug.Log(pos);
                //Debug.Log(direction);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
                Debug.DrawLine(pos + direction * vect_scale, pos + (direction + left * direction.magnitude * 0.45f) * vect_scale, Color.HSVToRGB(col_value * 2 / 3, 1, 1), t);
                Debug.DrawLine(pos + direction * vect_scale, pos + (direction + right * direction.magnitude * 0.45f) * vect_scale, Color.HSVToRGB(col_value * 2 / 3, 1, 1), t);
                ++count;
            }
        }
        Debug.Log("总计绘制箭头数量: " + count);

    }

    /// <summary>
    /// 使用GL库绘制矢量箭头，只能在OnPostRender()方法中调用
    /// </summary>
    /// <param name="mat">GL材质</param>
    public static void GL_DrawArrow(Material mat)
    {
        if (!mat)
        {
            Debug.Log("请给GL绘制的箭头赋材质");
            return;
        }
        mat.SetPass(0);
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 2, 1);
        GL.End();
        GL.PopMatrix();
        //Debug.Log("GL end");
    }

    public static void LR_DrawArrow(GeneralDataOld _gd, int[] _idx, GameObject obj, GameObject obj_child)
    {
        if (_gd == null)
        {
            Debug.Log("通用数据结构中无数据，请检查");
            return;
        }
        if (obj == null)
        {
            Debug.Log("物体未赋值,请检查");
            return;
        }
        else
        {
            //删除父物体所有子物体
            int cot = obj.transform.childCount;
            if (cot != 0)
            {
                for (int i = 0; i < cot; ++i)
                {
                    GameObject ooo = obj.transform.GetChild(i).gameObject;
                    Object.Destroy(ooo);
                }
                Debug.Log("Destroy Child Object");
            }
        }

        float vect_scale = 0.45f;
        float deno = (float)(_gd.nSlnMaxValue[_idx[0]] - _gd.nSlnMinValue[_idx[0]]);


        for (int i = 0; i < _gd.NodeNumber; ++i)
        {
            if (_gd.nSlnTypeValue[_idx[0]][i] == 0)
                continue;
            //if (Random.Range(0f, 1f) > 0.5)
            //    continue;

            Vector3 pos = new Vector3((float)_gd.NodeCoord[i][0], (float)_gd.NodeCoord[i][2], (float)_gd.NodeCoord[i][1]);      // 矢量点位置
            Vector3 direction = new Vector3((float)_gd.nSlnTypeValue[_idx[1]][i], (float)_gd.nSlnTypeValue[_idx[3]][i], (float)_gd.nSlnTypeValue[_idx[2]][i]);      //方向及大小
            float col_value = (float)(_gd.nSlnMaxValue[_idx[0]] - direction.magnitude) / deno;
            //float col_value = (float)(_gd.nSlnMinValue[_idx[0]] - direction.magnitude) / deno;

            GameObject _obj = GameObject.Instantiate(obj_child);
            _obj.transform.parent = obj.transform;
            LineRenderer _lineRenderer;
            if (!_obj.GetComponent<LineRenderer>())
            {
                _lineRenderer = _obj.AddComponent<LineRenderer>();
            }
            else
            {
                _lineRenderer = _obj.GetComponent<LineRenderer>();
            }

            //_lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            //_lineRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
            //_lineRenderer.material = new Material(Shader.Find("Standard"));
            _lineRenderer.material = new Material(Shader.Find("Custom/test"));


            _lineRenderer.material.SetColor("_Color", Color.HSVToRGB(col_value * 2 / 3, 1, 1));
            //_lineRenderer.material.EnableKeyword("_EMISSION");
            _lineRenderer.material.SetColor("_EmissionColor", Color.HSVToRGB(col_value * 2 / 3, 1, 1));     //为了更改standard shader里的EmissionColor，首先要启用关键字_EMISSION，控制自发光
            //_lineRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
            //_lineRenderer.sharedMaterial.SetColor("_Color", Color.HSVToRGB(col_value * 2 / 3, 1, 1));

            _lineRenderer.startColor = Color.HSVToRGB(col_value * 2 / 3, 1, 1);
            _lineRenderer.endColor = Color.HSVToRGB(col_value * 2 / 3, 1, 1);

            _lineRenderer.numPositions = 5;

            _lineRenderer.startWidth = 0.012f * direction.magnitude;
            _lineRenderer.endWidth = 0.012f * direction.magnitude;

            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);


            Vector3[] points = new Vector3[5] { pos, pos + direction * vect_scale, pos + (direction + left * direction.magnitude * 0.45f) * vect_scale, pos + direction * vect_scale, pos + (direction + right * direction.magnitude * 0.45f) * vect_scale };

            _lineRenderer.SetPositions(points);
        }
        Debug.Log("生成箭头数量: " + obj.transform.childCount);

    }

    /// <summary>
    /// 清空LineRenderer箭头
    /// </summary>
    /// <param name="obj">箭头物体</param>
    public static void Empyt_LRArrow(GameObject obj)
    {
        //删除父物体所有子物体
        int cot = obj.transform.childCount;
        if (cot != 0)
        {
            for (int i = 0; i < cot; ++i)
            {
                GameObject ooo = obj.transform.GetChild(i).gameObject;
                GameObject.Destroy(ooo);
            }
            Debug.Log("Destroy Child Object");
        }
    }


}
