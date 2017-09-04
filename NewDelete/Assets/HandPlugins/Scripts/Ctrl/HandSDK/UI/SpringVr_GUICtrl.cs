using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class SpringVr_GUICtrl : SpringVr_OnGUI
{

    #region 枚举
    public enum HandMark
    {
        /// <summary>右手</summary>
        RightHand,
        /// <summary>左手</summary>
        LeftHand,
        /// <summary>双手</summary>
        DoubleHand
    }
    public enum CorrectionWay
    {
        /// <summary>静态校准</summary>
        Static,
        /// <summary>动态校准</summary>
        Dynamic
    }
    #endregion

    #region 成员变量
    [Header("动态添加菜单选项")]
    public      List<string>                    m_lsMenuRowName;
    [Header("动态添加菜单选项触发调用的方法")]
    public      List<TimeEventTrigger>          m_ltAllList;                                    
    [HideInInspector]
    public      float                           m_floHorizontalValue;
    [HideInInspector]
    public      bool                            m_bolChangeLeftRightCtrl;
    private     List<byte[]>                    m_lbAllWriteDataByte;
    private     Rect                            YawCtrlPosition;             
    private     Rect                            RightLeftCtrlInitState;     
    private     Rect                            InitAngleCtrlTextPosition;      
    private     Rect                            InitAngleCtrlPosition;      
    private     Rect                            DropDownInitState;
    private     Rect                            windowRect;
    private     Rect                            tempWindowRect;
    private     string[]                        m_strWriteDatas;
    private     float[]                         m_floAllBtnY;
    private     float                           m_floSpeed;
    private     float                           m_floTempHorizontalValue;
    private     int                             m_iCommandHeigh;
    private     bool                            m_bolIsOpen;
    private     bool                            m_bolOpenCourseAngle;
    private     Transform[]                     hands;
    private     string                          m_strWinPsiCtrlName;
    private     string[]                        m_strWinPsiCtrlNames;
    private     Vector2                         m_v2ScrollPosition;
    #endregion

    #region 单例
    private static SpringVr_GUICtrl instance;
    public static SpringVr_GUICtrl Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region 函数

    #region 初始化
    protected override void OnAwake()
    {
        base.OnAwake();
        instance = this;
        DataInit();
    }
    protected override void OnStart()
    {
        base.OnStart();
        m_floHorizontalValue = PlayerPrefs.GetFloat("m_floHorizontalValue");
        m_floTempHorizontalValue = m_floHorizontalValue;
        switch (SpringVr_SerialPortTool.Instance.numberOfSensors)
        {
            //case SpringVr_SerialPortTool.NumberOfSensors.SixSensors:
            case SpringVr_SerialPortTool.NumberOfSensors.SevenSensors:
                hands[0] = RightHandActiveView.allHandJoint[2];
                hands[1] = LeftHandActiveView.allHandJoint[2];
                InitAngleCtrlTextPosition = new Rect(10, 60, 150, 20);
                InitAngleCtrlPosition = new Rect(10, 80, windowRect.width * 9 / 10, 20);
                DropDownInitState = new Rect(10, 100, 100, 40);
                break;
            case SpringVr_SerialPortTool.NumberOfSensors.EightSensors:
            case SpringVr_SerialPortTool.NumberOfSensors.NineSensors:
                InitAngleCtrlTextPosition = new Rect(10, 40, 150, 20);
                InitAngleCtrlPosition = new Rect(10, 60, windowRect.width * 9 / 10, 20);
                DropDownInitState = new Rect(10, 80, 100, 40);
                break;
            default:
                break;
        }
        for (int i = 0; i < m_floAllBtnY.Length; i++)
        {
            m_floAllBtnY[i] = DropDownInitState.y;
        }
    }
    /// <summary>数据初始化</summary>
    private void DataInit()
    {
        m_iCommandHeigh                         = 0;
        m_floSpeed                              = 20;
        m_bolIsOpen                             = false;
        windowRect                              = new Rect(0, 0, Screen.width / 3f, Screen.height);
        YawCtrlPosition                         = new Rect(10, 20, 100, 20);
        RightLeftCtrlInitState                  = new Rect(10, 40, 143, 20);
        tempWindowRect                          = windowRect;
        m_floAllBtnY                            = new float[m_lsMenuRowName.Count];
        m_strWinPsiCtrlNames                     = new string[2];
        m_strWinPsiCtrlNames[0]                  = "命\n令\n隐\n藏";
        m_strWinPsiCtrlNames[1]                  = "命\n令\n显\n示";
        m_strWinPsiCtrlName                     = m_strWinPsiCtrlNames[1];
        hands = new Transform[2];
    }
    #endregion

    #region GUI显示
    protected override void onGUI()
    {
        base.onGUI();
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "手套返回命令显示框");
        if (GUI.Button(new Rect(windowRect.x + windowRect.width, windowRect.y, windowRect.width / 10, windowRect.height / 5), m_strWinPsiCtrlName))
        {
            if (m_strWinPsiCtrlName == m_strWinPsiCtrlNames[1])
            {
                m_strWinPsiCtrlName = m_strWinPsiCtrlNames[0];
            }
            else
            {
                m_strWinPsiCtrlName = m_strWinPsiCtrlNames[1];
            }
        }
        if (m_strWinPsiCtrlName == m_strWinPsiCtrlNames[1])
        {
            if (windowRect.x > -windowRect.width)
            {
                windowRect.x -= m_floSpeed / 2;
            }
            if (windowRect.y > tempWindowRect.y || windowRect.y < tempWindowRect.y)
            {
                windowRect.y -= Mathf.Abs(windowRect.y) / windowRect.y * m_floSpeed / 2;
            }
        }
        if (m_strWinPsiCtrlName == m_strWinPsiCtrlNames[0])
        {
            if (windowRect.x < 0)
            {
                windowRect.x += m_floSpeed / 2;
            }
        }
    }
    private void DoMyWindow(int windowID)
    {
        m_v2ScrollPosition = GUI.BeginScrollView(new Rect(10, 0, windowRect.width - 10, windowRect.height - 20), m_v2ScrollPosition, new Rect(0, 0, windowRect.width + 20, windowRect.height + m_iCommandHeigh - 40));

        #region 开关航向角
        if (GUI.Toggle(YawCtrlPosition, m_bolOpenCourseAngle, "开关航向角"))
        {
            m_bolOpenCourseAngle = true;
        }
        else
        {
            m_bolOpenCourseAngle = false;
        }
        ToggleValueChanged(m_bolOpenCourseAngle);
        #endregion
        
        #region 定位器控制
        switch (SpringVr_SerialPortTool.Instance.numberOfSensors)
        {
            //case SpringVr_SerialPortTool.NumberOfSensors.SixSensors:
            case SpringVr_SerialPortTool.NumberOfSensors.SevenSensors:
                if (GUI.Toggle(RightLeftCtrlInitState, m_bolChangeLeftRightCtrl, "左右控制器调换"))
                {
                    m_bolChangeLeftRightCtrl = true;
                }
                else
                {
                    m_bolChangeLeftRightCtrl = false;
                }
                break;
            case SpringVr_SerialPortTool.NumberOfSensors.EightSensors:
                break;
            default:
                break;
        }
        #endregion

        #region 拖动控制手势初始角度旋转
        GUI.Label(InitAngleCtrlTextPosition, "拖动控制手势初始角度旋转");
        m_floHorizontalValue = GUI.HorizontalSlider(InitAngleCtrlPosition, m_floHorizontalValue, 0, 360);
        if (m_floTempHorizontalValue != m_floHorizontalValue)
        {
            PlayerPrefs.SetFloat("m_floHorizontalValue", m_floHorizontalValue);
        }
        m_floTempHorizontalValue = m_floHorizontalValue;
        InitAngleIncrement(m_floHorizontalValue);
        #endregion

        #region 手套校正下拉菜单
        if (GUI.Button(DropDownInitState, "手套校正"))
        {
            if (m_bolIsOpen)
            {
                m_bolIsOpen = false;
                for (int j = 0; j < m_lsMenuRowName.Count; j++)
                {
                    m_floAllBtnY[j] = DropDownInitState.y;
                }
            }
            else
            {
                m_bolIsOpen = true;
            }
        }
        if (m_bolIsOpen)
        {
            for (int i = 0; i < m_lsMenuRowName.Count; i++)
            {
                if (m_floAllBtnY[i] < DropDownInitState.y + DropDownInitState.height * (i + 1))
                {
                    m_floAllBtnY[i] += m_floSpeed / 2;
                }
                //校准下拉菜单按钮
                if (GUI.Button(new Rect(DropDownInitState.x, m_floAllBtnY[i], DropDownInitState.width, DropDownInitState.height), m_lsMenuRowName[i]))
                {
                    m_bolIsOpen = false;
                    for (int j = 0; j < m_lsMenuRowName.Count; j++)
                    {
                        m_floAllBtnY[j] = DropDownInitState.y;
                    }
                    m_ltAllList[i].m_TimeEvent.Invoke();//触发此脚本在Inspector面板上“AllList”中添加的方法
                }
            }
        }
        #endregion

        //事件触发次数显示
        m_iCommandHeigh = SpringVr_CommandShow.Instance.EventTimesShowOnGUI(windowRect, m_bolIsOpen,m_floSpeed);
        GUI.EndScrollView();
    }

    #endregion

    /// <summary>初始角度增量</summary>
    private void InitAngleIncrement(float horizontalValue)
    {
        if (LeftHandActiveView != null)
        {
            LeftHandActiveView.m_floHorizontalValue = horizontalValue;
        }
        if (RightHandActiveView != null)
        {
            RightHandActiveView.m_floHorizontalValue = horizontalValue;
        }
    }


    /// <summary>开关航向角</summary>
    private void ToggleValueChanged(bool isSon)
    {
        if (isSon)
        {
            if (LeftHandActiveView != null)
            {
                LeftHandActiveView.m_bolOpenOrCloseCourseAngle = true;
            }
            if (RightHandActiveView != null)
            {
                RightHandActiveView.m_bolOpenOrCloseCourseAngle = true;
            }
        }
        else
        {
            if (LeftHandActiveView != null)
            {
                LeftHandActiveView.m_bolOpenOrCloseCourseAngle = false;
            }
            if (RightHandActiveView != null)
            {
                RightHandActiveView.m_bolOpenOrCloseCourseAngle = false;
            }
        }
    }

    #region 在Inspector面板添加的左右手动态、静态校准方法，外部可直接以相同方式调用，触发相应校准事件
    /// <summary>右手静态校准</summary>
    public void RightHandStaticCalibration()
    {
        if (SpringVr_ReadRightHandData.Instance != null)
        {
            SpringVr_ReadRightHandData.Instance.StaticCalibration();
        }
        else
        {
            Debug.LogError("[Spring-Vr]读取右手数据失败，请检查右手是否可以正常传入数据！");
        }
    }
    /// <summary>右手动态校准</summary>
    public void RightHandDynamicCalibration()
    {
        if (SpringVr_ReadRightHandData.Instance != null)
        {
            SpringVr_ReadRightHandData.Instance.DynamicCalibration();
        }
        else
        {
            Debug.LogError("[Spring-Vr]读取右手数据失败，请检查右手是否可以正常传入数据！");
        }
    }
    /// <summary>右手拇指校准</summary>
    public void RightHandThumbOffsetCalibration()
    {
        if (SpringVr_ReadRightHandData.Instance != null)
        {
            RightHandActiveView.ThumbOffsetCalibration();
        }
        else
        {
            Debug.LogError("[Spring-Vr]读取右手数据失败，请检查右手是否可以正常传入数据！");
        }
    }
    /// <summary>左手静态校准</summary>
    public void LeftHandStaticCalibration()
    {
        if (SpringVr_ReadLeftHandData.Instance != null)
        {
            SpringVr_ReadLeftHandData.Instance.StaticCalibration();
        }
        else
        {
            Debug.LogError("[Spring-Vr]读取左手数据失败，请检查右手是否可以正常传入数据！");
        }
    }
    /// <summary>左手动态校准</summary>
    public void LeftHandDynamicCalibration()
    {
        if (SpringVr_ReadLeftHandData.Instance != null)
        {
            SpringVr_ReadLeftHandData.Instance.DynamicCalibration();
        }
        else
        {
            Debug.LogError("[Spring-Vr]读取左手数据失败，请检查右手是否可以正常传入数据！");
        }
    }
    /// <summary>左手拇指校准</summary>
    public void LeftHandThumbOffsetCalibration()
    {
        if (SpringVr_ReadLeftHandData.Instance != null)
        {
            LeftHandActiveView.ThumbOffsetCalibration();
        }
        else
        {
            Debug.LogError("[Spring-Vr]读取右手数据失败，请检查右手是否可以正常传入数据！");
        }
    }
    #endregion

    #endregion
}

#region Inspector面板事件添加框
[Serializable]
public class TimeEventTrigger
{
    [Serializable]
    public class TimeEvent : UnityEvent { }
    [SerializeField]
    public TimeEvent m_TimeEvent = new TimeEvent();
}
#endregion

