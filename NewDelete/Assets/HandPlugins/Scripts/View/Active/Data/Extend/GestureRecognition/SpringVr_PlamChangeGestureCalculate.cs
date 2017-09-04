using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_PalmChangeGestureCalculate : SpringVr_GestureCalculateBase
{
    //private float m_floLastPalmTimer;
    //private bool m_bolTimeBein;
    private bool m_bolSpace;
    //private int m_iTimes;
    public override T GestureCalculate<T>(int[] targetList, List<Transform> allJoint,SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        //Debug.Log(allJoint[2].up.y);
        //if (allJoint[2].up.y > -0.75f)
        //{
        //    return(T)(object)false;
        //}
        if (lastGesture == SpringVr_HandActiveCtrlExtend.AllGesture.Palm)
        {
            //m_bolTimeBein = true;
            m_bolSpace = true;
        }
        //if (m_bolTimeBein)
        //{
        //    m_floLastPalmTimer += Time.deltaTime;
        //}
        //if (m_floLastPalmTimer > 3f)
        //{
        //    StateReturnZero();
        //}
        if (m_bolSpace)
        {
            tempBol = true;
            for (int i = 4; i < targetList.Length; i++)
            {
                tempBol = targetList[i] == 2  ? tempBol : false;
            }
            if (tempBol)
            {
                m_bolSpace = false;
            }
        }
        else
        {
            tempBol = false;
        }
        //if (tempBol)
        //{
        //    ++m_iTimes;
        //    if (m_iTimes >= 2)
        //    {
        //        StateReturnZero();
        //    }
        //    else
        //    {
        //        tempBol = false;
        //    }
        //}
        return (T)(object)tempBol;
    }
    //private void StateReturnZero()
    //{
    //    m_floLastPalmTimer = 0;
    //    m_bolTimeBein = false;
    //    m_iTimes = 0;
    //    m_bolSpace = false;
    //}
}
