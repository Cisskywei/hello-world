using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

/// <summary>
/// 选择小组界面
/// </summary>
public class GroupListUI : uibase {

    public Dictionary<string, GroupInfor> grouplist;

    // ui 控件相关
    public Transform grouplistpanel;

    public GameObject iconPrafab;

    private string _name;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

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
    public void ShowSelf()
    {
        InitUI();

        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void HideSelf()
    {
        if(gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void InitUI()
    {
        // 获取小组信息
        grouplist = UiDataManager.getInstance().GetGroupList();

        int count = 0;
        int tip = 0;
        if(grouplistpanel != null)
        {
            count = grouplistpanel.childCount;
        }

        foreach(GroupInfor g in grouplist.Values)
        {
            if(tip < count)
            {
                Transform icon = grouplistpanel.GetChild(tip);
                if(icon)
                {
                    icon.GetComponent<GroupIcon>().Init(g.name, g.count);
                    if(!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                generateGroupIcon(g.name, g.count);
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

    // 生成小组 icon
    private void generateGroupIcon(string name, int num)
    {
        if(iconPrafab == null)
        {
            return;
        }

        GameObject icon = GameObject.Instantiate(iconPrafab,grouplistpanel);
        icon.GetComponent<GroupIcon>().Init(name, num);
    }

    // 确认返回
    public void Yes()
    {
        UiDataManager.getInstance().ChooseGroup(this._name);
    }

    public void No()
    {
        HideSelf();
    }
}
