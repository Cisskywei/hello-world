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

    /// <summary>
    /// 处理网络游戏物体同步
    /// </summary>
	public interface IObjectSync
    {
        void ReceiveSync(Hashtable data);
        void DoSync();
        void SendSync();
        Hashtable GetHash();
    }

    /// <summary>
    /// 处理网络游戏物体管道方式的同步
    /// </summary>
	public interface IObjectPipeSync
    {
        void ReceiveSync(int fromid, ArrayList data);
        void SendSync(System.Object data);
    }
}
