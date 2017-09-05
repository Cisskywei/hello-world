/** 
 *Copyright(C) 2015 by SpringVr 
 *All rights reserved. 
 *FileName:     SpringVrReadDataBase.cs 
 *Author:       SpringVr_Gz 
 *Version:      0.4.0 
 *UnityVersion：5.4.0f3 
 *Date:         2016-11-26 
 *Description:    此脚本用来调用连接蓝牙、接收数据、断开蓝牙方法，并将接收到的数据存储起来
*/
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System;
using LitJson;

public class SpringVr_SerialPortTool : SpringVr_Base
{

    #region 枚举
    public enum NumberOfSensors
    {
        SixSensors = 53,
        SevenSensors = 61,
        EightSensors = 69,
        NineSensors = 77
    }
    #endregion

    #region Dll调用

    #region SerialPortDll调用
    [DllImport("ComExports")]
    public extern static void ComGetPorts([MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]byte[] ports, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] byte[] count);
    [DllImport("ComExports")]
    public extern static void ComGetRawData(int flag, [MarshalAs(UnmanagedType.LPArray, SizeConst = 100000)]byte[] ctx, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] int[] ctxLen);
    [DllImport("ComExports")]
    public extern static void ComStop();
    [DllImport("ComExports")]
    public extern static void ComWriteData(int port, [MarshalAs(UnmanagedType.LPArray, SizeConst = 100)]byte[] ctx, int ctxLen);
    #endregion

    #region BLE蓝牙Dll引用
    [DllImport("BleExports")]
    public static extern bool BleInit([MarshalAs(UnmanagedType.FunctionPtr)]BleCallBack bleStatus);
    [DllImport("BleExports")]
    public static extern int BleGetDeviceNum();
    [DllImport("BleExports")]
    public static extern void BleGetDeviceAddress([MarshalAs(UnmanagedType.LPArray, SizeConst = 10)]UInt64[] pAddr);
    [DllImport("BleExports")]
    public static extern void BleConnectDevice(UInt64 addr);
    [DllImport("BleExports")]
    public static extern void BleDisconnectDevice(UInt64 addr);
    [DllImport("BleExports")]
    public static extern bool BleGetDeviceData(UInt64 addr, [MarshalAs(UnmanagedType.LPArray, SizeConst = 68)]byte[] ctx, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)]int[] ctxLen);
    [DllImport("BleExports")]
    public static extern void BleDisconnectAll();
    [DllImport("BleExports")]
    public static extern bool BleWriteDataToDevice(UInt64 addr, byte[] ctx, int ctxLen);
    [DllImport("BleExports")]
    public static extern IntPtr BleGetName(UInt64 ullAddr);
    #endregion

    #endregion

    #region 成员变量
    [HideInInspector]
    public NumberOfSensors numberOfSensors;
    private List<SpringVr_ReadDataBase> m_lrAllReadDataScripts;
    private string m_strBthName;
    private string m_strBthAddr;
    private int m_iDataLength;

#if UNITY_STANDALONE_WIN
    private Dictionary<int, StringBuilder> m_dicAllPortsData;
    private Dictionary<string, int> m_dicAllPortSign;
    private Dictionary<UInt64, bool> m_dicConnStatus;
    private List<int> m_liAllPortsFlag;
    private List<int> m_liConfirmPortsFlag;
    private byte[] m_bsAllPorts;
    private byte[] m_bsAllPortsNum;
    private byte[] m_bsContnetData;
    private UInt64[] m_ui64AddrArray;
    private int[] m_bsContnetDataLength;
    private string m_strFrameEnd;
    private bool m_bolBeginContentData;
    private bool m_bolMappingSerialPort;
    private int m_iAllRegisterPortsNum;
    private BleCallBack connectStageCallback;
#elif UNITY_ANDROID
    private AndroidJavaObject jo;
    private StringBuilder m_strbLeftReceiveDataQueue;
    private StringBuilder m_strbRightReceiveDataQueue;
    private byte[] m_bsLeftCmd;
    private byte[] m_bsRightCmd;
    private byte[] m_bsLeftCombinationData;
    private byte[] m_bsRightCombinationData;
    private bool m_bolLeftBthCnt;
    private bool m_bolRightBthCnt;
#endif

    #endregion

    #region 回调

