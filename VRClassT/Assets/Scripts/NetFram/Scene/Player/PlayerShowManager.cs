using System;
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

    // 人物模型预设
    public GameObject playerprefab;

    // 根据token保存的玩家数据 GameObject 后期可以替换成 绑定在 物体上的脚本
    private Dictionary<int, ScenePlayer> otherplayer = new Dictionary<int, ScenePlayer>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
	//}

    // 接收服务器的数据  t [ key : userid , value : hashtable(head,lefthand,righthand) ]
    public void ReciveOtherPlayerInfor(Hashtable t)
    {
        if (t == null || t.Count <= 0)
        {
            return;
        }

        try
        {
            int userid;
            int selfid = (int)UserInfor.getInstance().UserId;
            foreach (DictionaryEntry de in t)
            {
                userid = Convert.ToInt32(de.Key);

                if(userid == selfid)
                {
                    continue;
                }

                if(otherplayer.ContainsKey(userid))
                {
                    otherplayer[userid].HeadHandTransform((Hashtable)de.Value);
                }
                else
                {
                    // 生成新的人物
                    Debug.LogError("生成新人物");
                    if (playerprefab != null)
                    {
                        GameObject go = Instantiate(playerprefab);
                        ScenePlayer sp = go.GetComponent<ScenePlayer>();
                        sp.HeadHandTransform((Hashtable)de.Value);

                        otherplayer.Add(userid, sp);
                    }
                }
            }

        }
        catch
        {

        }
    }

    // 清理其他玩家
    public void ClearOtherPlayer()
    {

    }

    public void ClearOtherPlayer(Int64 id)
    {

    }
}
