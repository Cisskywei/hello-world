using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class OutUiManager : MonoBehaviour {

    private static GameObject selfgo;
    private static OutUiManager _instance;
    public static OutUiManager getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("Canvas");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<OutUiManager>();
            }
        }

        return _instance;

    }

    public enum UIList
    {
        Login = 0,
        CourseList,
        Prepare,
        Teaching,
        StudentUI,
        DrawingBoardUI,
    }

    // 界面变量
    [SerializeField]
    public OutUIBase[] alloutui;

    public void ShowUI(UIList id, params System.Object[] args)
    {
        if (alloutui == null || alloutui.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if(uiid > alloutui.Length)
        {
            return;
        }

        for(int i=0;i<alloutui.Length;i++)
        {
            if(i == uiid)
            {
                alloutui[i].ShowSelf(args);
            }
            else
            {
                alloutui[i].HideSelf();
            }
        }

    }

    public void HideUI(UIList id)
    {
        if (alloutui == null || alloutui.Length <= 0)
        {
            return;
        }
        int uiid = (int)id;
        if (uiid > alloutui.Length)
        {
            return;
        }

        alloutui[uiid].HideSelf();
    }

    void OnEnable()
    {
        RegisterEvent();
        UiDataManager.getInstance().RegisterEventListener();

        RegListener();
    }

    void OnDisable()
    {
        UnRegisterEvent();
        UiDataManager.getInstance().UnRegisterEventListener();

        RemoveListener();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.ChooseCourse, this.ChooseCourse);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.ChooseCourse, this.ChooseCourse);
    }

    // 注册 取消 网络消息监听模块
    private void RegListener()
    {
        if(UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataAll, PushDataAll);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataOne, PushDataOne);
    }

    private void RemoveListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataAll, PushDataAll);
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PushDataOne, PushDataOne);
    }

    public void ChooseCourse(int courseid)
    {
        // 
        Debug.Log("选择课程 id : " + courseid);

        EnterCourse.getInstance().PlayerEnterCourse(courseid);
    }

    private void PushDataAll(int userid, ArrayList msg)
    {
        if(msg == null || msg.Count<= 2)
        {
            return;
        }

        Hashtable files = (Hashtable)msg[2];
        DownLoadDataUI.getInstance().AddCourseDataAll(files);

        DownLoadDataManager.getInstance().DownLoadAll();
    }

    private void PushDataOne(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 5)
        {
            return;
        }

        string name, path, typ;
        int fileid;
        name = (string)msg[2];
        path = (string)msg[3];
        typ = (string)msg[4];
        fileid = Convert.ToInt32((string)msg[5]);
        Debug.Log("学生下载资料 " + name + fileid);
        // 学生端收到老师端在资源推送消息 单个文件下载
        DownLoadDataUI.getInstance().AddCourseData(name, path, typ, fileid);
        DownLoadDataManager.getInstance().DownLoadOnce(fileid);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChooseCourse(227);
        }
    }
}
