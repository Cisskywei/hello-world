using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

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

    private bool _isTeacher = false; // 标记是否是老师 用于控制界面的显示 隐藏
    public bool IsTeacher
    {
        get
        {
            return UserInfor.getInstance().isTeacher;
        }

        set
        {
            _isTeacher = value;

            InitUIByDuty();
        }
    }

    ///    ui 界面预设
    // 消息界面
    public MessageWindow msg;
    // 答题选择界面
    public GameObject classtest;
    // 重置场景
    public GameObject resetscene;
    // 学生端重置场景    自主训练模式 下可以使用
    public GameObject resetscene2;
    // 返回大厅按钮
    public GameObject back;
    // 小组信息界面
    public GroupListUI groups;
    // 成员信息界面
    public PlayerListUI players;
    // 测试问题
    public TestInClassUI question;
    //测试问题反馈
    public TestFeedBackUI questionback;
    // 抢答
    public FastInClassUI fastquestion;
    // 当前状态界面
    public GameObject stateinfor;
    // 当前状态学生端
    public GameObject stateinfor2;
    // 模式切换界面
    public SwitchModeUI mode;
    // 有疑问的图标
    public GameObject flashing;
    // 有疑问的按钮
    public GameObject feedbackbtn;
    // 点赞的按钮
    public GameObject likebtn;

    private bool _isInitVisible = false; //标记是否依据学生老师进行设置显示与否

    // Use this for initialization
    void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void ShowSelf()
    {

        InitUIByDuty();

        if (!gameObject.activeSelf)
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

    private void InitUIByDuty()
    {
        if(_isInitVisible)
        {
            return;
        }

        if(IsTeacher)
        {
            // 显示老师相关界面
            if(msg != null)
            {
                msg.gameObject.SetActive(true);
            }

            if(classtest!=null)
            {
                classtest.SetActive(true);
            }

            if (resetscene != null)
            {
                resetscene.SetActive(true);
            }

            if(back!=null)
            {
                back.SetActive(true);
            }

            if(stateinfor!=null)
            {
                stateinfor.SetActive(true);
            }

            if(mode!=null)
            {
                mode.gameObject.SetActive(true);
            }
        }
        else
        {
            // 显示学生相关界面
            if (msg != null)
            {
                msg.gameObject.SetActive(true);
            }

            if (resetscene2 != null)
            {
                resetscene2.SetActive(true);
            }

            if (stateinfor2 != null)
            {
                stateinfor2.SetActive(true);
            }

            if (feedbackbtn != null)
            {
                feedbackbtn.SetActive(true);
            }

            if (likebtn != null)
            {
                likebtn.SetActive(true);
            }
        }

        _isInitVisible = true;
    }

    void OnEnable()
    {
        RegisterEvent();
        UiDataManager.getInstance().RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEvent();
        UiDataManager.getInstance().UnRegisterEventListener();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string, Enums.TeachingMode, string>(EventId.SwitchModeFeedBack, this.SwitchModeFeedBack);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Enums.InClassTestType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string>(EventId.LikeFeedBack, this.ReceiveLike);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string, Enums.TeachingMode, string>(EventId.SwitchModeFeedBack, this.SwitchModeFeedBack);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Enums.InClassTestType,int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string>(EventId.LikeFeedBack, this.ReceiveLike);
    }

    // 监听函数
    public void SwitchMode(Enums.TeachingMode mode)
    {
        if(!IsTeacher)
        {
            return;
        }

        // 判断弹出对应界面
        switch (mode)
        {
            case Enums.TeachingMode.GuidanceMode_Personal:
                //弹出学生列表界面
                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();

                break;
            case Enums.TeachingMode.GuidanceMode_Group:
                //弹出小组选择界面
                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();

                break;
            case Enums.TeachingMode.SelfTrain_Personal:
                //弹出学生列表界面
                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();

                break;
            case Enums.TeachingMode.SelfTrain_Group:
                //弹出小组选择界面
                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();

                break;
            case Enums.TeachingMode.WatchLearnModel_Sync:
            case Enums.TeachingMode.WatchLearnModel_Async:
            case Enums.TeachingMode.SelfTrain_All:
            case Enums.TeachingMode.VideoOnDemand_General:
            case Enums.TeachingMode.VideoOnDemand_Full:
            case Enums.TeachingMode.VideoOnLive_General:
            case Enums.TeachingMode.VideoOnLive_Full:

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();

                break;
            default:
                break;
        }
    }

    // 切换模式返回 更新界面状态
    public void SwitchModeFeedBack(string token, Enums.TeachingMode mode, string target)
    {
        // TODO
        Debug.Log("模式切换 开始更改 界面显示 状态" + token + " -- " + mode + " --- " + target);

        string modeTxt = string.Empty;
        string who = string.Empty;
        PlayerInfor pi = null;
        GroupInfor gi = null;

        // 判断弹出对应界面
        switch (mode)
        {
            case Enums.TeachingMode.WatchLearnModel_Sync:
                modeTxt = "观学模式-同步";
                break;
            case Enums.TeachingMode.WatchLearnModel_Async:
                modeTxt = "观学模式-异步";
                break;
            case Enums.TeachingMode.GuidanceMode_Personal:
                pi = UiDataManager.getInstance().GetPlayerByToken(target);
                if(pi != null)
                {
                    who = pi.name;
                }
                modeTxt = "指导模式-指导人 (" + who + ")";
                break;
            case Enums.TeachingMode.GuidanceMode_Group:
                gi = UiDataManager.getInstance().GetGroupByToken(target);
                if (gi != null)
                {
                    who = gi.name;
                }
                modeTxt = "指导模式-指导组 (" + who + ")";
                break;
            case Enums.TeachingMode.SelfTrain_Personal:
                pi = UiDataManager.getInstance().GetPlayerByToken(target);
                if (pi != null)
                {
                    who = pi.name;
                }
                modeTxt = "自主训练模式-独立 (" + who + ")";
                break;
            case Enums.TeachingMode.SelfTrain_Group:
                gi = UiDataManager.getInstance().GetGroupByToken(target);
                if (gi != null)
                {
                    who = gi.name;
                }
                modeTxt = "自主训练模式-小组 (" + who + ")";
                break;
            case Enums.TeachingMode.SelfTrain_All:
                gi = UiDataManager.getInstance().GetGroupByToken(target);
                if (gi != null)
                {
                    who = gi.name;
                }
                modeTxt = "自主训练模式-全部";
                break;
            case Enums.TeachingMode.VideoOnDemand_General:
                modeTxt = "视频点播-普通";
                break;
            case Enums.TeachingMode.VideoOnDemand_Full:
                modeTxt = "视频点播-全景";
                break;
            case Enums.TeachingMode.VideoOnLive_General:
                modeTxt = "视频直播-普通";
                break;
            case Enums.TeachingMode.VideoOnLive_Full:
                modeTxt = "视频直播-全景";
                break;
            default:
                break;
        }

        if(IsTeacher)
        {
            // 老师界面 状态改变 
            stateinfor.GetComponent<StateInforShow>().SetModeText(modeTxt);
        }
        else
        {
            // 学生状态改变 
            stateinfor2.GetComponent<StateInforShow>().SetModeText(modeTxt);
        }
    }

    // 选择了某个试题对应的弹出相应界面
    public void ChooseQuestion(Enums.InClassTestType typ, int questionid)
    {
        if (!IsTeacher)
        {
            return;
        }

        switch (typ)
        {
            case Enums.InClassTestType.Test:
                //测试反馈
                questionback.ShowSelf(questionid,typ);

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                break;
            case Enums.InClassTestType.Ask:
                // 学生选择界面
                // TODO
                break;
            case Enums.InClassTestType.Fast:
                // 抢答学生界面
                //TODO
                break;
            default:
                break;
        }
    }

    //接收学生点赞 举手
    public void ReceiveLike(string token)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
    }

    public void ReceiveDoubt(string token)
    {
        if (IsTeacher)
        {
            // 
            // TODO
            // 显示有疑问图标
            if (flashing != null && !flashing.activeSelf)
            {
                flashing.SetActive(true);
            }
        }
        
    }

    // 学生点赞 举手
    public void SendLike()
    {
        if (IsTeacher)
        {
            return;
        }
        // TODO
        UiDataManager.getInstance().SendLike();
    }

    public void SendDoubt()
    {
        if (IsTeacher)
        {
            return;
        }
        // TODO
        UiDataManager.getInstance().SendDoubt();
    }

    // 中间的按钮模块
    // 测试按钮
    public void TestBtn(Toggle go)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!go.isOn)
        {
            return;
        }

        question.ShowSelf();

        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
    }
    // 抢答按钮
    public void FastBtn(Toggle go)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!go.isOn)
        {
            return;
        }

        question.ShowSelf();

        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
    }

    //提问按钮
    public void AskBtn(Toggle go)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!go.isOn)
        {
            return;
        }

        question.ShowSelf();

        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
    }
    //返回大厅
    public void BackLobby()
    {
        // 返回大厅
    }

    // 重置场景
    public Dropdown reset;
    public void ResetScene()
    {
        if (!IsTeacher)
        {
            return;
        }

        if (reset == null)
        {
            return;
        }

        switch (reset.value)
        {
            case 0:
                // 重置全部
                UiDataManager.getInstance().ResetSceneAll();

                players.HideSelf();
                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                break;
            case 1:
                // 重置组
                // 显示组选择界面
                UiDataManager.getInstance().ResetScene(1);

                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                break;
            case 2:
                // 重置学生
                // 显示所有学生界面
                UiDataManager.getInstance().ResetScene(2);

                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                break;
            default:
                break;
        }
    }
}
