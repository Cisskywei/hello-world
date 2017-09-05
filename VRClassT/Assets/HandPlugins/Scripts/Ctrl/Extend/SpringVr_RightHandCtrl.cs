using UnityEngine;
using System.Collections.Generic;

public class SpringVr_RightHandCtrl : SpringVr_HandActiveCtrlExtend
{
    #region 单例
    private static SpringVr_RightHandCtrl instace;
    public static SpringVr_RightHandCtrl Instace
    {
        get
        {
            return instace;
        }
    }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
        instace = this;
        ctrlHand = RightHandActiveView.gameObject;
        if (ctrlHand == null)
        {
            Destroy(this);
        }
    }
    protected override void OnStart()
    {
        base.OnStart();
        readDataScript = model.GetComponent<SpringVr_ReadRightHandData>();
    }

    // 只为测试
    public int[] GetAllFingerState()
    {
        return base.getAllFingerState();
    }
}
