using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_GetGoodsGestureCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = false;
        for (int i = 4; i < targetList.Length; i++)
        {
            tempBol = targetList[i] == 1  || targetList[i] == 2 ? true : tempBol;
        }
        if (tempBol)
        {
            //Debug.Log("正在抓东西的手势");
        }
        return (T)(object)tempBol;
    }

}