#if UNITY_STANDALONE_WIN
    public delegate void Del_ReadDataReady(Dictionary<string, int> allPortSign);
    public event Del_ReadDataReady Eve_ReadDataReady;
#endif

    #region 安卓事件回调
    public delegate void Del_ReceiveData(string address, string data);
    /// <summary>读取数据回调</summary>
    public Del_ReceiveData Eve_ReceiveData;
    public delegate void Del_DeviceFind(string address, string name);
    /// <summary>查找设备回调</summary>
    public Del_DeviceFind Eve_DeviceFind;
    public delegate void Del_ConnectionStateChange(string address, bool status);
    /// <summary>连接状态改变回调</summary>
    public Del_ConnectionStateChange Eve_ConnectionStateChange;
    public delegate void Del_SendDataToAndroid(string address, bool status, string hexData);
    /// <summary>连接状态改变回调</summary>
    public Del_SendDataToAndroid Eve_SendDataToAndroid;
    public delegate void BleCallBack(UInt64 ullAddr, bool bConnected);
    #endregion

    #endregion

    #region 函数

    #region 单例
    private static SpringVr_SerialPortTool instance;
    public static SpringVr_SerialPortTool Instance
    {
        get
        {
            return instance;
        }
    }

    #endregion

    #region 初始化
    void Awake()
    {
        instance = this;
        Application.runInBackground = true;
    }
    void Start()
    {
        numberOfSensors = SpringVr_Summary.Instance.numberOfSensors;
        m_iDataLength = (int)numberOfSensors;
#if UNITY_ANDROID
        AndroidInit();
#elif UNITY_STANDALONE_WIN
        WinDataInit();
        WindowsInit();
#endif
    }
    #endregion

    #region 数据接收模块
