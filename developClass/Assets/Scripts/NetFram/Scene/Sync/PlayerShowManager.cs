using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShowManager : MonoBehaviour {

    private static PlayerShowManager _instance;
    public static PlayerShowManager getInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.Find("SyncObjectOnce").GetComponent<PlayerShowManager>();
        }

        return _instance;
    }

    // 根据token保存的玩家数据 GameObject 后期可以替换成 绑定在 物体上的脚本
    private Dictionary<string, GameObject> otherplayer = new Dictionary<string, GameObject>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 接收服务器的数据
    public void ReciveOtherPlayerInfor(Hashtable t)
    {

    }

    // 清理其他玩家
    public void ClearOtherPlayer()
    {

    }

    public void ClearOtherPlayer(string token)
    {

    }
}
