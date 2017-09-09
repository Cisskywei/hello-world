using ko.NetFram;
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
}
