using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerInterFace {

    /// <summary>
    /// 处理网络游戏物体操作请求返回
    /// </summary>
    public interface IPlayerOrder
    {
        void PlayerOrder(int userid, ArrayList msg);
    }

    /// <summary>
    /// 处理网络游戏物体同步
    /// </summary>
	public interface IPlayerSync
    {
        void ReceiveSync(Hashtable data);
        void DoSync();
        void SendSync();
        //Hashtable GetHash();
    }
}
