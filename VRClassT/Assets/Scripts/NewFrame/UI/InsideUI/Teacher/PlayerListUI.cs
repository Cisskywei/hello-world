using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListUI : OutUIBase {

    public Dictionary<int, PlayerInfor> playerlist;
    public List<string> groupnamelist = null;

    public Toggle[] switchgroupall; // 0 all 1 group

    public GameObject iconPrafab;

    public Transform playerlistpanel;

    public Text titleinfor;
    private string _titleinfor = "请选择您需要指导的学生:";

    public Dropdown grouplist;
    private string _currentname;
    private bool _isall = true;

    private string _groupname; // 选择的小组
    private Int64 _personid; // 选择的人

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

    public void ChoosePerson(Int64 userid)
    {
        _personid = userid;
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.ChoosePerson, this.ChoosePerson);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.ChoosePerson, this.ChoosePerson);
    }

    public override void ShowSelf(params System.Object[] args)
    {
        ShowSelf(null, null);
    }

    // 提供外部调用显示接口
    public void ShowSelf(string groupname = null, string infor = null)
    {
        ChangeTitleInfor(infor);

        _isall = true;

        if(switchgroupall != null && switchgroupall.Length > 1)
        {
            switchgroupall[0].isOn = true;
            switchgroupall[1].isOn = false;
        }

        InitUI(groupname);

        if (_isall)
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

    public override void HideSelf()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    public void ChangeTitleInfor(string infor)
    {
        if(titleinfor == null)
        {
            return;
        }

        if(infor == null)
        {
            infor = _titleinfor;
        }

        titleinfor.text = infor;
    }

    public void InitUI(string groupname = null)
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
        playerlist = ClassManager.getInstance().GetAllPlayers();

        int count = 0;
        int tip = 0;
        if (playerlistpanel != null)
        {
            count = playerlistpanel.childCount;
        }

        if(playerlist == null || playerlist.Count <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                playerlistpanel.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        ToggleGroup tog = playerlistpanel.GetComponent<ToggleGroup>();

        foreach (PlayerInfor g in playerlist.Values)
        {
            if (tip < count)
            {
                Transform icon = playerlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<PlayerIcon>().Init(g, tog);
                    if (!icon.gameObject.activeSelf)
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

                GameObject icon = GameObject.Instantiate(iconPrafab, playerlistpanel);
                icon.GetComponent<PlayerIcon>().Init(g, tog);
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

    public void InitGroupList(string groupname = null)
    {
        if(groupname == null)
        {
            groupname = "组1";
        }
        // 获取小组信息
        playerlist = ClassManager.getInstance().GetMembersOfGroup(groupname);

        int count = 0;
        int tip = 0;
        if (playerlistpanel != null)
        {
            count = playerlistpanel.childCount;
        }

        if (playerlist == null || playerlist.Count <= 0)
        {
            for(int i=0;i<count;i++)
            {
                playerlistpanel.GetChild(i).gameObject.SetActive(false);
            }
            return;
        }

        ToggleGroup tog = playerlistpanel.GetComponent<ToggleGroup>();

        foreach (PlayerInfor g in playerlist.Values)
        {
            if (tip < count)
            {
                Transform icon = playerlistpanel.GetChild(tip);
                if (icon)
                {
                    icon.GetComponent<PlayerIcon>().Init(g.name, g.GetDuty(), g.iconid, g.userid, tog);
                    if (!icon.gameObject.activeSelf)
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

                GameObject icon = GameObject.Instantiate(iconPrafab, playerlistpanel);
                icon.GetComponent<PlayerIcon>().Init(g.name, g.GetDuty(), g.iconid, g.userid, tog);
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

        if (_isall)
        {
            grouplist.gameObject.SetActive(false);
        }
        else
        {
            grouplist.gameObject.SetActive(true);
        }

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

        if (_isall)
        {
            grouplist.gameObject.SetActive(false);
        }
        else
        {
            grouplist.gameObject.SetActive(true);
        }

        InitGroupShowList();
        InitGroupList(_currentname);
    }

    // 初始化小组列表
    private bool isinitgroupshowlist = false;
    private void InitGroupShowList()
    {
        if(isinitgroupshowlist)
        {
            return;
        }

        groupnamelist = UiDataManager.getInstance().GetGroupNameList();

        if (groupnamelist != null && groupnamelist.Count > 0)
        {
            Debug.Log("groupnamelist +  是空的");
            return;
        }

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

        isinitgroupshowlist = true;

        Debug.Log("初始化小组选择列表");
    }

    // 确认返回
    public void Yes()
    {
        UiDataManager.getInstance().ChoosePerson(this._personid);
        HideSelf();

        TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
    }

    public void No()
    {
        HideSelf();

        TeacherUI.getInstance().ShowUILeft(TeacherUI.UILeft.First);
    }
}
