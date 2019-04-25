# ProtocolOld  
  基于协议开发的可视化工具箱
## 文件说明 
  + ProtocolOld  
      + CAE_OnGUI.cs  （Unity GUI函数）
        + *void Start ()*
        + *void Update ()*
        + *void OnGUI ()*
      + CameraControl.cs （用于调试，控制Unity相机）
        + *void LateUpdate ()*
      + Contours.cs  （可视化函数集合类，包含了多种可视化显示函数）
        + *void ShowColors* （有限元单双层显示模型）
        + *void Sub_Dynamic_ShowColor* （动态显示每一子步每一帧的表面标量云图）
        + *void CreatePrefabStaff*  （创建显示物理量标尺的显示）
        + *void Debug_DrawArrow*  （Debug方式绘制空间流体矢量箭头,_idx存放物理量序号，分别为矢量的模、x分量、y分量、z分量）
        + *void GL_DrawArrow* （使用GL库绘制矢量箭头，只能在OnPostRender()方法中调用）
        + *void LR_DrawArrow* （LineRender绘制箭头）
        + *void Empyt_LRArrow*  （清空LineRenderer箭头）
      + Data_Manipulation.cs  （数据操作类，包含数据压缩、筛选等算法）
        + *string Return_Contour_Click_Values*  （返回鼠标点击处的物理参量）
        + *float[] ExtrmeValue* （返回极值）
        + *GeneralDataOld FillMaxMin* （自动填充GenaralData里缺损的物理量最大最小值）
        + *GeneralDataOld Select_Horizon_gd*  （选择平面数据）
        + *GeneralDataOld Reduce_gd*  （数据精简）
        + *Dynamic_GeneralData Trans_gd_To_dgd* （转化为Dynamic_GeneralData）
        + *GeneralDataOld Linear_Interpolate_gd*  （插值gd）
      + GeneralDataOld.cs（旧协议）
      + PostProcess.cs  （后处理，目前只包含数据写出）
        + *void TransferFile_ToDisk*  （写出数据到磁盘）
      + ReadData.cs  （数据读取类）
        + *GeneralDataOld CFD_Post_Static_Data* （读取数据存储到 GeneralDataOld）
        + *GeneralDataOld CFD_Post_Dynamic_Data_CoorAndIndex* （从导出的动态数据中读取节点和单元组成顺序和结构，存储到通用数据结构中）
        + *GeneralDataOld CFD_Post_Dynamic_Data_ReadNodeCoor* （从传入的节点文件路径中读取节点坐标，并存储到通用数据结构中）
        + *GeneralDataOld CFD_Post_Dynamic_Data_ReadElemIndex*  （读取节点构造顺序，并存储到通用数据结构中）
        + *GeneralDataOld CFD_Post_Dynamic_Data_ReadNodeDisSol* （读取节点解，并赋值给通用数据）
        + *GeneralDataOld CFD_Post_Dynamic_Data_ReadNodeStreSol*  （读取节点解，并赋值给通用数据）
        + *List\<float> CFD_Post_Dynamic_Data_TimeStep* （读取时间步）
  + README.md
