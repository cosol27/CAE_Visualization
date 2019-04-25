using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CAE_OnGUI : MonoBehaviour {

    public GameObject obj;
    public GameObject obj_1;
    public GameObject obj_2;
    public GameObject obj_3;
    public GameObject obj_line;
    public GameObject obj_linechild;

    public GameObject maincamere, aidcamera;

    public Button transfer_button;      //转换数据文件按钮


    string idx = "1",
           z_hori = "1.5";
    string show;
    private GeneralDataOld gdd, selected_gd, dy_gd;
    private int indexer;

    // Debug_DrawArrow 变量
    int[] arrow_index = new int[4] { 1, 2, 3, 4 };        //传入物理量序号到数组
    float[] rect_region = new float[6] { -15, -5, 220, 225, 12, 15 };

    //GL_DrawArrow 变量
    public Material mat;

    // ExtrmeValue 变量
    //float[] ExValues = new float[6];

    // Use this for initialization
    void Start () {
        //transfer_button.onClick.AddListener(delegate ()
        //{
        //    Debug.Log("Button down!");
        //});
        obj.GetComponent<MeshRenderer>().enabled = false;
        obj_1.GetComponent<MeshRenderer>().enabled = false;
        obj_2.GetComponent<MeshRenderer>().enabled = false;
        obj_3.GetComponent<MeshRenderer>().enabled = false;

        obj.GetComponent<MeshRenderer>().enabled = true;
        obj_1.GetComponent<MeshRenderer>().enabled = true;

        obj.GetComponent<MeshCollider>().enabled = true;
        obj_1.GetComponent<MeshCollider>().enabled = true;

        maincamere.SetActive(true);
        aidcamera.SetActive(false);

        mat = new Material(Shader.Find("Particles/Additive"));
        //CreateLineMaterial();

        //float vect_scale = 0.45f;

        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        //lineRenderer.startColor = Color.red;
        //lineRenderer.endColor = Color.red;

        //lineRenderer.numPositions = 5;

        //Vector3 pos = new Vector3(0, 0, 0);
        //Vector3 direction = new Vector3(5.4f, 1.9f, 2.5f);

        //lineRenderer.startWidth = 0.005f * direction.magnitude;
        //lineRenderer.endWidth = 0.005f * direction.magnitude;

        //Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
        //Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);


        //Vector3[] points = new Vector3[5] { pos, pos + direction * vect_scale, pos + (direction + left * direction.magnitude * 0.45f) * vect_scale, pos + direction * vect_scale, pos + (direction + right * direction.magnitude * 0.45f) * vect_scale };

        //lineRenderer.SetPositions(points);
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0) && gdd != null)
        {
            show = Data_Manipulation.Return_Contour_Click_Values(gdd, int.Parse(idx));
        }
        if (Input.GetMouseButtonDown(1))
        {


        }
        if (Input.GetKey("space"))          //切换相机
        {
            if (maincamere.activeInHierarchy == true)
            {
                maincamere.SetActive(false);
                aidcamera.SetActive(true);
            }
            else
            {
                aidcamera.SetActive(false);
                maincamere.SetActive(true);
            }
            
        }

    }

    //private void OnPostRender()
    //{
    //    Click_Return.GL_DrawArrow(mat);
    //}

    private void OnGUI()
    {
        GUI.Label(new Rect(50, 50, 100, 30), "选择读取物理量");    //GUI显示
        idx = GUI.TextField(new Rect(150, 50, 40, 20), idx);

        if (GUI.Button(new Rect(50, 100, 80, 50), "读取数据"))  //按下“读取数据”按钮
        {
            //string st_path = UnityEngine.Application.dataPath + @"/Resources/building_v1.0/Exterior-shanqiu-Pressure.csv";
            //string st_path1 = UnityEngine.Application.dataPath + @"/Resources/building_v1.0/Exterior-jianzhu-Pressure.csv";
            //string st_path2 = UnityEngine.Application.dataPath + @"/Resources/building_v2.0/Exterior-wall top-PressureVelocity.csv";
            string st_path3 = UnityEngine.Application.dataPath + @"/Resources/building_v3.0/Exterior-fluent5-4-fluid2-PressureVelocity.csv";
            Debug.Log("正在读取数据……");

            indexer = int.Parse(idx);
            gdd = ReadData.CFD_Post_Static_Data(st_path3);

            Debug.Log("物理量序号:" + indexer);
            Debug.Log("TypeNumber:" + gdd.nSlnTypeNames.Count);
            foreach (string s in gdd.nSlnTypeNames)
                Debug.Log(s);
            Debug.Log("NodeNumber:" + gdd.NodeNumber);
            Debug.Log("ElemNumber:" + gdd.ElemNumber);

            //ExValues = Click_Return.ExtrmeValue(gdd);
            //Debug.Log("X_Min: " + ExValues[0]);
            Debug.Log("confirm X_Min: " + gdd.Coor_Xmin);
            //Debug.Log("X_Max: " + ExValues[1]);
            Debug.Log("confirm X_Max: " + gdd.Coor_Xmax);
            //Debug.Log("Y_Min: " + ExValues[2]);
            Debug.Log("confirm Y_Min: " + gdd.Coor_Ymin);
            //Debug.Log("Y_Max: " + ExValues[3]);
            Debug.Log("confirm Y_Max: " + gdd.Coor_Ymax);
            //Debug.Log("Z_Min: " + ExValues[4]);
            Debug.Log("confirm Z_Min: " + gdd.Coor_Zmin);
            //Debug.Log("Z_Max: " + ExValues[5]);
            Debug.Log("confirm Z_Max: " + gdd.Coor_Zmax);

            Debug.Log("绘制");
            //Debug.Log(arrow_index[0]);
            //Debug.Log(new Vector3(0, 3, 0) + new Vector3(1, 1.5f, 2) * 3);

            //Click_Return.Debug_DrawArrow(gdd, arrow_index, 10);

        }

        if (GUI.Button(new Rect(50, 180, 80, 50), "精简数据"))
        {
            gdd = Data_Manipulation.Reduce_gd(gdd, "ratio", 0.5f);
            if (gdd != null)
            {
                Contours.Debug_DrawArrow(gdd, arrow_index, 10);      //精简后的数据进行矢量显示
            }
            else
            {
                Debug.Log("简化的传入数据为空");
            }

        }

        if (GUI.Button(new Rect(50, 260, 130, 50), "Debug_DrawArrow"))
        {
            Contours.Debug_DrawArrow(gdd, arrow_index, 10);
            //Click_Return.Debug_DrawArrow(gdd, arrow_index, rect_region, 10);
        }

        if (GUI.Button(new Rect(150, 180, 100, 50), "LineRender箭头"))
        {

            Contours.LR_DrawArrow(gdd, arrow_index, obj_line, obj_linechild);
        }

        if (GUI.Button(new Rect(260, 180, 80, 50), "清空箭头"))
        {

            Contours.Empyt_LRArrow(obj_line);
        }


        if (GUI.Button(new Rect(150, 100, 80, 50), "绘制云图"))
        {
            if (indexer >= gdd.nSlnTypeNames.Count)
                Debug.Log("错误： 物理量序号超出范围，请确认！");
            else if (gdd.ElemNumber == 0)
                Debug.Log("数据中无单元数据!");
            else
            {
                //Flu_Contours.ShowColors(obj_1, gdd, indexer, gdd.nSlnMaxValue[indexer], gdd.nSlnMinValue[indexer], true, 2);
                Contours.ShowColors(obj, obj_1, gdd, indexer, gdd.nSlnMaxValue[indexer], gdd.nSlnMinValue[indexer], true, 1);
            }
        }

        if (GUI.Button(new Rect(250, 100, 100, 50), "Transfer2File"))
        {
            PostProcess.TransferFile_ToDisk(gdd);
            Debug.Log("Button down, Transfer2File!");
        }

        GUI.Label(new Rect(500, 50, 100, 30), "选择Z平面高度");
        z_hori = GUI.TextField(new Rect(590, 50, 40, 20), z_hori);
        if (GUI.Button(new Rect(640, 40, 60, 50), "选择"))
        {
            selected_gd = Data_Manipulation.Select_Horizon_gd(gdd, double.Parse(z_hori));
            Debug.Log("符合条件的数据个数:"+selected_gd.NodeCoord.Count);
        }

        if (show != null)
        {
            GUI.Label(new Rect(500, 180, 160, 50), show);
        }

        if (GUI.Button(new Rect(500, 100, 80, 50), "动态显示"))
        {
            string dy_path = Application.dataPath + @"/Resources/yalirong";
            indexer = 0;

            if (Directory.Exists(dy_path))          //判断文件夹路径是否存在
            {
                dy_gd = new GeneralDataOld();
                dy_gd = ReadData.CFD_Post_Dynamic_Data_CoorAndIndex(dy_path, "NODE", "ELEM");

                StopAllCoroutines();
                StartCoroutine("StreSolution");
            }
            else
            {
                Debug.Log("Directory Doesn't Exist!");
            }
        }

        if (GUI.Button(new Rect(600, 100, 80, 50), "批量读取"))
        {
            //System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();
            //Stopwatch.Start();
            //int s_file_count = 0;
            indexer = 0;
            string path = Application.dataPath + @"/Resources/yalirong";
            Dynamic_GeneralData d_gd = new Dynamic_GeneralData();

            FileStream fs = new FileStream(Application.dataPath + @"/Resources/yalirong/stre_list.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            if (Directory.Exists(path))
            {
                GeneralDataOld tmp_gd = ReadData.CFD_Post_Dynamic_Data_CoorAndIndex(path, "NODE", "ELEM");        // 寻找根目录下节点坐标、单元序列配置文件

                d_gd = Data_Manipulation.Trans_gd_To_dgd(tmp_gd);       //将静态通用数据结构转换为动态通用数据结构
                d_gd.nSlnTypeNames = new List<string> { "Node Solution" };
                // 清空坐标、单元序列数据
                //tmp_gd.NodeCoord.Clear();
                //tmp_gd.Elem_NodesIndex.Clear();

                DirectoryInfo root = new DirectoryInfo(path);
                foreach (DirectoryInfo DI in root.GetDirectories())     //寻找根目录下文件夹
                {
                    //Debug.Log("DirectoryInfo: " + DI);
                    if (DI.GetFiles() != null)                         //根目录文件夹下文件
                    {
                        foreach (FileInfo d in DI.GetFiles())
                        {
                            if (!d.ToString().Contains("meta") && d.ToString().Contains("-s-"))     // 除去unity meta 文件
                            {
                                Debug.Log("读取 File: " + d);
                                tmp_gd = ReadData.CFD_Post_Dynamic_Data_ReadNodeStreSol(tmp_gd, d.FullName, "NODE");
                                d_gd.TimeSteps.Add(tmp_gd.Time);
                                d_gd.nSlnTypeValues.Add(tmp_gd.nSlnTypeValue);
                                d_gd.nSlnMaxValues.Add(tmp_gd.nSlnMaxValue);
                                d_gd.nSlnMinValues.Add(tmp_gd.nSlnMinValue);

                            }
                        }
                           
                    }

                }
                int count = d_gd.nSlnTypeValues.Count;
                Debug.Log("总数: " + count + " " + d_gd.TimeSteps.Count);
                

                //读取时间步
                d_gd.TimeSteps = ReadData.CFD_Post_Dynamic_Data_TimeStep(path + @"/SET.lis", count, "SET");

                GeneralDataOld interpolated_gd = Data_Manipulation.Linear_Interpolate_gd(d_gd, 10.95f);
                

                Contours.ShowColors(obj, obj_1, interpolated_gd, indexer, interpolated_gd.nSlnMaxValue[indexer], interpolated_gd.nSlnMinValue[indexer], true, 1);

                Debug.Log("Showed");

                foreach (List<double>[] vl in d_gd.nSlnTypeValues)
                {
                    sw.Write(vl[0][17345] + " ");
                }
                sw.WriteLine();
                foreach (float t in d_gd.TimeSteps)
                {
                    sw.Write(t + " ");
                }


                //MemoryStream ms = new MemoryStream();
                //BinaryFormatter formatter = new BinaryFormatter();
                //formatter.Serialize(ms, d_gd);
                //Debug.Log("占用大小: " + ms.Length);

            }
            else
            {
                Debug.Log("Directory Doesn't Exist!");
            }
            //Stopwatch.Stop();
            //TimeSpan time = Stopwatch.Elapsed;
            //Debug.Log(time.TotalSeconds);
            sw.Close();

        }

    }


    IEnumerator StreSolution()
    {
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/1/nsol-s-1-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/1/nsol-s-1-3.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/1/nsol-s-1-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/2/nsol-s-2-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/2/nsol-s-2-3.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/2/nsol-s-2-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/3/nsol-s-3-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/3/nsol-s-3-3.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/3/nsol-s-3-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/4/nsol-s-4-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/4/nsol-s-4-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/4/nsol-s-4-9.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/4/nsol-s-4-12.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/5/nsol-s-5-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/5/nsol-s-5-6.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/5/nsol-s-5-10.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/5/nsol-s-5-14.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/6/nsol-s-6-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/6/nsol-s-6-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/6/nsol-s-6-10.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/7/nsol-s-7-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame(); 
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/7/nsol-s-7-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/7/nsol-s-7-9.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/7/nsol-s-7-13.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/8/nsol-s-8-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/8/nsol-s-8-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/8/nsol-s-8-9.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/8/nsol-s-8-13.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/9/nsol-s-9-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/9/nsol-s-9-6.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/9/nsol-s-9-11.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/9/nsol-s-9-17.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-6.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-11.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-16.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-21.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-26.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/10/nsol-s-10-31.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();

        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/11/nsol-s-11-1.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/11/nsol-s-11-5.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/11/nsol-s-11-9.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/11/nsol-s-11-13.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/11/nsol-s-11-17.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
        Contours.Sub_Dynamic_ShowColor(obj, obj_1, dy_gd, @"/Resources/yalirong/11/nsol-s-11-22.lis", indexer, true, 1);
        yield return new WaitForEndOfFrame();
    }
}
