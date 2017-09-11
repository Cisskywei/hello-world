using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class GroupInfor : PlayerBaseInfor
{

    public GroupInfor()
    {

    }

    public GroupInfor(string name, ComonEnums.SexEnum sex = ComonEnums.SexEnum.None, ComonEnums.DutyEnum duty = ComonEnums.DutyEnum.None)
    {
        this.name = name;
        this.sex = sex;
        this.duty = duty;
    }

    public void AddMember(PlayerInfor p)
    {
        if(p == null)
        {
            return;
        }

        if(members.ContainsKey(p.userid))
        {
            members[p.userid] = p;
        }
        else
        {
            members.Add(p.userid, p);
        }
    }

    // 被选中 发送 通知消息
    public void OnClick()
    {
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<string>(EventId.ChooseGroup, this.token);
    }

    public Dictionary<int, PlayerInfor> members = new Dictionary<int, PlayerInfor>();
    private int _count;
    public int count
    {
        get
        {
            return members.Count;
        }
        set
        {
            _count = value;
        }
    }
}
