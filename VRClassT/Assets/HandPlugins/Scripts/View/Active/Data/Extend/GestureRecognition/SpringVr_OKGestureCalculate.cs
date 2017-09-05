using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_OKGestureCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = true;
        tempBol = targetList[4] == 2 ? tempBol : false;
        if (tempBol)
        {
            for (int i = 5; i < targetList.Length; i++)
            {
                tempBol = targetList[i] == 0 ? tempBol : false;
            }
        }
        tempBol = targetList[3] == 2 ? tempBol : false;

        return (T)(object)tempBol;
    }
}
