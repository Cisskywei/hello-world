using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpringVr;
using System;

public class SpringVr_LeftHandActiveView : SpringVr_HandActiveView
{
    protected override void OnAwake()
    {
        base.OnAwake();
        bigArm = transform.GetChild(0);
        handDistinguish = HandDistinguish.LeftHand;
    }
    protected override float EightThumb3JointOffset()
    {
        return allHandFirstJoint[0].localEulerAngles.x;
    }
    protected override Quaternion SevenThumb3JointOffset(float targetParame)
    {
        return Thumb3JointOffset(targetParame + 20);
    }
}
