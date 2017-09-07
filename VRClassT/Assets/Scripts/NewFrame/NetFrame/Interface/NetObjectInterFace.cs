using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetObjectInterFace {

    /// <summary>
    /// 处理网络游戏物体操作请求返回
    /// </summary>
	public interface IObjectOperate
    {
        void ObjectOperate(int userid, ArrayList msg);
        //void RetHoldRelease(int userid, ArrayList msg);
    }
}
