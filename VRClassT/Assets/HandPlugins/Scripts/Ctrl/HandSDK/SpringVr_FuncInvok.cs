using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpringVr_FuncInvok{


    #region 成员变量
    public static SpringVr_AllUseData allUseData;

    #endregion

    #region 函数

    #region 初始化函数

    static SpringVr_FuncInvok()
    {
        allUseData = GameObject.Find("HandSDK").GetComponent<SpringVr_AllUseData>();
    }
    #endregion

    #endregion


}