#if UNITY_ANDROID

    private void AndroidInit()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("checkAndEnableBluetoothEnable");
        jo.Call<bool>("initial");
        CheckAndEnableBth();
        //StartCoroutine()
        Invoke("WaiteForStarDiscovery", 2);
        m_strbLeftReceiveDataQueue = new StringBuilder();
        m_strbRightReceiveDataQueue = new StringBuilder();
        m_bsLeftCmd = new byte[1];
        m_bsRightCmd = new byte[1];
        m_bsRightCombinationData = new byte[m_iDataLength - 2];
        m_bsLeftCombinationData = new byte[m_iDataLength - 2];
    }
    private void WaiteForStarDiscovery()
    {
        StartDiscovery();
    }

    #region Unity调取Android方法
    /// <summary>唤起安卓原生打开蓝牙面板</summary>
    public void CheckAndEnableBth()
    {
        jo.Call("checkAndEnableBluetoothEnable");
    }
    /// <summary>停止搜索设备</summary>
    public void StopDiscovery()
    {
        jo.Call<bool>("startDiscovery", false);
    }
    /// <summary>搜索设备</summary>
    /// <returns>是否可以搜索设备</returns>
    public void StartDiscovery()
    {
        bool isReady = jo.Call<bool>("startDiscovery", true);
        if (isReady)
        {
            Debug.LogError("搜索设备");
        }
    }
    /// <summary>连接相应蓝牙并读取数据</summary>
    /// <param name="Address">将要连接的蓝牙地址</param>
    /// <returns>是否可以连接</returns>
    public bool ConnectBth(string Address)
    {
        bool isReady = jo.Call<bool>("connect", Address);
        return isReady;
    }
    /// <summary>给蓝牙写入数据</summary>
    /// <param name="address">蓝牙地址</param>
    /// <param name="dataMge">数据</param>
    /// <returns>是否可以写入数据</returns>
    public bool WriteData(string address, byte[] dataMge)
    {
        bool isReady = jo.Call<bool>("sendData", address, dataMge);
        return isReady;
    }
    /// <summary>断开相应的蓝牙连接</summary>
    /// <param name="address">断开连接的蓝牙地址</param>
    public void CloseTargetBth(string address)
    {
        jo.Call("close", address);
    }
    /// <summary>断开所有的蓝牙连接</summary>
    public void CloseAllBth()
    {
        jo.Call("closeAll");
        //SpringVrAllUseDataMgr.Instance.m_lsAllBthName.Clear();
    }


    #endregion

    #region Android调取Unity方法
    /// <summary>查找设备的回调</summary>
    public void AndroidDeviceFind(string content)
    {
        JsonData jd = JsonMapper.ToObject(content);
        //Debug.LogError((string)jd["Address"]);
        //Debug.LogError((string)jd["Name"]);
        //Debug.LogError((int)jd["Rssi"]);
        m_strBthName = (string)jd["Name"];
        m_strBthAddr = (string)jd["Address"];
        if (m_strBthName.StartsWith("MiiG"))
        {
            if (m_strBthName.Substring(8, 1) == "L")
            {
                if (!SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData)
                {
                    SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData = true;
                    if (ConnectBth(m_strBthAddr))
                    {
                        SpringVr_ReadLeftHandData.Instance.m_strBthAddr = m_strBthAddr;
                        m_bolLeftBthCnt = true;
                    }
                    else
                    {
                        SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData = false;
                    }
                }
            }
            else if (m_strBthName.Substring(8, 1) == "R")
            {
                if (!SpringVr_ReadRightHandData.Instance.m_bolBeginReadData)
                {
                    SpringVr_ReadRightHandData.Instance.m_bolBeginReadData = true;
                    if (ConnectBth(m_strBthAddr))
                    {
                        SpringVr_ReadRightHandData.Instance.m_strBthAddr = m_strBthAddr;
                        m_bolRightBthCnt = true;
                    }
                    else
                    {
                        SpringVr_ReadRightHandData.Instance.m_bolBeginReadData = false;
                    }
                }
            }
        }
        if (m_bolRightBthCnt && m_bolLeftBthCnt)
        {
            StopDiscovery();
            m_bolLeftBthCnt = false;
            m_bolRightBthCnt = false;
        }
    }
    /// <summary>发送数据给Unity的回调</summary>
    public void AndroidSendDataToUnity(string content)
    {
        try
        {
            JsonData jd = JsonMapper.ToObject(content);
            if ((string)jd["Address"] == SpringVr_ReadRightHandData.Instance.m_strBthAddr)
            {
                m_strbRightReceiveDataQueue.Append((string)jd["HexData"]);
                m_bsRightCombinationData = DataCombination(ref m_strbRightReceiveDataQueue, SpringVr_ReadRightHandData.Instance.m_strFrameHeadle, SpringVr_ReadRightHandData.Instance.m_strFrameEnd, m_iDataLength);
                if (m_bsRightCombinationData == null) return;
                m_bsRightCmd[0] = m_bsRightCombinationData[1];
                SpringVr_ReadRightHandData.Instance.ContentData(ByteToFloatArray(m_iDataLength, m_bsRightCombinationData), m_bsRightCmd);
            }
            else if ((string)jd["Address"] == SpringVr_ReadLeftHandData.Instance.m_strBthAddr)
            {
                m_strbLeftReceiveDataQueue.Append((string)jd["HexData"]);
                m_bsLeftCombinationData = DataCombination(ref m_strbLeftReceiveDataQueue, SpringVr_ReadLeftHandData.Instance.m_strFrameHeadle, SpringVr_ReadLeftHandData.Instance.m_strFrameEnd, m_iDataLength);
                if (m_bsLeftCombinationData == null) return;
                m_bsLeftCmd[0] = m_bsLeftCombinationData[1];
                SpringVr_ReadLeftHandData.Instance.ContentData(ByteToFloatArray(m_iDataLength, m_bsLeftCombinationData), m_bsLeftCmd);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }
    }
    /// <summary>连接状态改变的回调</summary>
    public void AndroidConnectionStateChange(string content)
    {
        JsonData jd = JsonMapper.ToObject(content);
        if ((bool)jd["Status"])
        {
            Debug.LogError("蓝牙已连接 " + (string)jd["Address"]);
        }
        else
        {
            Debug.LogError("蓝牙已断开 " + (string)jd["Address"]);
        }
        if ((bool)jd["Status"])
        {
            if ((string)jd["Address"] == SpringVr_ReadLeftHandData.Instance.m_strBthAddr)
            {
                SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData = true;
            }
            else if ((string)jd["Address"] == SpringVr_ReadRightHandData.Instance.m_strBthAddr)
            {
                SpringVr_ReadRightHandData.Instance.m_bolBeginReadData = true;
            }

        }
        else
        {
            if ((string)jd["Address"] == SpringVr_ReadLeftHandData.Instance.m_strBthAddr)
            {
                SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData = false;
            }
            else if ((string)jd["Address"] == SpringVr_ReadRightHandData.Instance.m_strBthAddr)
            {
                SpringVr_ReadRightHandData.Instance.m_bolBeginReadData = false;
            }
        }
    }
    /// <summary>Unity向蓝牙中写入数据的回调 </summary>
    public void UnitySendDataToAndroid(string content)
    {
        JsonData jd = JsonMapper.ToObject(content);
        if (Eve_SendDataToAndroid != null)
        {
            Eve_SendDataToAndroid((string)jd["Address"], (bool)jd["Status"], (string)jd["HexData"]);
        }
    }
    #endregion
    private void AndroidQuit(object param)
    {
        StopDiscovery();
        CloseAllBth();
    }
#elif UNITY_STANDALONE_WIN

    private void WinDataInit()
    {
        Debug.LogError("WinDataInit == ");
        m_dicAllPortsData = new Dictionary<int, StringBuilder>();
        m_lrAllReadDataScripts = new List<SpringVr_ReadDataBase>();
        if (m_dicAllPortSign == null)
        {
            m_dicAllPortSign = new Dictionary<string, int>();
            Debug.LogError("WinDataInit m_dicAllPortSign == null");
        }
        //m_dicAllPortSign = new Dictionary<string, int>();
        m_liAllPortsFlag = new List<int>();
        m_liConfirmPortsFlag = new List<int>();
        m_bsContnetDataLength = new int[1] { m_iDataLength };
        m_bsContnetData = new byte[100000];
        m_bsAllPorts = new byte[4];
        m_bsAllPortsNum = new byte[1];
        m_bolMappingSerialPort = true;
        m_strFrameEnd = "FD";
        m_iAllRegisterPortsNum = 0;
    }

    #region WindowsBLE蓝牙数据接收逻辑

    private void WindowsInit()
    {
        switch (SpringVr_Summary.Instance.reciveDataWay)
        {
            case SpringVr_Summary.ReceiveDataWay.WindowsSerialPort:
                SerialPort_GetPortsNum();
                ThreadPool.QueueUserWorkItem(new WaitCallback(SerialPort_MappingSerialPort), m_liAllPortsFlag);
                break;
            case SpringVr_Summary.ReceiveDataWay.WindowsBLE:
                WindowsBLEInit();
                break;
            default:
                break;
        }
    }

    private void WindowsBLEInit()
    {
        connectStageCallback = new BleCallBack(WinBleCallBack);
        BleInit(connectStageCallback);
        m_dicConnStatus = new Dictionary<ulong, bool>();
        int canConnectBLENum = BleGetDeviceNum();
        m_ui64AddrArray = new UInt64[canConnectBLENum];
        BleGetDeviceAddress(m_ui64AddrArray);
        for (int i = 0; i < canConnectBLENum; i++)
        {
            Debug.Log("检测到的地址 : " + m_ui64AddrArray[i].ToString("X2"));
        }
        if (canConnectBLENum != 0)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartDiscovery));
        }
    }

    /// <summary>蓝牙连接状态回调</summary>
    /// <param name="addr">地址</param>
    /// <param name="bConnectd">状态，TRUE为连接，FALSE为断开</param>
    public void WinBleCallBack(UInt64 addr, bool bConnectd)
    {
        if (bConnectd)
        {
            Debug.Log("蓝牙" + addr.ToString("X2") + "已连接");
        }
        else
        {
            Debug.Log("蓝牙" + addr.ToString("X2") + "已断开");
        }
        m_dicConnStatus[addr] = bConnectd;
    }

    /// <summary>搜索设备</summary>
    /// <returns>是否可以搜索设备</returns>
    public void StartDiscovery(object param)
    {
        int bleCount = BleGetDeviceNum();
        m_ui64AddrArray = new UInt64[bleCount];
        BleGetDeviceAddress(m_ui64AddrArray);
        for (int i = 0; i < bleCount; i++)
        {
            Debug.Log("检测到的地址 : " + m_ui64AddrArray[i].ToString("X2"));
            string tempBLEName = Marshal.PtrToStringAnsi(BleGetName(m_ui64AddrArray[i]));
            if (tempBLEName.StartsWith("MiiG"))
            {
                if (tempBLEName.Substring(8, 1) == "L")
                {
                    if (!SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData)
                    {
                        WindowsBLEReceiveData(m_ui64AddrArray[i], SpringVr_ReadLeftHandData.Instance, "FE81", "FD");
                        SpringVr_ReadLeftHandData.Instance.m_strBthAddr = tempBLEName; 
                        SpringVr_ReadLeftHandData.Instance.m_bolBeginReadData = true; 
                    }
                }
                else if (tempBLEName.Substring(8, 1) == "R")
                {
                    if (!SpringVr_ReadRightHandData.Instance.m_bolBeginReadData)
                    {
                        SpringVr_ReadRightHandData.Instance.m_bolBeginReadData = true;
                        SpringVr_ReadRightHandData.Instance.m_strBthAddr = tempBLEName; 
                        WindowsBLEReceiveData(m_ui64AddrArray[i], SpringVr_ReadRightHandData.Instance, "FE80", "FD");
                    }
                }

            }
        }
    }
    /// <summary>给蓝牙写入数据</summary>
    /// <param name="address">蓝牙地址</param>
    /// <param name="dataMge">数据</param>
    /// <returns>是否可以写入数据</returns>
    public void WriteData(string address, byte[] dataMge)
    {
        int dataLength = dataMge.Length;
        BleWriteDataToDevice(Convert.ToUInt64(address), dataMge, dataLength);
    }
    /// <summary>断开相应的蓝牙连接</summary>
    /// <param name="address">断开连接的蓝牙地址</param>
    public void CloseTargetBth(string address)
    {
    }
    /// <summary>断开所有的蓝牙连接</summary>
    public void CloseAllBth()
    {
        BleDisconnectAll();
        //SpringVrAllUseDataMgr.Instance.m_lsAllBthName.Clear();
    }

    public void WindowsBLEReceiveData(UInt64 addr, SpringVr_ReadDataBase readDataScripts, string frameHeadle, string frameEnd)
    {
        bool bConnected = false;
        StringBuilder tempStr = new StringBuilder();
        byte[] combinationData = new byte[m_iDataLength];
        m_dicConnStatus.TryGetValue(addr, out bConnected);
        byte[] data;
        int[] len;
        byte[] cmd = new byte[1];
        while (bConnected)
        {
            data = new byte[m_iDataLength];
            len = new int[1];
            len[0] = m_iDataLength;
            BleGetDeviceData(addr, data, len);
            if (len[0] == 0)
            {
                continue;
            }
            for (int i = 0; i < len[0]; i++)
            {
                tempStr.Append(data[i].ToString("X2"));
            }
            combinationData = DataCombination(ref tempStr, frameHeadle, frameEnd, m_iDataLength);
            if (combinationData == null) continue;
            cmd[0] = combinationData[1];
            readDataScripts.ContentData(ByteToFloatArray(m_iDataLength, combinationData), cmd);
        }
    }

    #endregion

    #region 串口数据接收逻辑
    /// <summary>拿到相应串口</summary>
    private void SerialPort_GetPortsNum()
    {
        ComGetPorts(m_bsAllPorts, m_bsAllPortsNum);
        for (int i = 0; i < m_bsAllPortsNum[0]; i++)
        {
            m_liAllPortsFlag.Add(m_bsAllPorts[i]);
            m_dicAllPortsData.Add(m_bsAllPorts[i], new StringBuilder());
        }
    }
    /// <summary>匹配串口</summary>
    private void SerialPort_MappingSerialPort(object arges)
    {
        List<int> allPorts = (List<int>)arges;
        Debug.LogError(allPorts.Count + " -- allPorts.Count");
        while (m_bolMappingSerialPort)
        {
            Thread.Sleep(100);
            for (int i = 0; i < allPorts.Count; i++)
            {
                if (m_liConfirmPortsFlag.Contains(allPorts[i])) continue;
                m_bsContnetDataLength[0] = m_iDataLength;
                StringBuilder tempStrBld = m_dicAllPortsData[allPorts[i]];
                m_bsContnetData = new byte[1000];
                ReceiveDataMoeule(ref tempStrBld, allPorts[i], m_bsContnetData, m_bsContnetDataLength);
                m_dicAllPortsData[allPorts[i]] = tempStrBld;
                if (m_dicAllPortsData[allPorts[i]].Length <= m_iDataLength) continue;
                foreach (KeyValuePair<string, int> frameHeadle in m_dicAllPortSign)
                {
                    if (frameHeadle.Value != 0) continue;
                    tempStrBld = m_dicAllPortsData[allPorts[i]];
                    byte[] checkData = DataCombination(ref tempStrBld, frameHeadle.Key, m_strFrameEnd, m_iDataLength);
                    m_dicAllPortsData[allPorts[i]] = tempStrBld;
                    if (checkData == null) continue;
                    m_liConfirmPortsFlag.Add(allPorts[i]);
                    m_dicAllPortSign[frameHeadle.Key] = allPorts[i];
                    if (Eve_ReadDataReady != null)
                    {
                        Eve_ReadDataReady(m_dicAllPortSign);
                    }
                    m_bolBeginContentData = true;
                    if (m_iAllRegisterPortsNum == m_liConfirmPortsFlag.Count)
                    {
                        m_bolMappingSerialPort = false;
                    }
                    break;
                }
            }
        }
    }


    /// <summary>外部调用接收数据方法</summary>
    /// <param name="portFlag">串口标志位</param>
    /// <param name="contentData">接收数据使用的空数组</param>
    /// <param name="dataLength">接收数据使用的空数组长度</param>
    /// <param name="frameHeadle">帧头</param>
    /// <param name="frameEnd">帧尾</param>
    /// <param name="readDataScript">读取数据的脚本</param>
    public void ContentData(int portFlag, int m_iDataLength, string frameHeadle, string frameEnd, SpringVr_ReadDataBase readDataScript)
    {
        while (!m_bolBeginContentData) Thread.Sleep(100);
        StringBuilder tempStr = new StringBuilder();
        byte[] combinationData = new byte[m_iDataLength];
        int[] dataLength = new int[1] { m_iDataLength };
        byte[] contentData = new byte[10000];
        byte[] cmd = new byte[1];
        m_lrAllReadDataScripts.Add(readDataScript);
        while (m_bolBeginContentData)
        {
                dataLength[0] = m_iDataLength;
                ReceiveDataMoeule(ref tempStr, portFlag, contentData, dataLength);
                combinationData = DataCombination(ref tempStr, frameHeadle, frameEnd, m_iDataLength);
                if (combinationData == null) continue;
                cmd[0] = combinationData[1];
                readDataScript.ContentData(ByteToFloatArray(m_iDataLength, combinationData), cmd);
        }
    }

    /// <summary>接收数据模块</summary>
    /// <param name="m_strbReceiveDataQueue">接收到的数据字符串</param>
    /// <param name="portFlag">串口标志位</param>
    /// <param name="contentData">接收数据使用的空数组</param>
    /// <param name="dataLength">接收数据使用的空数组长度</param>
    private void ReceiveDataMoeule(ref StringBuilder m_strbReceiveDataQueue, int portFlag, byte[] contentData, int[] dataLength)
    {
        ComGetRawData(portFlag, contentData, dataLength);
        if (dataLength[0] == 0)
        {
            //Thread.Sleep(5);
            return;
        }
        for (int i = 0; i < dataLength[0]; i++)
        {
            m_strbReceiveDataQueue.Append(contentData[i].ToString("X2"));
        }
        //Debug.Log(m_strbReceiveDataQueue);
    }
    public void AddPortMonitor(string monitorPortFrameHeadle)
    {
        if(m_dicAllPortSign == null)
        {
            m_dicAllPortSign = new Dictionary<string, int>();
            Debug.LogError("m_dicAllPortSign == null");
        }
        Debug.LogError(monitorPortFrameHeadle + " -- ");
        m_dicAllPortSign.Add(monitorPortFrameHeadle, new int());
        ++m_iAllRegisterPortsNum;
    }
    /// <summary>写入数据</summary>
    public void WriteData(int flag, byte[] writeData, int writeDataLength)
    {
        ComWriteData(flag, writeData, writeDataLength);
    }

    private void EndThread(object arges)
    {
        ComStop();
    }
    
    #endregion

