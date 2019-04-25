# CAE Post Visualization
  定制了新协议GeneralData，和旧协议GeneralDataOld的可视化后处理工具箱

## 文件说明 
  + **GeneralDataOld.cs**  
      + 新版协议。 **说明:** 基于新协议的可视化工具箱还需改动，ProtocolOld文件下的脚本现主要针对旧协议
  + ProtocolOld  
      + CAE_OnGUI.cs  
        + Unity GUI函数，一般仅用于调试 
      + CameraControl.cs  
        + 用于调试，控制Unity相机  
      + Contours.cs  
        + 可视化函数集合类，包含了多种可视化显示函数  
      + Data_Manipulation.cs  
        + 数据操作类，包含数据压缩、筛选等算法
      + GeneralDataOld.cs
        + 旧协议
      + PostProcess.cs  
        + 后处理，目前只包含数据写出
      + ReadData.cs  
        + 数据读取类  
  + README.md
