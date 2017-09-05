using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_FingerMoveGetsturCalculate : SpringVr_GestureCalculateBase
{
    private Vector3 m_vecLastPostion;
    private float m_floFingerMoveDis;
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        m_floFingerMoveDis = 0;
        tempBol = true;
        tempBol = targetList[4] == 0 ? tempBol : false;
        tempBol = targetList[3] == 2  ? tempBol : false;
        if (!tempBol) return (T)(object)m_floFingerMoveDis;
        for (int i = 6; i < targetList.Length; i++)
        {
            tempBol = targetList[i] == 2 || targetList[i] == 1 ? tempBol : false;
        }
        if (tempBol)
        {
            tempBol = targetList[5] == 0? tempBol : false;
            if (tempBol)
            {
                m_floFingerMoveDis = Vector3.Cross(allJoint[4].forward, m_vecLastPostion).y;
                if (m_floFingerMoveDis > -0.002f || m_floFingerMoveDis < -0.1f)
                {
                    m_floFingerMoveDis = 0;
                }
            }
            else
            {
                m_floFingerMoveDis = Vector3.Cross(allJoint[4].forward, m_vecLastPostion).y;
                if (m_floFingerMoveDis < 0.002f || m_floFingerMoveDis > 0.1f)
                {
                    m_floFingerMoveDis = 0;
                }
            }
        }
        else
        {
            m_floFingerMoveDis = 0;
        }
        m_vecLastPostion = allJoint[4].forward;
        return (T)(object)m_floFingerMoveDis;
    }
}
