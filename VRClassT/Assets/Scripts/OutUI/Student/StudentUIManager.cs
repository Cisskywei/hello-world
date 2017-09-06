using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class StudentUIManager : OutUIBase
{
    private static GameObject selfgo;
    private static StudentUIManager _instance;
    public static StudentUIManager getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("student");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<StudentUIManager>();
            }
        }

        return _instance;
    }

    public enum UIStudent
    {
        Prepare = 0,
        ClassTest,
        FastTest,
        FastTestGet,
        WritingPad,
    }

    [SerializeField]
    public OutUIBase[] uilist;

    public override void ShowSelf(params System.Object[] args)
    {
        ShowUI(UIStudent.Prepare);

        base.ShowSelf();
    }

    void OnEnable()
    {
 //       RegisterEvent();
        RegListener();
    }

    void OnDisable()
    {
  //      UnRegisterEvent();
        RemoveListener();
    }

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.StudentAnswerQuestion, this.StudentAnswerQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int, string>(EventId.StudentFastQuestion, this.StudentFastQuestion);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<string, string, string, int>(EventId.DownLoadFileOne, this.DownLoadFileOne);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Hashtable>(EventId.DownLoadFileAll, this.DownLoadFileAll);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Int64, int>(EventId.SwitchWhiteBoard, this.SwitchWhiteBoard);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int, string>(EventId.StudentFastQuestion, this.StudentFastQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int, int, string>(EventId.StudentReciveQuestion, this.StudentReciveQuestion);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<string, string, string, int>(EventId.DownLoadFileOne, this.DownLoadFileOne);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Hashtable>(EventId.DownLoadFileAll, this.DownLoadFileAll);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Int64, int>(EventId.SwitchWhiteBoard, this.SwitchWhiteBoard);
    }

    // 注册 取消 网络消息监听模块
    private void RegListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenContent, OpenContent);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.TestInClass, TestInClass);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenPPt, OpenPPt);
    }

    private void RemoveListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenContent, OpenContent);
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.TestInClass, TestInClass);
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.OpenPPt, OpenPPt);
    }

    private void OpenContent(int userid, ArrayList msg)
    {
        if(msg == null || msg.Count <= 2)
        {
            return;
        }

        Int64 file = (Int64)msg[2];

        OpenContent((int)file);
    }

    private void TestInClass(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 3)
        {
            return;
        }

        Int64 questiontyp = (Int64)msg[2];
        Int64 questionid = (Int64)msg[3];

        StudentReciveQuestion(0,(int)questiontyp, (int)questionid,null);
    }

    private void OpenPPt(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 2)
        {
            return;
        }

        Int64 openclose = (Int64)msg[2];

        SwitchWhiteBoard(0, (int)openclose);
    }

    //0 是关 1是开 
    private void SwitchWhiteBoard(Int64 userid, int openclose)
    {
        switch(openclose)
        {
            case 0:
                ShowUI(UIStudent.Prepare);
                break;
            case 1:
                ShowUI(UIStudent.WritingPad);
                break;
            default:
                break;
        }
    }

    private void StudentAnswerQuestion(int optionid)
    {
        ShowUI(UIStudent.Prepare);
    }

    private void DownLoadFileOne(string name, string path, string typ, int fileid)
    {
        Debug.Log("学生下载资料 " + name + fileid);
        // 学生端收到老师端在资源推送消息 单个文件下载
        if(uilist[(int)UIStudent.Prepare] != null)
        {
            DownLoadDataUI ddui = (DownLoadDataUI)uilist[(int)UIStudent.Prepare];
            ddui.AddCourseData(name, path, typ, fileid);
            DownLoadDataManager.getInstance().DownLoadOnce(fileid);
        }
    }

    private void DownLoadFileAll(Hashtable files)
    {
        if (uilist[(int)UIStudent.Prepare] == null)
        {
            return;
        }

        DownLoadDataUI ddui = (DownLoadDataUI)uilist[(int)UIStudent.Prepare];

        ddui.AddCourseDataAll(files);

        DownLoadDataManager.getInstance().DownLoadAll();
    }

    // 学生获得抢答权
    private void StudentFastQuestion(int userid, int questionid, string teachername)
    {
        if(userid != (int)(UserInfor.getInstance().UserId))
        {
            ShowUI(UIStudent.Prepare);
            return;
        }

        StudentFastGet sf = (StudentFastGet)uilist[(int)UIStudent.FastTestGet];
        System.Object[] args = new System.Object[] { questionid, teachername};
        ShowUI(UIStudent.FastTestGet, args);
    }

    // 学生接收老师推题
    private void StudentReciveQuestion(int userid, int questiontyp, int questionid, string other)
    {
        ComonEnums.InClassTestType catage = (ComonEnums.InClassTestType)questiontyp;
        // 学生
        switch (catage)
        {
            case ComonEnums.InClassTestType.Test:
                StudentTest st = (StudentTest)uilist[(int)UIStudent.ClassTest];
                st.questionid = questionid;
                ShowUI(UIStudent.ClassTest);
                break;
            case ComonEnums.InClassTestType.Ask:
                break;
            case ComonEnums.InClassTestType.Fast:
                StudentFast sf = (StudentFast)uilist[(int)UIStudent.FastTest];
                sf.questionid = questionid;
                ShowUI(UIStudent.FastTest);
                break;
            default:
                break;
        }
    }

    public void ShowUI(UIStudent id, params System.Object[] args)
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

    public void HideUI(UIStudent id)
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

    // 监听老师操作相关
    private void OpenContent(int fileid)
    {
        Debug.Log("学生端打开文件 " + fileid);

        DownLoadItemInfor infor = DownLoadDataManager.getInstance().GetContentById(fileid);
        ComonEnums.ContentDataType typ = FileManager.getInstance().GetFileContenType(infor.filename, infor.type);

        switch (typ)
        {
            case ComonEnums.ContentDataType.Exe:
                OpenFileManager.getInstance().OpenExe(infor.fullfilepath);
                break;
            case ComonEnums.ContentDataType.PanoramicVideo:
                OpenFileManager.getInstance().OpenPanoramicVideo(infor.fullfilepath);
                break;
            case ComonEnums.ContentDataType.OrdinaryVideo:
                OpenFileManager.getInstance().OpenOrdinaryVideo(infor.fullfilepath,gameObject);
                break;
            case ComonEnums.ContentDataType.Panorama:
                break;
            case ComonEnums.ContentDataType.Picture:
                break;
            case ComonEnums.ContentDataType.PPt:
                OpenFileManager.getInstance().OpenPPt(infor.fullfilepath);
                OutUiManager.getInstance().ShowUI(OutUiManager.UIList.DrawingBoardUI);
                break;
            default:
                break;
        }
    }

    // 按钮相关
    public void OnClickPrepare()
    {
        ShowUI(UIStudent.Prepare);
    }

    public void OnClickDoubt()
    {
        // 发送有疑问
        MsgModule.getInstance().reqSendDoubt();
    }
}
