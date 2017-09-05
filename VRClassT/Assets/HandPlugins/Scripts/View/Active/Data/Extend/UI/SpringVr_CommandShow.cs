/** 
 *Copyright(C) 2015 by #COMPANY# 
 *All rights reserved. 
 *FileName:     #SCRIPTFULLNAME# 
 *Author:       #AUTHOR# 
 *Version:      #VERSION# 
 *UnityVersion：#UNITYVERSION# 
 *Date:         #DATE# 
 *Description:    
*/
using UnityEngine;
using System.Collections.Generic;

public class SpringVr_CommandShow
{

    #region 数据
    private Dictionary<int,List<string>>    m_dicAllText;
    private List<string>                    m_lstrHandGroups;
    private List<string>                    m_lstrTimesEventShowText;
    private List<string>                    m_lstrFloatShowText;
    private List<int>                       m_liTimes;
    private List<int>                       m_liAllSuperposition;
    private List<bool>                      m_lbolAllSuperposition;
    private bool                            m_bolShowEventMsg;
    private float                           m_floHeight;
    #endregion

    #region 单例
    private static SpringVr_CommandShow instance;
    public static SpringVr_CommandShow Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SpringVr_CommandShow();
            }
            return instance;
        }
    }

    #endregion

    #region 函数

    #region 初始化
    private SpringVr_CommandShow()
    {
        DataInit();
        EventRegister();
    }
    /// <summary>事件注册</summary>
    private void EventRegister()
    {
        SpringVr_ReadLeftHandData.Instance.eve_GetBtnDown               += LeftHandGetBtnDown;
        SpringVr_ReadLeftHandData.Instance.eve_GetBtnUp                 += LeftHandGetBtnUp;
        SpringVr_ReadLeftHandData.Instance.eve_PressBtn                 += LeftHandPressBtn;
        SpringVr_ReadLeftHandData.Instance.eve_StaticCorrection         += LeftHandStaticCorrection;
        SpringVr_ReadLeftHandData.Instance.eve_DynamicCorrection        += LeftHandDynamicCorrection;
        SpringVr_LeftHandCtrl.Instace.eve_Boxing                        += LeftHandBoxing;
        SpringVr_LeftHandCtrl.Instace.eve_PalmDlg                       += LeftHandPalmDlg;
        SpringVr_LeftHandCtrl.Instace.eve_IndexFinger                   += LeftHandIndexFinger;
        SpringVr_LeftHandCtrl.Instace.eve_RockDlg                       += LeftHandRockDlg;
        SpringVr_LeftHandCtrl.Instace.eve_ThumbStage                    += LeftHandThumbStageDlg;
        SpringVr_LeftHandCtrl.Instace.eve_IndexMiddleFinger             += LeftIndexMiddle;
        SpringVr_LeftHandCtrl.Instace.eve_FingerMove                    += LeftHandFingerMove;
        SpringVr_LeftHandCtrl.Instace.eve_OK                            += LeftHandOK;

        SpringVr_ReadRightHandData.Instance.eve_GetBtnDown              += RightHandGetBtnDown;
        SpringVr_ReadRightHandData.Instance.eve_GetBtnUp                += RightHandGetBtnUp;
        SpringVr_ReadRightHandData.Instance.eve_PressBtn                += RightHandPressBtn;
        SpringVr_ReadRightHandData.Instance.eve_StaticCorrection        += RightHandStaticCorrection;
        SpringVr_ReadRightHandData.Instance.eve_DynamicCorrection       += RightHandDynamicCorrection;
        SpringVr_RightHandCtrl.Instace.eve_Boxing                       += RightHandBoxing;
        SpringVr_RightHandCtrl.Instace.eve_PalmDlg                      += RightHandPalmDlg;
        SpringVr_RightHandCtrl.Instace.eve_IndexFinger                  += RightHandIndexFinger;
        SpringVr_RightHandCtrl.Instace.eve_RockDlg                      += RightHandRockDlg;
        SpringVr_RightHandCtrl.Instace.eve_ThumbStage                   += RightHandThumbStageDlg;
        SpringVr_RightHandCtrl.Instace.eve_IndexMiddleFinger            += RightIndexMiddle;
        SpringVr_RightHandCtrl.Instace.eve_FingerMove                   += RightHandFingerMove;
        SpringVr_RightHandCtrl.Instace.eve_OK                           += RightHandOK;
    }

    /// <summary>数据初始化</summary>
    private void DataInit()
    {
        m_liAllSuperposition    = new List<int>();
        m_lbolAllSuperposition  = new List<bool>();
        m_dicAllText            = new Dictionary<int, List<string>>();
        m_lstrHandGroups        = new List<string> { "左手信息", "右手信息" };
        m_lstrTimesEventShowText= new List<string> { "单击", "长按结束" , "长按" , "静态校准完毕" , "动态校准完毕" , "握拳手势", "手掌手势", "食指伸出", "摇滚手势", "拇指抬起", "拇指放下", "食指中指伸出" ,"OK手势"};
        m_lstrFloatShowText     = new List<string> { "手指滑动距离" };
        m_liTimes               = new List<int>();
        m_floHeight             = 150.0f;
        for (int i = 0; i < m_lstrTimesEventShowText.Count; i++)
        {
            m_liAllSuperposition.Add(0);
            m_lbolAllSuperposition.Add(false);
        }
        for (int i = 0; i < 2; i++)
        {
            List<string> tempList = new List<string>();
            for (int j = 0; j < m_lstrTimesEventShowText.Count; j++)
            {
                tempList.Add("次数");
            }
            for (int j = 0; j < m_lstrFloatShowText.Count; j++)
            {
                tempList.Add("手指滑动");
            }
            m_dicAllText.Add(i, tempList);
        }
        for (int i = 0; i < m_lstrTimesEventShowText.Count * 2; i++)
        {
            m_liTimes.Add(-1);
        }
        for (int i = 0; i < m_lstrTimesEventShowText.Count; i++)
        {
            ContentCtrl(0, i);
        }
        for (int i = 0; i < m_lstrTimesEventShowText.Count; i++)
        {
            ContentCtrl(1, i);
        }
    }
    #endregion

    #region GUI显示

    public int EventTimesShowOnGUI(Rect windowRect,bool down,float downSpeed)
    {
        GUI.DragWindow(new Rect(0, 0, windowRect.width, windowRect.height));
        for (int j = 0; j < 2; j++)
        {
            if (down)
            {
                if (m_floHeight < 400)
                {
                    m_floHeight += downSpeed / 4;
                }
                GUI.BeginGroup(new Rect(j * windowRect.width / 2, m_floHeight, windowRect.width / 2, windowRect.height));
            }
            else
            {
                if (m_floHeight > 150)
                {
                    m_floHeight -= downSpeed / 4;
                }
                GUI.BeginGroup(new Rect(j * windowRect.width / 2, m_floHeight, windowRect.width / 2, windowRect.height));
            }
            GUI.Label(new Rect(windowRect.width / 6, 0, windowRect.width / 2 - 15, 25), m_lstrHandGroups[j]);
            for (int i = 0; i < m_lstrTimesEventShowText.Count; i++)
            {
                GUI.Box(new Rect(5, i * 30 + 20, windowRect.width / 2 - 15, 25), m_dicAllText[j][i]);
            }
            for (int i = 0; i < m_lstrFloatShowText.Count; i++)
            {
                GUI.Box(new Rect(5, (i + m_lstrTimesEventShowText.Count) * 30 + 20, windowRect.width / 2 - 15, 25), m_dicAllText[j][i + m_lstrTimesEventShowText.Count]);
            }
            GUI.EndGroup();
        }
        return m_lstrTimesEventShowText.Count * 25;
    }

    #endregion

    #region 事件实现函数
    private void LeftHandGetBtnDown()
    {
        ContentCtrl(0, 0);
    }
    private void LeftHandGetBtnUp()
    {
        ContentCtrl(0, 1);
    }
    private void LeftHandPressBtn()
    {
        ContentCtrl(0, 2);
    }
    private void LeftHandStaticCorrection()
    {
        ContentCtrl(0, 3);
    }
    private void LeftHandDynamicCorrection()
    {
        ContentCtrl(0, 4);
    }
    private void LeftHandBoxing(bool tempBol,List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 0, 0, 5);
    }
    private void LeftHandPalmDlg(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 1, 0, 6);
    }
    private void LeftHandIndexFinger(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 2, 0, 7);
    }
    private void LeftHandRockDlg(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 3, 0, 8);
    }
    private void LeftHandThumbStageDlg(SpringVr_GestureCalculateBase.ForearmKinestate kinestate, List<Transform> allJoint)
    {
        switch (kinestate)
        {
            case SpringVr_GestureCalculateBase.ForearmKinestate.Static:
                break;
            case SpringVr_GestureCalculateBase.ForearmKinestate.OutWord:
                ContentCtrl(0, 9);
                break;
            case SpringVr_GestureCalculateBase.ForearmKinestate.Entad:
                ContentCtrl(0, 10);
                break;
            default:
                break;
        }
    }
    private void LeftHandFingerMove(float distance, List<Transform> allJoints)
    {
        ContentCtrl(0, m_lstrTimesEventShowText.Count, distance);
    }
    private void LeftIndexMiddle(bool tempBol,List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 4, 0, 11);
    }
    private void LeftHandOK(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 5, 0, 12);
    }


    private void RightHandGetBtnDown()
    {
        ContentCtrl(1, 0);
    }
    private void RightHandGetBtnUp()
    {
        ContentCtrl(1, 1);
    }
    private void RightHandPressBtn()
    {
        ContentCtrl(1, 2);
    }
    private void RightHandStaticCorrection()
    {
        ContentCtrl(1, 3);
    }
    private void RightHandDynamicCorrection()
    {
        ContentCtrl(1, 4);
    }

    private void RightHandBoxing(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 4, 1, 5);
    }
    private void RightHandPalmDlg(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 5, 1, 6);
    }
    private void RightHandIndexFinger(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 6, 1, 7);
    }
    private void RightHandRockDlg(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 7, 1, 8);
    }
    private void RightHandThumbStageDlg(SpringVr_GestureCalculateBase.ForearmKinestate kinestate, List<Transform> allJoint)
    {
        switch (kinestate)
        {
            case SpringVr_GestureCalculateBase.ForearmKinestate.Static:
                break;
            case SpringVr_GestureCalculateBase.ForearmKinestate.OutWord:
                ContentCtrl(1, 9);
                break;
            case SpringVr_GestureCalculateBase.ForearmKinestate.Entad:
                ContentCtrl(1, 10);
                break;
            default:
                break;
        }
    }
    private  void RightIndexMiddle(bool tempBol,List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 8, 1, 11);
    }
    private void RightHandOK(bool tempBol, List<Transform> allJoint)
    {
        SuperpositionConfirmation(tempBol, 9, 1, 12);
    }
    private void RightHandFingerMove(float distance, List<Transform> allJoints)
    {
        ContentCtrl(1, m_lstrTimesEventShowText.Count, distance);
    }
    #endregion

    /// <summary>触发才调用事件的处理函数(显示次数)</summary>
    /// <param name="handNum">手的标志位</param>
    /// <param name="timeNm">次数</param>
    private void ContentCtrl(int handNum, int timeNm)
    {
        ++m_liTimes[handNum * m_lstrTimesEventShowText.Count + timeNm];
        m_dicAllText[handNum][timeNm] = m_lstrTimesEventShowText[timeNm] + "次数：" + m_liTimes[handNum * m_lstrTimesEventShowText.Count + timeNm];
    }
    /// <summary>触发才调用事件的处理函数(显示幅度) </summary>
    /// <param name="handNum">手的标志位</param>
    /// <param name="flog">显示的内容说明</param>
    /// <param name="moveDis">幅度</param>
    private void ContentCtrl(int handNum,int flog,float moveDis)
    {
        m_dicAllText[handNum][flog] = m_lstrFloatShowText[flog - m_lstrTimesEventShowText.Count] + " ：" + moveDis;
    }
    /// <summary>一直被调用触发传入true参数的处理函数</summary>
    /// <param name="tempBol">此帧传过来的BOOL值</param>
    /// <param name="flag">标志位</param>
    /// <param name="group">组别</param>
    /// <param name="serialNum">编号</param>
    private void SuperpositionConfirmation(bool tempBol,int flag,int group,int serialNum)
    {
        if (m_lbolAllSuperposition[flag] != tempBol)
        {
            ++m_liAllSuperposition[flag];
            if (m_liAllSuperposition[flag] > 5)
            {
                m_lbolAllSuperposition[flag] = tempBol;
                m_liAllSuperposition[flag] = 0;
                if (m_lbolAllSuperposition[flag])
                {
                    ContentCtrl(group, serialNum);
                }
            }
        }
        else
        {
            m_liAllSuperposition[flag] = 0;
        }
    }
    #endregion
}