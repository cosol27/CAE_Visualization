using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PostProcess
{
    public static void TransferFile_ToDisk(GeneralDataOld gd)
    {
        try
        {
            string SavePath = @"D:/Zhou/Desktop/Flu_CAE";
            FileStream fs = new FileStream(SavePath + @"/Log.csv", FileMode.Create);
            //UnityEngine.Application.dataPath + @"/File";
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("[Name]");
            sw.WriteLine(gd.ModelName);
            sw.WriteLine("[Data]");
            sw.Write("[Node Nmuber], X[m], Y[m], Z[m],");
            for (int j = 0; j < gd.nSlnTypeNames.Count; ++j)
            {
                if (j < gd.nSlnTypeNames.Count - 1)
                    sw.Write(gd.nSlnTypeNames[j] + ", ");
                else
                    sw.WriteLine(gd.nSlnTypeNames[j]);
            }
            for (int i = 0; i < gd.NodeNumber; ++i)
            {
                sw.Write(i + ", ");
                for (int j = 0; j < 3; ++j)
                {
                    sw.Write(gd.NodeCoord[i][j] + ", ");     //坐标写入文件
                }
                for (int j = 0; j < gd.nSlnTypeNames.Count; ++j)        //写入节点数据
                {
                    if (j < gd.nSlnTypeNames.Count - 1)
                    {
                        sw.Write(gd.nSlnTypeValue[j][i] + ", ");
                    }
                    else
                        sw.WriteLine(gd.nSlnTypeValue[j][i]);
                }
            }
            if (gd.Elem_NodesIndex != null)
            {
                sw.WriteLine("[Face]");
                for (int i = 0; i < gd.ElemNumber; ++i)
                {
                    sw.Write(gd.Elem_NodesIndex[i][0] + ", ");
                    sw.Write(gd.Elem_NodesIndex[i][1] + ", ");
                    sw.WriteLine(gd.Elem_NodesIndex[i][2]);
                }
            }
            sw.Close();
        }
        catch(IOException e)
        {
            Debug.Log(e);
        }
        
    }
}
