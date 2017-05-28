using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : uibase {

    public Dictionary<string, PlayerInfor> playerlist;
    public List<string> groupnamelist = null;

    public GameObject iconPrafab;

    public Transform playerlistpanel;

    public Dropdown grouplist;
    private string _currentname;
    private bool _isall = true;

    private string _name; // 选择的小组

    // Use this for initialization
    void Start () {
    }

    //// Update is called once per frame
    //void Update () {

    //}

    void OnEnable()
    {
        RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEventListener();
    }

    public void ChoosePerson(string token)
    {
        _name = token;
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string>(EventId.ChoosePerson, this.ChoosePerson);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string>(EventId.ChoosePerson, this.ChoosePerson);
    }

    // 提供外部调用显示接口
    public void ShowSelf(string groupname)
    {
        InitUI(groupname);

        if(_isall)
        {
            grouplist.gameObject.SetActive(false);
        }
        else
        {
            InitGroupShowList();
            grouplist.gameObject.SetActive(true);
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void InitUI(string groupname)
    {
        if(_isall)
        {
            InitAllList();
        }
        else
        {
            InitGroupList(groupname);
        }
    }

    public void InitAllList()
    {
        // 获取小组信息
        playerlist = UiDataManager.getInstance().GetAllPlayerList();

        int count = 0;
        int tip = 0;
        if (playerlistpanel != null)
        {
            count = playerlistpanel.childCount;
        }

        foreach (PlayerInfor g in playerlist.Values)
        {
            if (tip < count)
            {
                Transform icon = playerlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<PlayerIcon>().Init(g.name, g.GetDuty(), g.iconid);
                    if (!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                generateGroupIcon(g.name, g.GetDuty(), g.iconid);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                var icon = playerlistpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }
    }

    public void InitGroupList(string groupname)
    {
        // 获取小组信息
        playerlist = UiDataManager.getInstance().GetGroupMemeber(groupname);

        int count = 0;
        int tip = 0;
        if (playerlistpanel != null)
        {
            count = playerlistpanel.childCount;
        }

        foreach (PlayerInfor g in playerlist.Values)
        {
            if (tip < count)
            {
                Transform icon = playerlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<PlayerIcon>().Init(g.name, g.GetDuty(), g.iconid);
                    if (!icon.gameObject.activeSelf)
                    {
                        icon.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                generateGroupIcon(g.name, g.GetDuty(), g.iconid);
            }
            tip++;
        }

        if (tip < count)
        {
            for (int i = tip; i < count; i++)
            {
                var icon = playerlistpanel.GetChild(i).gameObject;
                if (icon.activeSelf)
                {
                    icon.SetActive(false);
                }
            }
        }
    }

    // 生成小组 icon
    private void generateGroupIcon(string name, string duty, int iconid)
    {
        if (iconPrafab == null)
        {
            return;
        }

        GameObject icon = GameObject.Instantiate(iconPrafab, playerlistpanel);
        icon.GetComponent<PlayerIcon>().Init(name, duty, iconid);
    }

    // 切换组
    public void SwitchGroup()
    {
        if(_currentname != grouplist.captionText.text)
        {
            _currentname = grouplist.captionText.text;
        }

        InitGroupList(_currentname);
    }

    // 切换小组和全部
    public void SwitchToAll(Toggle tog)
    {
        if(!tog.isOn)
        {
            return;
        }

        _isall = true;
        InitAllList();
    }

    // 切换小组和全部
    public void SwitchToGroup(Toggle tog)
    {
        if (!tog.isOn)
        {
            return;
        }

        _isall = false;
        InitGroupList(_currentname);
    }

    // 初始化小组列表
    private void InitGroupShowList()
    {
        if(groupnamelist != null && groupnamelist.Count > 0)
        {
            return;
        }

        groupnamelist = UiDataManager.getInstance().GetGroupNameList();

        // 初始化界面显示
        grouplist.options.Clear();

        Dropdown.OptionData tempdate;
        foreach(string name in groupnamelist)
        {
            tempdate = new Dropdown.OptionData();
            tempdate.text = name;
            grouplist.options.Add(tempdate);
        }

        if(groupnamelist.Count>0)
        {
            grouplist.captionText.text = groupnamelist[0];
        }
    }

    // 确认返回
    public void Yes()
    {
        UiDataManager.getInstance().ChoosePerson(this._name);
    }

    public void No()
    {
        HideSelf();
    }
}
