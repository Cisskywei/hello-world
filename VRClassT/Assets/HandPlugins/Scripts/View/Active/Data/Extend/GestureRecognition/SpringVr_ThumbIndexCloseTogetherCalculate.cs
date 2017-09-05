using System;
using System.Collections.Generic;
using UnityEngine;


public class SpringVr_ThumbIndexCloseTogetherCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList, List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        if (lastGesture == SpringVr_HandActiveCtrlExtend.AllGesture.ThumbIndexSeparate)
        {
            tempBol = true;
        }
        else
        {
            tempBol = false;
        }
        if (tempBol)
        {
            tempBol = targetList[4] == 0 ? tempBol : false;
            for (int i = 5; i < targetList.Length; i++)
            {
                tempBol = targetList[i] == 2 ? tempBol : false;
            }
            tempBol = targetList[3] == 0 ? tempBol : false;
        }
        return (T)(object)tempBol;
    }
}