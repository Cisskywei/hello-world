using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public enum UIList
    {
        None = 0,
        First,
        Second,

        Max,
    }

    // 信息
    public MsgTips msgShow;

    public UIList _uisort = UIList.None;
    [SerializeField]
    public GameObject[] uisort = new GameObject[(int)UIList.Max];
    public GameObject uiroot;   // 界面根目录

    public GameObject first01;

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
    // 电子白板
    public WhiteBoardUI palette;

    // 模式文字
    public string modeTxt = "观学模式-同步";

    // 学生端
    public StudentStateInfor studentstate;
    public StudentTest studenttest;
    public StudentFast studentfast;
    public StudentFastGet studentfastget;

    private bool _isInitVisible = false; //标记是否依据学生老师进行设置显示与否

 //   // Use this for initialization
 //   void Start () {
		
	//}

 //   // Update is called once per frame
 //   void Update()
 //   {

 //   }

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

    private void InitUIByDuty()
    {
        if(_isInitVisible)
        {
            return;
        }

        if (IsTeacher)
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

            if (stateinforteacher != null)
            {
                stateinforteacher.gameObject.SetActive(true);
            }

            if (mode!=null)
            {
                mode.gameObject.SetActive(true);
            }

            if(palettebtn != null)
            {
                palettebtn.SetActive(true);
            }
        }
        else
        {
            // 显示学生相关界面
            if (msg != null)
            {
                msg.gameObject.SetActive(true);
            }

            if(studentstate != null)
            {
                studentstate.gameObject.SetActive(true);
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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64, Int64, string>(EventId.SwitchModeFeedBack, this.SwitchModeFeedBack);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.LikeFeedBack, this.ReceiveLike);

        EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.OpenUI, this.OpenUI);

        // student
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int>(EventId.StudentFastQuestion, this.StudentFastQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int, int, string>(EventId.StudentReciveQuestion, this.StudentReciveQuestion); 
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64, int>(EventId.SwitchWhiteBoard, this.SwitchWhiteBoard);

        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.BackToLobby, this.BackToLobby);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.ResetScene, this.ResetSceneBack);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64, Int64, string>(EventId.SwitchModeFeedBack, this.SwitchModeFeedBack);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.LikeFeedBack, this.ReceiveLike);

        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.OpenUI, this.OpenUI);

        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int>(EventId.StudentFastQuestion, this.StudentFastQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int, int, string>(EventId.StudentReciveQuestion, this.StudentReciveQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64, int>(EventId.SwitchWhiteBoard, this.SwitchWhiteBoard);

        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.BackToLobby, this.BackToLobby);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.ResetScene, this.ResetSceneBack);

    }

    public int _isopenui = 0;
    public GameObject guide;
    private int uiorder = -1;
    private void OpenUI()
    {
        uiorder = (int)_uisort;
        uiorder = (++uiorder)%(int)UIList.Max;

        uiorder -= 1;
        for (int i=0;i<uisort.Length;i++)
        {
            if(i == uiorder)
            {
                uisort[i].SetActive(true);
                if(uiorder == 0)
                {
                    if(!first01.activeSelf)
                    {
                        first01.SetActive(true);
                    }

                    questionback.HideSelf();
                    groups.HideSelf();
                    players.HideSelf();
                    question.HideSelf();
                    fastquestion.HideSelf();
                    handuplist.HideSelf();
                }
                palette.HideSelf();
            }
            else
            {
                uisort[i].SetActive(false);
            }
        }

        _uisort = (UIList)(++uiorder);

        if (_uisort == UIList.Max || _uisort == UIList.None)
        {
            // 关闭所有界面
            if(uiroot.activeSelf)
            {
                uiroot.SetActive(false);
            }

        }
        else
        {
            // 关闭所有界面
            if (!uiroot.activeSelf)
            {
                uiroot.SetActive(true);
            }

        }
    }

    public void ShowFirstUI(bool show)
    {
        if(!IsTeacher)
        {
            return;
        }

        if(first01 != null)
        {
            first01.SetActive(show);
        }

        if(handuplist != null)
        {
            handuplist.HideSelf();
        }

        if(show)
        {
            if(palette!=null && palette.gameObject.activeSelf)
            {
                palette.gameObject.SetActive(false);
            }
        }
    }

    // 监听函数
    public void SwitchMode(ComonEnums.TeachingMode mode)
    {
        if(!IsTeacher)
        {
            return;
        }

        // 判断弹出对应界面
        switch (mode)
        {
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
                //弹出学生列表界面
                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);

                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
                //弹出小组选择界面
                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);

                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
                //学生各自协同操作
                //players.ShowSelf();

                //groups.HideSelf();
                //question.HideSelf();
                //questionback.HideSelf();
                //fastquestion.HideSelf();

                //ShowFirstUI(false);


                break;
            case ComonEnums.TeachingMode.SelfTrain_Group:
                //小组各自协同操作
                //groups.ShowSelf();

                //players.HideSelf();
                //question.HideSelf();
                //questionback.HideSelf();
                //fastquestion.HideSelf();

                //ShowFirstUI(false);


                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
            case ComonEnums.TeachingMode.SelfTrain_All:
            case ComonEnums.TeachingMode.VideoOnDemand_General:
            case ComonEnums.TeachingMode.VideoOnDemand_Full:
            case ComonEnums.TeachingMode.VideoOnLive_General:
            case ComonEnums.TeachingMode.VideoOnLive_Full:

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                break;
            default:
                break;
        }

    }

    // 切换模式返回 更新界面状态
    public void SwitchModeFeedBack(Int64 userid, Int64 modee, string targetid)
    {
        ComonEnums.TeachingMode mode = (ComonEnums.TeachingMode)modee;
        Debug.Log(mode + " -- 模式改变");
        // TODO
        string who = string.Empty;
        PlayerInfor pi = null;
        GroupInfor gi = null;

        // 判断弹出对应界面
        switch (mode)
        {
            case ComonEnums.TeachingMode.WatchLearnModel_Sync:
                modeTxt = "观学模式-同步";
                break;
            case ComonEnums.TeachingMode.WatchLearnModel_Async:
                modeTxt = "观学模式-异步";
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Personal:
                pi = UiDataManager.getInstance().GetPlayerById(Convert.ToInt64(targetid));
                if(pi != null)
                {
                    who = pi.name;
                }
                modeTxt = "指导模式-指导人 (" + who + ")";
                break;
            case ComonEnums.TeachingMode.GuidanceMode_Group:
                gi = UiDataManager.getInstance().GetGroupByToken(targetid);
                if (gi != null)
                {
                    who = gi.name;
                }
                modeTxt = "指导模式-指导组 (" + who + ")";
                break;
            case ComonEnums.TeachingMode.SelfTrain_Personal:
             //   pi = UiDataManager.getInstance().GetPlayerById(Convert.ToInt64(targetid));
            //    if (pi != null)
            //    {
            //        who = pi.name;
            //    }
                modeTxt = "自主训练模式-独立";
                break;
            case ComonEnums.TeachingMode.SelfTrain_Group:
       //         gi = UiDataManager.getInstance().GetGroupByToken(targetid);
      //          if (gi != null)
     //           {
      //              who = gi.name;
     //           }
                modeTxt = "自主训练模式-小组";
                break;
            case ComonEnums.TeachingMode.SelfTrain_All:
        //        gi = UiDataManager.getInstance().GetGroupByToken(targetid);
       //         if (gi != null)
        //        {
        //            who = gi.name;
        //        }
                modeTxt = "自主训练模式-全部";
                break;
            case ComonEnums.TeachingMode.VideoOnDemand_General:
                modeTxt = "视频点播-普通";
                break;
            case ComonEnums.TeachingMode.VideoOnDemand_Full:
                modeTxt = "视频点播-全景";
                break;
            case ComonEnums.TeachingMode.VideoOnLive_General:
                modeTxt = "视频直播-普通";
                break;
            case ComonEnums.TeachingMode.VideoOnLive_Full:
                modeTxt = "视频直播-全景";
                break;
            default:
                break;
        }

        if (msgShow != null)
        {
            msgShow.ShowMessage(modeTxt);
        }

        if (IsTeacher)
        {
            // 老师界面 状态改变 
            stateinfor.GetComponent<StateInforShow>().SetModeText(modeTxt);
        }
        else
        {
            // 学生状态改变 
            studentstate.ChangeState(modeTxt);
        }

        Debug.Log(modeTxt + " -- 模式改变");
    }

    // 选择了某个试题对应的弹出相应界面
    public void ChooseQuestion(ComonEnums.InClassTestType catage, ComonEnums.QuestionType typ, int questionid)
    {
        if (!IsTeacher)
        {
            return;
        }

        switch (catage)
        {
            case ComonEnums.InClassTestType.Test:
                //测试反馈
                questionback.ShowSelf(questionid,typ);

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            case ComonEnums.InClassTestType.Ask:
                // 学生选择界面
                // TODO
                ShowFirstUI(false);

                players.ShowSelf(null,"请选择你要提问的学生:");
                handuplist.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                fastquestion.HideSelf();
                questionback.HideSelf();
                break;
            case ComonEnums.InClassTestType.Fast:
                // 抢答学生界面
                //TODO
                fastquestion.ShowSelf(questionid);

                groups.HideSelf();
                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);

                break;
            default:
                break;
        }
    }

    //接收学生点赞 举手
    public void ReceiveLike(Int64 userid)
    {
        if (!IsTeacher)
        {
            return;
        }

        // TODO
        PlayerInfor p = UiDataManager.getInstance().GetPlayerById(userid);
        if(p != null)
        {
            string modeTxt = p.name + "给您点了赞";
            if (msgShow != null)
            {
                msgShow.ShowMessage(modeTxt);
            }
        }

        if (stateinforteacher != null)
        {
            stateinforteacher.ChangeLikeCount(1);
        }
    }

    public void ReceiveDoubt(Int64 userid)
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

            // 添加有疑问的学生进入列表
            // TODO !!!
            if(stateinforteacher != null)
            {
                stateinforteacher.AddDoubtPerson((int)userid);
            }

            // 添加举手回答问题的学生
            if (handuplist != null && handuplist.gameObject.activeInHierarchy)
            {
                handuplist.AddStudent((int)userid);
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
        fastquestion.HideSelf();
        handuplist.HideSelf();

        ShowFirstUI(false);
    }

    public void TestBtn(bool isopen = true)
    {
        if (!IsTeacher)
        {
            return;
        }

        if (!isopen)
        {
            return;
        }

        question.ShowSelf();

        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        fastquestion.HideSelf();
        handuplist.HideSelf();

        ShowFirstUI(false);
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

        question.ShowSelf(ComonEnums.InClassTestType.Fast);

        fastquestion.HideSelf();
        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        handuplist.HideSelf();
        ShowFirstUI(false);

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

        question.ShowSelf(ComonEnums.InClassTestType.Ask);

        fastquestion.HideSelf();
        players.HideSelf();
        groups.HideSelf();
        questionback.HideSelf();
        handuplist.HideSelf();
        ShowFirstUI(false);
    }

    // 接受题目

    public void StudentReciveQuestion(int userid, int questiontyp, int questionid, string other)
    {
        //if(questionid < 0)
        //{
        //    // 抢答题
        //    ReciveQuestion((ComonEnums.InClassTestType)questiontyp, (ComonEnums.QuestionType)questiontyp, questionid, other);
        //}
        //else if(questionid == 16)
        //{
        //    // 提问 显示点赞等的界面
        //    if (!first01.activeSelf)
        //    {
        //        first01.SetActive(true);
        //    }
        //}
        //else
        //{
        //    ReciveQuestion(ComonEnums.InClassTestType.Test, ComonEnums.QuestionType.SingleChoice, questionid, other);
        //}

        ReciveQuestion((ComonEnums.InClassTestType)questiontyp, questionid, other);
    }
    public void ReciveQuestion(ComonEnums.InClassTestType catage, int id, string other)
    {
        if(IsTeacher)
        {
            return;
        }

        // 学生
        switch(catage)
        {
            case ComonEnums.InClassTestType.Test:
                studenttest.ShowSelf(id, other);

                studentfast.HideSelf();
                break;
            case ComonEnums.InClassTestType.Ask:
                break;
            case ComonEnums.InClassTestType.Fast:
                studentfast.ShowSelf(id, other);

                studenttest.HideSelf();
                break;
            default:
                break;
        }
    }

    // 学生操作监听
    public void StudentAnswerQuestion(int id)
    {

    }

    public void StudentFastQuestion(int userid, int questionid)
    {
        if(studentfast != null)
        {
            studentfast.HideSelf();
        }
        if(studentfastget!=null && ((userid == (int)(UserInfor.getInstance().UserId)) || UserInfor.getInstance().isleader))
        {
            studentfastget.ShowSelf(questionid);
        }
    }

    public void WhiteBoard()
    {
        // TODO
        if(palette != null)
        {
            palette.ShowSelf();
            // 关闭其他界面
            groups.HideSelf();
            players.HideSelf();
            question.HideSelf();
            questionback.HideSelf();
            fastquestion.HideSelf();
            handuplist.HideSelf();

            ShowFirstUI(false);
        }

    }

    public void SwitchWhiteBoard(Int64 userid, int openclose)
    {
        if(IsTeacher)
        {
            return;
        }

        if(openclose == 1)
        {
            palette.ShowSelf();
        }
        else
        {
            palette.HideSelf();
        }
        
    }

    //返回大厅 按钮点击监听
    public void BackLobby()
    {
        // 返回大厅
        MsgModule.getInstance().reqBackLobby();
    }

    public void BackToLobby(Int64 userid)
    {
        Application.Quit();
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
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            case 1:
                // 重置组
                // 显示组选择界面
                UiDataManager.getInstance().ResetScene(1);

                groups.ShowSelf();

                players.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            case 2:
                // 重置学生
                // 显示所有学生界面
                UiDataManager.getInstance().ResetScene(2);

                players.ShowSelf();

                groups.HideSelf();
                question.HideSelf();
                questionback.HideSelf();
                fastquestion.HideSelf();
                handuplist.HideSelf();

                ShowFirstUI(false);
                break;
            default:
                break;
        }
    }

    public void ResetSceneBack(Int64 userid)
    {
        SceneManager.LoadScene(0);
    }
}
