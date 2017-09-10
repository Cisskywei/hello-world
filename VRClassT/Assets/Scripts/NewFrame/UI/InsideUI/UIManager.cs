using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 内部界面管理 负责老师、学生界面显示选择
/// </summary>
public class UIManager : MonoBehaviour {

    private static GameObject selfgo;
    private static UIManager _instance;
    public static UIManager getInstance()
    {
        if(_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("UIManager");
            }

            if(selfgo != null)
            {
                _instance = selfgo.GetComponent<UIManager>();
            }
        }

        return _instance;
    }

    public enum UIType
    {
        None = -1,

        Teacher,
        Student,
    }

    public OutUIBase[] uilist;

    public ComonEnums.DutyEnum duty
    {
        get
        {
            return UserInfor.getInstance().UserDuty;
        }

        set
        {
            InitUIByDuty();
        }
    }

    // 信息
    public MsgTips msgShow;

    public GameObject uiroot;   // 界面根目录

    ///    ui 界面预设
    // 消息界面
    public MessageWindow msg;
    // 答题选择界面
    public GameObject classtest;
    // 重置场景
    public GameObject resetscene;
    // 画笔按钮
    public GameObject palettebtn;
    // 返回大厅按钮
    public GameObject back;
    // 小组信息界面
    public GroupListUI groups;
    // 成员信息界面
    public PlayerListUI players;
    // 学生举手界面
    public StudentHandUp handuplist;
    // 测试问题
    public TestInClassUI question;
    //测试问题反馈
    public TestFeedBackUI questionback;
    // 抢答
    public FastInClassUI fastquestion;
    // 当前状态界面
    public GameObject stateinfor;
    // 老师tips状态界面
    public StateInforTeacher stateinforteacher;
    // 模式切换界面
    public SwitchModeUI mode;
    // 有疑问的图标
    public GameObject flashing;

    // 模式文字
    public string modeTxt = "观学模式-同步";

    // 学生端
    public StudentStateInfor studentstate;
    public StudentTest studenttest;
    public StudentFast studentfast;
    public StudentFastGet studentfastget;

    public void ShowSelf()
    {
        InitUIByDuty();

        if (!uiroot.activeSelf)
        {
            uiroot.SetActive(true);
        }
    }

    public void HideSelf()
    {
        if(uiroot.activeSelf)
        {
            uiroot.SetActive(false);
        }
    }

    public void ShowUI(UIType id, params System.Object[] args)
    {
        if (uilist == null || uilist.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > uilist.Length)
        {
            return;
        }

        for (int i = 0; i < uilist.Length; i++)
        {
            if (i == uiid)
            {
                uilist[i].ShowSelf(args);
            }
            else
            {
                uilist[i].HideSelf();
            }
        }
    }

    private void InitUIByDuty()
    {
        if (duty == ComonEnums.DutyEnum.Teacher || duty == ComonEnums.DutyEnum.BigScreen)
        {
            // 显示老师相关界面
            ShowUI(UIType.Teacher);
        }
        else
        {
            // 显示学生相关界面
            ShowUI(UIType.Student);
        }
    }

    void OnEnable()
    {
        RegisterEvent();
    }

    void OnDisable()
    {
        UnRegisterEvent();
    }

    public void RegisterEvent()
    {
        //EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.OpenUI, this.OpenUI);
    }

    public void UnRegisterEvent()
    {
        //EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.OpenUI, this.OpenUI);
    }
}
