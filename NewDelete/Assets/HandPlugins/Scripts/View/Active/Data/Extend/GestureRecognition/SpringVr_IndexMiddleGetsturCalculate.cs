using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpringVr_IndexMiddleGetsturCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = true;
        tempBol = targetList[4] == 0 ? tempBol : false;
        tempBol = targetList[5] == 0 ? tempBol : false;
        if (tempBol)
        {
            for (int i = 6; i < targetList.Length - 1; i++)
            {
                tempBol = targetList[i] == 2? tempBol : false;
            }
        }
        return (T)(object)tempBol;
    }


}
