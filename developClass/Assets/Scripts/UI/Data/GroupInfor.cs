using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class GroupInfor : PlayerBaseInfor
{

    public GroupInfor()
    {

    }

    public GroupInfor(string name, Enums.SexEnum sex, Enums.DutyEnum duty)
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

    public string token; // 用于服务器识别
    public Dictionary<string, PlayerInfor> members = new Dictionary<string, PlayerInfor>();
    public int count;
}
