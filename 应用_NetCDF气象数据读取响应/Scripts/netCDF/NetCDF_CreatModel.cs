using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 气象模型的拆分计算
/// 模拟以34层片体组成
/// </summary>

public class NetCDF_CreatModel {

    public GameObject father;//父物体
	public Material floorMat;//材质球

    /// <summary>
    /// 构建模型
    /// </summary>
    /// <param name="points"></param>
    /// <param name="values">可以为空</param>
    /// <param name="max">大于min</param>
    /// <param name="min"></param>
    public void CreatVisualObjects(List<float[]> points, float[] values, float max, float min){

        List<Mesh[]> meshes;

        NetCDF_Handler ncdf = NetCDF_Handler.GetInstance();

        if (ncdf.newGddContents.ObjectMeshes.Count > 0)
        {
            meshes = ncdf.newGddContents.ObjectMeshes;
        }
        else {
            meshes = CreatMesh(points);
        }

        List<Color[][]> colors = CreatColors(values, max, min);

        for (int i = 0; i < meshes.Count; i++) {

            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            obj.transform.parent = father.transform;
            obj.name = "MeshObject_" + i;

            int leng = meshes[i].Length;
            for (int j = 0; j < leng; j++)
            {
                GameObject fool = GameObject.CreatePrimitive(PrimitiveType.Plane);
                fool.transform.parent = obj.transform;
                fool.name = "floor_" + j;

                fool.GetComponent<MeshFilter>().mesh = meshes[i][j];
                fool.GetComponent<MeshFilter>().mesh.colors = colors[i][j];
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }
    }
    /// <summary>
    /// 赋予已构建的模型的顶点色
    /// </summary>
    /// <param name="values"></param>
    /// <param name="max"></param>
    /// <param name="min"></param>
    public void CreatVisualObjects(float[] values, float max, float min)
    {

        List<Mesh[]> meshes;

        NetCDF_Handler ncdf = NetCDF_Handler.GetInstance();

        if (ncdf.newGddContents.ObjectMeshes.Count > 0)
        {
            meshes = ncdf.newGddContents.ObjectMeshes;
        }
        else
        {
            Debug.Log("meshes has not built!!\n");
            return;
        }

        List<Color[][]> colors = CreatColors(values, max, min);

        for (int i = 0; i < meshes.Count; i++)
        {

            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            obj.transform.parent = father.transform;
            obj.name = "MeshObject_" + i;

            int leng = meshes[i].Length;
            for (int j = 0; j < leng; j++)
            {
                GameObject fool = GameObject.CreatePrimitive(PrimitiveType.Plane);
                fool.transform.parent = obj.transform;
                fool.name = "floor_" + j;

                fool.GetComponent<MeshFilter>().mesh = meshes[i][j];
                fool.GetComponent<MeshFilter>().mesh.colors = colors[i][j];
                fool.GetComponent<MeshRenderer>().material = floorMat;
            }
        }
    }




    /// <summary>
    /// 构建模型，渲染模型
    /// 函数内的gameobject关系，请自定义
    /// </summary>
    /// <param name="result">顶点列表</param>
    /// <param name="values">顶点值列表（转化为顶点颜色）,可以为空</param>
    /// <param name="maxVal">顶点值最大值</param>
    /// <param name="minVal">顶点值最小值</param>
    public List<Mesh[]> CreatMesh(List<float[]> points)
    {
        Debug.Log("create" + points.Count);
        
        int len = 420 * 450;
        int count = points.Count / len;

        List<List<float[]>> VoltexPoints = new List<List<float[]>>();

        for (int i = 0; i < count; i++)
        {
            List<float[]> temp = new List<float[]>();
            for (int j = i * len; j < (i + 1) * len; j++)
            {
                temp.Add(points[j]);
            }
            VoltexPoints.Add(temp);
        }

        List<Mesh[]> meshes = new List<Mesh[]>();

        for (int p = 0; p < VoltexPoints.Count; p++)
        {
            Mesh[] newMesh = SetNewMesh(VoltexPoints[p]);
            meshes.Add(newMesh);
        }
        return meshes;
    }

    Mesh[] SetNewMesh(List<float[]> vertices)
    {

		int count = 21;
		int VerCount = 21;
		Mesh[] myMesh = new Mesh[count];
		for (int k = 0; k < count; k++) 
		{
			myMesh [k] = new Mesh ();
			List<Vector3> newVecs = new List<Vector3> ();
			for (int i = 0; i < VerCount; i++) 
			{
				int dataindex = k * VerCount + i;
				dataindex = dataindex - k;			
				if (dataindex > 419)
					break;
				for (int j = 0; j < 450; j++) {
                    newVecs.Add(new Vector3(
                        (float)vertices[dataindex * 450 + j][0], 
                        (float)vertices[dataindex * 450 + j][1], 
                        (float)vertices[dataindex * 450 + j][2]));
				}
			}
            //顶点排序
			List<int> triagles = new List<int> ();
			for (int i = 0; i < VerCount - 1; i++) {

				if (i + k * VerCount - k > 418)
					break;

				for (int j = 0; j < 449; j++) {
					triagles.Add (i * 450 + j + 0);
					triagles.Add (i * 450 + j + 1);
					triagles.Add (i * 450 + j + 1 + 450);
					triagles.Add (i * 450 + j + 1 + 450);
					triagles.Add (i * 450 + j + 0 + 450);
					triagles.Add (i * 450 + j + 0);
				}
			}
            myMesh[k].vertices = newVecs.ToArray();
			myMesh[k].triangles = triagles.ToArray();
			myMesh[k].RecalculateNormals ();
			myMesh[k].RecalculateBounds ();
		}

		return myMesh;
	}


    public List<Color[][]> CreatColors(float[] values, float maxVal, float minVal)
    {
        Debug.Log("create" + values.Length);

        int len = 420 * 450;
        int count = values.Length / len;

        List<List<float>> VoltexPoints = new List<List<float>>();

        for (int i = 0; i < count; i++)
        {
            List<float> temp = new List<float>();
            for (int j = i * len; j < (i + 1) * len; j++)
            {
                temp.Add(values[j]);
            }
            VoltexPoints.Add(temp);
        }

        List<Color[][]> colors = new List<Color[][]>();

        for (int p = 0; p < VoltexPoints.Count; p++)
        {
            Color[][] newMesh = SetMeshColor(VoltexPoints[p], maxVal, minVal);
            colors.Add(newMesh);
        }
        return colors;
    }
    Color[][] SetMeshColor(List<float> values, float maxVal, float minVal)
    {
        int count = 21;
        int VerCount = 21;
        float Vals = maxVal - minVal;
        Color[][] myColors = new Color[count][];
        for (int k = 0; k < count; k++)
        {
            List<Vector3> newVecs = new List<Vector3>();
            List<Color> newCols = new List<Color>();
            for (int i = 0; i < VerCount; i++)
            {
                int dataindex = k * VerCount + i;
                dataindex = dataindex - k;
                if (dataindex > 419)
                    break;
                for (int j = 0; j < 450; j++)
                {
                    newCols.Add(Color.HSVToRGB(0.6f * (values[dataindex * 450 + j] - minVal) / Vals, 1, 1));
                }
                
            }
            myColors[k] = newCols.ToArray();
        }
        return myColors;
    }

}
