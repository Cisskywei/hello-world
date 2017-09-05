using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class Teaching : OutUIBase
{
    private static GameObject selfgo;
    private static Teaching _instance;
    public static Teaching getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("Teaching");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<Teaching>();
            }
        }

        return _instance;
    }

    public enum UITeaching
    {
        CourseContent = 0,
        QuestionList,
        QuestionBack,
        FastQuestion,
        GuidePerson,
    }

    //界面系列
    [SerializeField]
    public OutUIBase[] uilist;

    public ComonEnums.InClassTestType catage = ComonEnums.InClassTestType.Test;

    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    void OnEnable()
    {
        RegisterEvent();
    }

    void OnDisable()
    {
        UnRegisterEvent();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.ChoosePerson, this.ChoosePerson);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.ContentDataType, DownLoadItemInfor>(EventId.OpenContent, this.OpenContent);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<ComonEnums.ContentDataType, DownLoadItemInfor>(EventId.DownLoadContent, this.StartDownLoad);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);

        //OpenContent
        // student
        //EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int, string>(EventId.StudentFastQuestion, this.StudentFastQuestion);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.InClassTestType, ComonEnums.QuestionType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.ChoosePerson, this.ChoosePerson);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.ContentDataType, DownLoadItemInfor>(EventId.OpenContent, this.OpenContent);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<ComonEnums.ContentDataType, DownLoadItemInfor>(EventId.DownLoadContent, this.StartDownLoad);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64>(EventId.DoubtFeedBack, this.ReceiveDoubt);

        //EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int, string>(EventId.StudentFastQuestion, this.StudentFastQuestion);
    }

    private void StudentFastQuestion(int userid, int questionid, string teachername)
    {
        ShowUI(UITeaching.FastQuestion);
        if(uilist[(int)UITeaching.FastQuestion] != null)
        {
            FastInClassUI ficu = (FastInClassUI)uilist[(int)UITeaching.FastQuestion];
            ficu.SomeOneCome(userid);
        }
    }

    // 老师端接收学生疑问
    private void ReceiveDoubt(Int64 userid)
    {
        // 有疑问学生列表头像
        // TODO
    }

    public void ChooseQuestion(ComonEnums.InClassTestType catage, ComonEnums.QuestionType typ, int questionid)
    {
        // 选择题目监听
        //TODO
        switch(this.catage)
        {
            case ComonEnums.InClassTestType.Fast:
                FastInClassUI fic = (FastInClassUI)uilist[(int)UITeaching.FastQuestion];
                fic.questionid = questionid;
                ShowUI(UITeaching.FastQuestion);
                break;
            case ComonEnums.InClassTestType.Test:
                TestFeedBackUI tb = (TestFeedBackUI)uilist[(int)UITeaching.QuestionBack];
                tb.questionid = questionid;
                tb.questiontyp = typ;
                ShowUI(UITeaching.QuestionBack);
                break;
            case ComonEnums.InClassTestType.Ask:
                PlayerListUI pl = (PlayerListUI)uilist[(int)UITeaching.GuidePerson];
                ShowUI(UITeaching.GuidePerson);
                break;
            default:
                break;
        }
    }

    public void ChoosePerson(Int64 id)
    {
        if(this.catage != ComonEnums.InClassTestType.Ask)
        {
            return;
        }
        // 选择某人回答问题
        //TODO
    }

    private void OpenContent(ComonEnums.ContentDataType typ, DownLoadItemInfor infor)
    {
        Debug.Log("打开文件 " + typ + "    " + infor.fullfilepath);
        switch(typ)
        {
            case ComonEnums.ContentDataType.Exe:
                OpenFileManager.getInstance().OpenExe(infor.fullfilepath);
                break;
            case ComonEnums.ContentDataType.PanoramicVideo:
                OpenFileManager.getInstance().OpenPanoramicVideo(infor.fullfilepath);
                break;
            case ComonEnums.ContentDataType.OrdinaryVideo:
                OpenFileManager.getInstance().OpenOrdinaryVideo(infor.fullfilepath, gameObject);
                break;
            case ComonEnums.ContentDataType.Panorama:
                break;
            case ComonEnums.ContentDataType.Picture:
                break;
            case ComonEnums.ContentDataType.PPt:
                OpenFileManager.getInstance().OpenPPt(infor.fullfilepath);
                OutUiManager.getInstance().ShowUI(OutUiManager.UIList.DrawingBoardUI);
                break;
            case ComonEnums.ContentDataType.Zip:
                OpenFileManager.getInstance().OpenExe(infor.fullfilepath);
                break;
            default:
                break;
        }

        MsgModule.getInstance().reqOpenContent(infor.fileid);
    }

    private void StartDownLoad(ComonEnums.ContentDataType typ, DownLoadItemInfor dlii)
    {
        // 发送推送消息
        if (dlii != null)
        {
            MsgModule.getInstance().reqDownLoadFileOne(dlii.filename, dlii.filepath, dlii.type, dlii.fileid);
        }
    }

    public override void ShowSelf(params System.Object[] args)
    {
        ShowUI(UITeaching.CourseContent);
        base.ShowSelf();
    }

    public void ShowUI(UITeaching id, params System.Object[] args)
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

    public void HideUI(UITeaching id)
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

        uilist[uiid].HideSelf();
    }

    public void OnClickBack()
    {
        // 返回
        HideSelf();
        OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Prepare);
    }

    public void OnClickAllTest()
    {
        System.Object[] args = new System.Object[] { ComonEnums.InClassTestType.Test };
        ShowUI(UITeaching.QuestionList, args);
        catage = ComonEnums.InClassTestType.Test;
    }

    public void OnClickFastTest()
    {
        System.Object[] args = new System.Object[] { ComonEnums.InClassTestType.Fast };

        ShowUI(UITeaching.QuestionList, args);
        catage = ComonEnums.InClassTestType.Fast;
    }

    public void OnClickChoose()
    {
        System.Object[] args = new System.Object[] { ComonEnums.InClassTestType.Ask };

        ShowUI(UITeaching.QuestionList, args);
        catage = ComonEnums.InClassTestType.Ask;
    }

    public void OnClickWrite()
    {
        OutUiManager.getInstance().ShowUI(OutUiManager.UIList.DrawingBoardUI);

        MsgModule.getInstance().reqWhiteBoard(1);
    }
}
