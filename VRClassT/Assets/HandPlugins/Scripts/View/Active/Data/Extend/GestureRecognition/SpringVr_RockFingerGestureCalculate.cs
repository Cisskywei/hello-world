using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_RockFingerGestureCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = true;
        tempBol = targetList[4] == 0 ? tempBol : false;
        tempBol = targetList[7] == 0 ? tempBol : false;
        if (tempBol)
        {
            for (int i = 5; i < targetList.Length-1; i++)
            {
                tempBol = targetList[i] == 2 ? tempBol : false;
            }
        }
        return (T)(object)tempBol;
    }
}
