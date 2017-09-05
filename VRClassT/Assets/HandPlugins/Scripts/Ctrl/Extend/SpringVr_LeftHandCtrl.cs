using UnityEngine;
using System.Collections.Generic;

public class SpringVr_LeftHandCtrl : SpringVr_HandActiveCtrlExtend
{
    private static SpringVr_LeftHandCtrl instance;
    public static SpringVr_LeftHandCtrl Instace
    {
        get
        {
            return instance;
        }
    }
    protected override void OnAwake()
    {
        base.OnAwake();
        instance = this;
        ctrlHand = LeftHandActiveView.gameObject;
        if (ctrlHand == null)
        {
            Destroy(this);
        }
    }
    protected override void OnStart()
    {
        base.OnStart();
        readDataScript = model.GetComponent<SpringVr_ReadLeftHandData>();
    }

    // 只为测试
    public int[] GetAllFingerState()
    {
        return base.getAllFingerState();
    }
}
