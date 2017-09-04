using UnityEngine;
using System.Collections.Generic;
using System;

public class SpringVr_RightHandActiveView : SpringVr_HandActiveView
{
    protected override void OnAwake()
    {
        base.OnAwake();
        bigArm = transform.GetChild(0);
        handDistinguish = HandDistinguish.RightHand;
    }
    protected override float EightThumb3JointOffset()
    {
        return -allHandFirstJoint[0].localEulerAngles.x + 20;
    }
    protected override Quaternion SevenThumb3JointOffset(float targetParma)
    {
        return Thumb3JointOffset(targetParma + 40);
    }
}
