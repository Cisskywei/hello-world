using UnityEngine;
using System.Collections.Generic;

public class SpringVr_GestureChoose
{
    private SpringVr_GestureCalculateBase gestureCalculate;
    private SpringVr_GestureCalculateBase.ForearmKinestate tempStage;
    public SpringVr_GestureChoose(SpringVr_GestureCalculateBase targetScript)
    {
        gestureCalculate = targetScript;
    }
    public T GestureCalculate<T>(int[] targetList, List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        if (targetList == null)
        {
            return (T)(object)false;
        }
        return gestureCalculate.GestureCalculate<T>(targetList, allJoint, lastGesture);
    }
}
