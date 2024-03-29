﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_LaserGestureCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = true;
        for (int i = 4; i < targetList.Length; i++)
        {
            tempBol = targetList[i] == 2 ? tempBol : false;
            if (i == 4)
            {
                //Debug.Log(targetList[i].localEulerAngles.z);
            }
        }
        return (T)(object)tempBol;
    }
}
