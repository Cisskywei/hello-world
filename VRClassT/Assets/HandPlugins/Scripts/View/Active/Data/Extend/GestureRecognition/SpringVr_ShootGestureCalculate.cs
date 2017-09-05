using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 食指伸开手势检测
/// </summary>
public class SpringVr_ShootGestureCalculate : SpringVr_GestureCalculateBase
{
    public override T GestureCalculate<T>(int[] targetList,List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        tempBol = true;
        tempBol = targetList[4] == 0 ? tempBol : false;
        if (tempBol)
        {
            for (int i = 5; i < targetList.Length; i++)
            {
                tempBol = targetList[i] == 2 ? tempBol : false;
            }
        }
        return (T)(object)tempBol;
    }
}
