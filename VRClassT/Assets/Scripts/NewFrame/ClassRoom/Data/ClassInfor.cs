using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassInfor {

    public Dictionary<int, PlayerInfor> playerlist = new Dictionary<int, PlayerInfor>(); // 所有学生列表
    public int Count
    {
        get
        {
            return playerlist.Count;
        }
    }

    public string classname = "班级1";
    public int classid = 1;

    public void InitClass(int classid, string classname = null)
    {
        this.classid = classid;

        if(classname == null)
        {
            this.classname = "班级" + classid;
        }
        else
        {
            this.classname = classname;
        }
    }

    public PlayerInfor FindPlayerById(int userid)
    {
        if(playerlist == null || playerlist.Count <= 0 || !playerlist.ContainsKey(userid))
        {
            return null;
        }

        return playerlist[userid];
    }

    public void AddStudent(PlayerInfor player)
    {
        if(playerlist.ContainsKey(player.userid))
        {
            playerlist[player.userid] = player;
        }
        else
        {
            playerlist.Add(player.userid, player);
        }
    }

    public void Playeronline(int userid)
    {
        PlayerInfor pi = FindPlayerById(userid);
        if(pi == null)
        {
            return;
        }

        pi.isonline = true;
    }

    public void Playersonline(ArrayList userids)
    {
        for(int i=0;i<userids.Count;i++)
        {
            Int64 id = (Int64)userids[i];
            PlayerInfor pi = FindPlayerById((int)id);
            if (pi == null)
            {
                return;
            }

            pi.isonline = true;
        }
    }

    public void GetAllPlayers(ref Dictionary<int, PlayerInfor> players)
    {
        if(players == null)
        {
            players = new Dictionary<int, PlayerInfor>();
        }

        foreach(KeyValuePair<int,PlayerInfor> pi in playerlist)
        {
            players.Add((int)pi.Key, (PlayerInfor)pi.Value);
        }
    }
}
