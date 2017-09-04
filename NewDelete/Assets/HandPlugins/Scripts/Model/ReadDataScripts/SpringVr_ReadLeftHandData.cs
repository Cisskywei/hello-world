using UnityEngine;
using System.Collections;
using System.Threading;

public class SpringVr_ReadLeftHandData : SpringVr_ReadDataBase
{
    #region 单例
    private static SpringVr_ReadLeftHandData instance;
    public static SpringVr_ReadLeftHandData Instance
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
        if (GameObject.Find("LeftHand") == null)
        {
            Destroy(this);
        }
        m_strFrameHeadle = "FE81";
    }
}
