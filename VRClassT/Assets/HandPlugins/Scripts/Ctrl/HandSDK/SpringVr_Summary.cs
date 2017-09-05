using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpringVr_Summary : MonoBehaviour {

    #region 枚举
    public enum ReceiveDataWay
    {
        /// <summary>串口接收数据</summary>
        WindowsSerialPort,
        /// <summary>WindowsBLE蓝牙接收数据</summary>
        WindowsBLE,
        /// <summary>AndroidBLE接收数据</summary>
        AndroidBLE
    }
    #endregion

    #region 数据声明
    [HideInInspector]
    public SpringVr_SerialPortTool.NumberOfSensors numberOfSensors;
    [HideInInspector]
    public SpringVr_HandPositionCtrl.PositioningSystem positioningSystem;
    [HideInInspector]
    public Transform HeadTracker;
    [HideInInspector]
    public Transform RightHandTracker;
    [HideInInspector]
    public Transform LeftHandTracker;
    [HideInInspector]
    public ReceiveDataWay reciveDataWay;

    private SpringVr_HandPositionCtrl handPositionCtrl;
    private SpringVr_SerialPortTool serialPortTool;
    #endregion

    #region 单例
    private static SpringVr_Summary instance;
    public static SpringVr_Summary Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region 函数

    #region 初始化
    void Awake()
    {
        serialPortTool = transform.Find("Model/SVReadData").GetComponent<SpringVr_SerialPortTool>();
        handPositionCtrl = transform.Find("Ctrl/HandPositionCtrl").GetComponent<SpringVr_HandPositionCtrl>();
        serialPortTool.numberOfSensors = numberOfSensors;
        handPositionCtrl.positioningSystem = positioningSystem;
    }
    private void OnEnable()
    {
        instance = this;
    }
    #endregion

    void Update()
    {
        if (serialPortTool == null)
        {
            serialPortTool = transform.Find("Model/SVReadData").GetComponent<SpringVr_SerialPortTool>();
        }
        serialPortTool.numberOfSensors = numberOfSensors;
        if (handPositionCtrl == null)
        {
            handPositionCtrl = transform.Find("Ctrl/HandPositionCtrl").GetComponent<SpringVr_HandPositionCtrl>();
        }
        handPositionCtrl.positioningSystem = positioningSystem;
    }
    #endregion
}
