using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责处理角色相关 角色生成 销毁  同步  以及如换装 等指令
/// </summary>
public class RoleManager {

    public static RoleManager getInstance()
    {
        return Singleton<RoleManager>.getInstance();
    }

    public RoleManager()
    {
        playerPrefab = Resources.Load(rolepath + rolename);
    }

    // 除了自己的其他玩家列表
    private Dictionary<int, SyncPlayer> _allplayers = new Dictionary<int, SyncPlayer>();
    // 人物角色的整体预设
    public static string rolepath = "Role/";
    private string rolename = "role_man";
    public static UnityEngine.Object playerPrefab;
    public int selfplayerid = -1;

    // 被销毁的人物缓存
    private List<SyncPlayer> _cacheplayers = new List<SyncPlayer>();

    // 接收服务器的数据  t [ key : userid , value : hashtable(head,lefthand,righthand) ]
    public void RecivePlayerInfor(Hashtable data)
    {
        if (data == null || data.Count <= 0)
        {
            return;
        }

        try
        {
            int userid = -1;
            foreach (DictionaryEntry de in data)
            {
                userid = Convert.ToInt32(de.Key);

                if (userid == selfplayerid)
                {
                    continue;
                }

                if (_allplayers.ContainsKey(userid))
                {
                    _allplayers[userid].ReceiveSync((Hashtable)de.Value);
                }
                else
                {
                    GeneratePlayer(userid, (Hashtable)de.Value);
                }
            }

        }
        catch
        {

        }
    }

    public void AddPlayer(int userid, SyncPlayer sp)
    {
        if(_allplayers.Count <= 0 || _allplayers.ContainsKey(userid))
        {
            _allplayers[userid] = sp;
        }
        else
        {
            _allplayers.Add(userid, sp);
        }
    }

    private void GeneratePlayer(int userid, Hashtable data)
    {
        if(playerPrefab == null)
        {
            return;
        }

        SyncPlayer sp = null;
        if (_cacheplayers.Count > 0)
        {
            sp = _cacheplayers[0];
            _cacheplayers.RemoveAt(0);
            sp.gameObject.SetActive(true);
        }
        else
        {
            GameObject go = GameObject.Instantiate(playerPrefab) as GameObject;
            sp = go.GetComponent<SyncPlayer>();
        }

        if(sp != null)
        {
            sp.Init(userid);
            sp.ReceiveSync(data);

            _allplayers.Add(userid, sp);
        }
    }

    private void DeletePlayer(int userid)
    {
        if(_allplayers == null || _allplayers.Count <= 0)
        {
            return;
        }

        if(_allplayers.ContainsKey(userid))
        {
            SyncPlayer sp = _allplayers[userid];
            _allplayers.Remove(userid);

            sp.gameObject.SetActive(false);
            _cacheplayers.Add(sp);

     //       GameObject.Destroy(sp.gameObject);
        }
    }
}
