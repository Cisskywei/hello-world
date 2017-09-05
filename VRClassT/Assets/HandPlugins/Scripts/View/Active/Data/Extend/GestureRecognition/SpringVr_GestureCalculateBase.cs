using UnityEngine;
using System.Collections.Generic;

public abstract class SpringVr_GestureCalculateBase
{
    #region 动态识别数据声明

    #region 枚举
    public enum ForearmKinestate
    {
        /// <summary>静止</summary>
        Static,
        /// <summary>向外运动</summary>
        OutWord,
        /// <summary>向内运动</summary>
        Entad,
    }
    #endregion

    protected ForearmKinestate currentStage;
    protected ForearmKinestate lastStage;
    protected float m_floAngle;
    protected float m_floLastAngle;
    /// <summary>两个角度之间的最小间隔 </summary>
    protected float m_floInterval = 0.3f;
    /// <summary>小于这个角度则为向内运动</summary>
    protected float m_floDycMinAngle;
    /// <summary>大于这个角度则为向外运动</summary>
    protected float m_floDycMaxAngle;
    /// <summary>夹角增大叠加次数 </summary>
    protected int   m_iBigger;
    /// <summary>夹角减小叠加次数 </summary>
    protected int   m_iDecrease;
    /// <summary>叠加次数大于此值则改变状态</summary>
    protected int   m_iDelicacy = 10;
    protected bool  m_bolBiggerMark;
    protected bool  m_bolDecreaseMark;
    #endregion

    #region 静态识别数据声明
    protected int   straightMaxAngle = 20;
    protected int   middleAngle = 130;
    protected int   curveMinAngle = 295;
    protected int   curveMaxAngle = 325;
    protected bool  tempBol;
    #endregion

    #region 函数

    public SpringVr_GestureCalculateBase()
    {
        m_bolBiggerMark = true;
        m_bolDecreaseMark = true;
    }

    public abstract T GestureCalculate<T>(int[] targetList, List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture);

    /// <summary>动态状态监测</summary>
    /// <param name="currentNumerical">当前状态</param>
    /// <returns>动态状态</returns>
    protected ForearmKinestate StageDetection(float currentNumerical)
    {
        if (currentNumerical - m_floLastAngle > m_floInterval)
        {
            m_iBigger++;
            if (m_iDecrease > 0)
            {
                m_iDecrease--;
            }
        }
        else if (currentNumerical - m_floLastAngle < -m_floInterval)
        {
            m_iDecrease++;
            if (m_iBigger > 0)
            {
                m_iBigger--;
            }
        }
        m_floLastAngle = currentNumerical;
        if (m_iBigger >= m_iDelicacy)
        {
            if (currentNumerical > m_floDycMaxAngle)
            {
                m_bolDecreaseMark = true;
                if (m_bolBiggerMark)
                {
                    m_bolBiggerMark = false;
                    currentStage = ForearmKinestate.OutWord;
                    m_iBigger = 0;
                    m_iDecrease = 0;
                }
            }
        }
        else if (m_iDecrease >= m_iDelicacy)
        {
            if (currentNumerical < m_floDycMinAngle)
            {
                m_bolBiggerMark = true;
                if (m_bolDecreaseMark)
                {
                    m_bolDecreaseMark = false;
                    m_iBigger = 0;
                    m_iDecrease = 0;
                    currentStage = ForearmKinestate.Entad;
                }
            }
        }
        switch (lastStage)
        {
            case ForearmKinestate.Static:
                break;
            case ForearmKinestate.OutWord:
            case ForearmKinestate.Entad:
                currentStage = ForearmKinestate.Static;
                break;
            default:
                break;
        }
        lastStage = currentStage;
        //Debug.Log(currentStage);
        return currentStage;
    }
    #endregion
}
