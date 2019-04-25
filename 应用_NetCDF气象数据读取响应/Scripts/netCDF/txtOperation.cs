using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;

public class txtOperation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static bool txtWriter(string txtpath, string[] txtcontents){
        try
        {
            // 写入文件的源路径及其写入流
            StreamWriter swWriteFile = File.CreateText(txtpath);

            // 读取流直至文件末尾结束，并逐行写入另一文件内
            int index = 0;
            while (index != txtcontents.Length)
            {
                string strReadLine = txtcontents[index]; //读取每行数据
                swWriteFile.WriteLine(strReadLine); //写入读取的每行数据
                index++;
            }
            swWriteFile.Close();
            return true;
        }
        catch {

            return false;
        }
        
    }

    public static List<string> txtReader(string txtpath)
    {
        try {
            List<string> contents = new List<string>();
            // 读取文件的源路径及其读取流
            StreamReader srReadFile = new StreamReader(txtpath);

            // 读取流直至文件末尾结束
            string strReadLine;
            while (!srReadFile.EndOfStream)
            {
                strReadLine = srReadFile.ReadLine(); //读取每行数据
                contents.Add(strReadLine);
            }
            // 关闭读取流文件
            srReadFile.Close();
            Debug.Log(System.DateTime.Now.Second);
            return contents;
        }
        catch { 
            return null;
        }
    }
}
