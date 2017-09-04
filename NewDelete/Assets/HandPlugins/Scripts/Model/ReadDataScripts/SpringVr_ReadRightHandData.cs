using UnityEngine;
using System.Collections;
using System.Threading;

public class SpringVr_ReadRightHandData : SpringVr_ReadDataBase
{
    #region 单例
    private static SpringVr_ReadRightHandData instance;
    public static SpringVr_ReadRightHandData Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
    protected override void OnAwake()
    {
        base.OnAwake();
        instance = this;
        if (GameObject.Find("RightHand") == null)
        {
            Destroy(this);
        }
        m_strFrameHeadle = "FE80";
    }
}
