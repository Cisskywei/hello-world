using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择小组界面
/// </summary>
public class GroupListUI : OutUIBase {

    public Dictionary<string, GroupInfor> grouplist;

    // ui 控件相关
    public Transform grouplistpanel;

    public GameObject iconPrafab;

    private string _name;

    void OnEnable()
    {
        RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEventListener();
    }

    public void ChooseGroup(string token)
    {
        this._name = token;
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string>(EventId.ChooseGroup, this.ChooseGroup);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string>(EventId.ChooseGroup, this.ChooseGroup);
    }

    // 提供外部调用显示接口
    public override void ShowSelf(params object[] args)
    {
        InitUI();

        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void InitUI()
    {
        // 获取小组信息
        grouplist = ClassManager.getInstance().GetAllGroups();

        int count = 0;
        int tip = 0;
        if(grouplistpanel != null)
        {
            count = grouplistpanel.childCount;
        }

        if(grouplist == null || grouplist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                grouplistpanel.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        ToggleGroup tog = grouplistpanel.GetComponent<ToggleGroup>();

        foreach (GroupInfor g in grouplist.Values)
        {
            if(tip < count)
            {
                Transform icon = grouplistpanel.GetChild(tip);
                if(icon)
                {
                    icon.GetComponent<GroupIcon>().Init(g.name, g.count, tog);
                    if(!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (iconPrafab == null)
                {
                    return;
                }

                GameObject icon = GameObject.Instantiate(iconPrafab, grouplistpanel);
                icon.GetComponent<GroupIcon>().Init(g.name, g.count, tog);
            }
            tip++;
        }

        if(tip < count)
        {
            for(int i=tip; i<count;i++)
            {
                var icon = grouplistpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }
    }

    // 确认返回
    public void Yes()
    {
        UiDataManager.getInstance().ChooseGroup(this._name);
        HideSelf();
        TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
    }

    public void No()
    {
        HideSelf();
        TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
    }
}
