using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class GroupInfor : PlayerBaseInfor
{

    public GroupInfor()
    {

    }

    public GroupInfor(string name, Enums.SexEnum sex = Enums.SexEnum.None, Enums.DutyEnum duty = Enums.DutyEnum.None)
    {
        this.name = name;
        this.sex = sex;
        this.duty = duty;
    }

    public void AddMember(PlayerInfor p)
    {
        if(p == null || p.token == null)
        {
            return;
        }

        if(members.ContainsKey(p.token))
        {
            members[p.token] = p;
        }
        else
        {
            members.Add(p.token, p);
        }
    }

    // 被选中 发送 通知消息
    public void OnClick()
    {
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<string>(EventId.ChooseGroup, this.token);
    }

    public Dictionary<string, PlayerInfor> members = new Dictionary<string, PlayerInfor>();
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
