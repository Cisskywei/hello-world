using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_PalmGestureCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = true;
        for (int i = 4; i < targetList.Length; i++)
        {
            tempBol = targetList[i] == 0 ? tempBol : false;
        }
        return (T)(object)tempBol;
    }
}