#endif

    #endregion

    #region 数据处理模块
    /// <summary>完整数据处理</summary>
    /// <param name="contentData">接收到的数据</param>
    /// <param name="beginStr">数据帧头</param>
    /// <param name="endStr">数据帧尾</param>
    /// <param name="dataLength">一帧完整数据长度</param>
    /// <returns>校验完成后正确的一帧数据数组</returns>
    private byte[] DataCombination(ref StringBuilder contentData, string beginStr, string endStr, int dataLength)
    {
        string tempData = contentData.ToString();
        int indexOfFrameHeadle = tempData.IndexOf(beginStr);
        if (indexOfFrameHeadle < 0) return null;
        if (tempData.Length < indexOfFrameHeadle + dataLength * 2) return null;
        int indexOfFrameEnd = indexOfFrameHeadle + dataLength * 2 - endStr.Length;
        if (tempData.Substring(indexOfFrameEnd, endStr.Length) != endStr)
        {
            indexOfFrameHeadle = tempData.IndexOf(beginStr, indexOfFrameHeadle + 1);
            if (indexOfFrameHeadle < 0) return null;
            contentData.Remove(0, indexOfFrameHeadle);
            return null;
        }
        contentData.Remove(0, indexOfFrameHeadle + dataLength * 2);
        byte[] checkData = DataCheck(tempData.Substring(indexOfFrameHeadle, dataLength * 2), dataLength);
        if (checkData == null) return null;
        return checkData;
    }

    /// <summary>数据校验</summary>
    /// <returns>正确完整数据或null</returns>
    private byte[] DataCheck(string contentData, int dataLength)
    {
        if (contentData == null) return null;
        if (contentData.Length != dataLength * 2) return null;
        byte[] returnBytes = new byte[dataLength - 2];
        for (int i = 0; i < returnBytes.Length; i++)
        {
            returnBytes[i] = Convert.ToByte(contentData.Substring(2 + i * 2, 2), 16);
        }
        byte tempByte = returnBytes[0];
        for (int i = 1; i < returnBytes.Length - 1; i++)
        {
            tempByte ^= returnBytes[i];
        }
        if (returnBytes[returnBytes.Length - 1] != tempByte) return null;
        return returnBytes;
    }

    /// <summary>byte数组转换为float数组</summary>
    /// <param name="dataLength">一帧完整数据长度</param>
    /// <param name="resultData">需要转换的byte数组</param>
    /// <returns>可以直接赋给旋转使用的数据</returns>
    private float[] ByteToFloatArray(int dataLength, byte[] resultData)
    {
        if (resultData == null) return null;
        byte[] tempReceiveData = new byte[dataLength - 4];
        for (int i = 0; i < tempReceiveData.Length; i++)
        {
            tempReceiveData[i] = resultData[i + 1];
        }
        //此处将floatArrayLength减去FE/80/校验位/FD除以2，因为这里咱们是64个字符组成8个四元数，每个四元数的每个元素是由2个字符组成，所以此处要除以2
        int floatArrayLength = (dataLength - 4) / 2;
        float[] m_floAllData = new float[floatArrayLength];
        for (int i = 0; i < floatArrayLength; i++)
        {
            //这一行是最重要的，将返回的字符每相邻的两个字符通过这种形式融合为一个浮点类型的小数，这个数可以直接用来做四元数的元素
            m_floAllData[i] = ((Int16)((byte)tempReceiveData[2 * i + 1] << 8 | (byte)tempReceiveData[2 * i + 2])) / 10000.0f;
        }
        return m_floAllData;
    }


    #endregion

    #region 退出
    void OnApplicationQuit()
    {
#if UNITY_ANDROID
        ThreadPool.QueueUserWorkItem(new WaitCallback(AndroidQuit));
#elif UNITY_STANDALONE_WIN
        m_bolMappingSerialPort = false;
        m_bolBeginContentData = false;
        for (int i = 0; i < m_lrAllReadDataScripts.Count; i++)
        {
            if (m_lrAllReadDataScripts[i] == null) continue;
            m_lrAllReadDataScripts[i].m_tReadDataThread.Join();//等待线程结束
        }
        ThreadPool.QueueUserWorkItem(new WaitCallback(EndThread));
#endif
    }

#endregion

    #endregion
}