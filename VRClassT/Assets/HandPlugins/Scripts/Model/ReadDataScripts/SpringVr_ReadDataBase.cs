using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System;

public class SpringVr_ReadDataBase : SpringVr_Base
{

    #region 声明变量
    [HideInInspector]
    public      Thread          m_tReadDataThread;
    [HideInInspector]
    public      float[]         aGroupByteData;
    [HideInInspector]
    public      bool            m_bolBeginReadData;
    [HideInInspector]
    public      Vector3[]       m_v3sAllJointsRot;
    [HideInInspector]
    public      string          m_strBthAddr;
    [HideInInspector]
    public      string          m_strFrameHeadle;
    [HideInInspector]
    public      string          m_strFrameEnd;
    protected   int             m_iDateLength;
    protected   int             m_iSerialPortFlag;
    private     List<byte[]>    m_lbAllWriteDataByte;
    private     string[]        m_strWriteDatas;



    #endregion

    #region  回调
    public delegate void del_GetBtnDown();                              /// <summary>单击按钮事件</summary>
    public event del_GetBtnDown eve_GetBtnDown;
    public delegate void del_PressBtn();                                /// <summary>长按按钮触发事件</summary>
    public event del_PressBtn eve_PressBtn;
    public delegate void del_GetBtnUp();                                /// <summary>按下按钮抬起事件</summary>
    public event del_GetBtnUp eve_GetBtnUp;
    public delegate void del_StaticCorrection();                        /// <summary>静态校准结束事件</summary>
    public event del_StaticCorrection eve_StaticCorrection;
    public delegate void del_DynamicCorrection();                       /// <summary>动态校准结束事件</summary>
    public event del_DynamicCorrection eve_DynamicCorrection;
    #endregion

    #region 函数

    #region 初始化

    protected override void OnAwake()
    {
        base.OnAwake();
        m_v3sAllJointsRot = new Vector3[17];
        m_strFrameEnd = "FD";
    }
    protected override void OnStart()
    {
        base.OnStart();
#if UNITY_STANDALONE_WIN
        SpringVr_SerialPortTool.Instance.AddPortMonitor(m_strFrameHeadle);
        SpringVr_SerialPortTool.Instance.Eve_ReadDataReady += GainSerialPortFlag;
#endif
        DataInit();
    }
    private void DataInit()
    {
        m_iDateLength = (int)SpringVr_SerialPortTool.Instance.numberOfSensors;
        m_lbAllWriteDataByte = new List<byte[]>();
        m_strWriteDatas = new string[2];
        m_strWriteDatas[0] = "FE8002000082FD";//静态校准命令
        m_strWriteDatas[1] = "FE8004000084FD";//动态校准命令
        for (int i = 0; i < m_strWriteDatas.Length; i++)
        {
            m_lbAllWriteDataByte.Add(new byte[7]);
            for (int j = 0; j < 7; j++)
            {
                m_lbAllWriteDataByte[i][j] = Convert.ToByte(m_strWriteDatas[i].Substring(j * 2, 2), 16);
            }
        }
    }
    #endregion

    #region 通信数据
#if UNITY_STANDALONE_WIN

    /// <summary>拿到相应串口的标志位</summary>
    private void GainSerialPortFlag(Dictionary<string, int> allPortSign)
    {
        foreach (KeyValuePair<string, int> item in allPortSign)
        {
            if (item.Value == 0) continue;
            if (item.Key == m_strFrameHeadle && !m_bolBeginReadData)
            {
                m_iSerialPortFlag = item.Value;
                m_tReadDataThread = new Thread(RegisterContentData);
                m_tReadDataThread.Start();
                m_bolBeginReadData = true;
            }
        }
    }
    /// <summary>开启读数据的多线程</summary>
    private void RegisterContentData(object arges)
    {
        while (m_iSerialPortFlag == 0)
        {
            Thread.Sleep(50);
        }
        SpringVr_SerialPortTool.Instance.ContentData(m_iSerialPortFlag, m_iDateLength, m_strFrameHeadle, m_strFrameEnd, this);
    }
#endif
    /// <summary>接收数据和特殊命令</summary>
    public void ContentData(float[] contentData, byte[] cmd)
    {
        aGroupByteData = contentData;
        if (cmd != null)
        {
            SpecialCommandDispose(cmd[0].ToString("X2"));
        }
    }
    /// <summary>接收数据和特殊命令</summary>
    public void ContentData(float[] contentData)
    {
        aGroupByteData = contentData;
    }
    /// <summary>写入数据方法</summary>
    public void WriteData(byte[] tempByte)
    {
#if UNITY_ANDROID
        if (!SpringVr_SerialPortTool.Instance.WriteData(m_strBthAddr, tempByte))
        {
            Debug.LogError("写入数据失败");
        }
#elif UNITY_STANDALONE_WIN
        switch (SpringVr_Summary.Instance.reciveDataWay)
        {
            case SpringVr_Summary.ReceiveDataWay.WindowsSerialPort:
                SpringVr_SerialPortTool.Instance.WriteData(m_iSerialPortFlag, tempByte, tempByte.Length);
                break;
            case SpringVr_Summary.ReceiveDataWay.WindowsBLE:
                SpringVr_SerialPortTool.Instance.WriteData(m_strBthAddr, tempByte);
                break;
            case SpringVr_Summary.ReceiveDataWay.AndroidBLE:
                Debug.LogError("[Spring-Vr]不允许在PC环境下写入Android命令，请修改数据连接方式");
                break;
            default:
                break;
        }
#endif
    }

    /// <summary>特殊命令处理</summary>
    private void SpecialCommandDispose(string specialCommand)
    {
        if (specialCommand != "")
        {
            switch (specialCommand)
            {
                case "02":
                    if (eve_GetBtnDown != null)
                    {
                        eve_GetBtnDown();
                    }
                    break;
                case "03":
                    if (eve_PressBtn != null)
                    {
                        eve_PressBtn();
                    }
                    break;
                case "04":
                    if (eve_GetBtnUp != null)
                    {
                        eve_GetBtnUp();
                    }
                    break;
                case "05":
                    if (eve_StaticCorrection != null)
                    {
                        eve_StaticCorrection();
                    }
                    break;
                case "06":
                    if (eve_DynamicCorrection != null)
                    {
                        eve_DynamicCorrection();
                    }
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region 校准方法
    /// <summary>静态校准</summary>
    public void StaticCalibration()
    {
        if (m_bolBeginReadData)
        {
            WriteData(m_lbAllWriteDataByte[0]);
        }
        else
        {
            Debug.LogError("[Spring-Vr]请检查是否正确连接");
        }
    }

    /// <summary>动态校准</summary>
    public void DynamicCalibration()
    {
        if (m_bolBeginReadData)
        {
            WriteData(m_lbAllWriteDataByte[1]);
        }
        else
        {
            Debug.LogError("[Spring-Vr]请检查是否正确连接");
        }
    }

    #endregion

    void Update()
    {
        GarbageCollection();
    }
    /// <summary>清理垃圾</summary>
    private void GarbageCollection()
    {
        if (Time.frameCount % 120 == 0) System.GC.Collect();
    }

    #endregion

}
