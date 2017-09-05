using UnityEngine;
using System.Collections.Generic;

public class SpringVr_ThumbGestureCalculate : SpringVr_GestureCalculateBase
{
    private int lastState;
    public override T GestureCalculate<T>(int[] targetList, List<Transform> allJoint, SpringVr_HandActiveCtrlExtend.AllGesture lastGesture)
    {
        if (lastState == targetList[3])
        {
            return (T)(object)ForearmKinestate.Static;
        }
        switch (targetList[3])
        {
            case 0:
                currentStage = ForearmKinestate.Entad;
                break;
            case 1:
                currentStage = ForearmKinestate.OutWord;
                break;
            default:
                currentStage = ForearmKinestate.Static;
                break;
        }
        lastState = targetList[3];
        return (T)(object)currentStage;
    }
}
