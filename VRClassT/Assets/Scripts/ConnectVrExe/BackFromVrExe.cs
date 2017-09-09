using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TinyFrameWork;
using UnityEngine;

public class BackFromVrExe : MonoBehaviour  //, msg_req_ret
{
/*
    private static GameObject selfgo;
    private static BackFromVrExe _instance;
    public static BackFromVrExe getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("BackFromVrExe");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<BackFromVrExe>();
            }
        }

        return _instance;
    }

    // 界面相关
    public GameObject login;
    public GameObject courselist;
    public GameObject uimanage;

    private void Awake()
    {
        string cmdInfo = string.Empty;
        string[] arguments = Environment.GetCommandLineArgs();

        if (arguments != null)
        {
            if(arguments.Length > 5)
            {
                string name = arguments[1];
                string password = arguments[2];
                string courseid = arguments[3];
                string path = arguments[4];
                string fullpath = arguments[5];

                InitSaveData(name, password, Convert.ToInt32(courseid));
                InitSaveDataPath(path, fullpath);

                StartSelfByOther(path);

            }
            else if(arguments.Length > 3)
            {
                string name = arguments[1];
                string password = arguments[2];
                string courseid = arguments[3];

                InitSaveData(name, password, Convert.ToInt32(courseid));

                StartSelfByOther(null);

            }

            EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.ConnectedHub, this.connectedhub);
        }
    }

    //// Use this for initialization
    //void Start () {

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

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.PlayerBackLobby, this.PlayerBackLobby);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.PlayerBackLobby, this.PlayerBackLobby);
    }

    private void PlayerBackLobby(int ret)
    {
        if (ret != 1) return;

        if (savedata == null) return;
        if (savedata.fullpath == null) return;

        if (!File.Exists(savedata.fullpath))
        {
            return;
        }

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = savedata.fullpath;
            startInfo.Arguments = savedata.name
                + " " + savedata.password
                + " " + savedata.courseid.ToString();
            Process proc = Process.Start(startInfo);

            Application.Quit();
        }
        catch (ArgumentException ex)
        {

        }
    }

    // 启动课件之前的关键数据保存
    public void SaveJsonData()
    {
        string path = FileManager.getInstance().TransitionCache;  // 交换文件 缓存目录

        //JsonDataHelp.getInstance().JsonDeserialize<DataType.CourseListRetData>(jsondata);
    }

    // 获取交换文件缓存数据
    public void GetJsonData()
    {
        string path = FileManager.getInstance().TransitionCache;  // 交换文件 缓存目录

        //JsonDataHelp.getInstance().JsonDeserialize<DataType.CourseListRetData>(jsondata);
    }

    // 临时方案
    public class TransitionData
    {
        public string name = "lixin";
        public string password = "1";
        public int courseid = 80;
        public string filepath = string.Empty;
        public string fullpath = string.Empty;
    }

    public TransitionData savedata;

    public void InitSaveData(string name, string password, int courseid)
    {
        if(savedata == null)
        {
            savedata = new TransitionData();
        }

        savedata.name = name;
        savedata.password = password;
        savedata.courseid = courseid;
    }

    public void InitSaveData(string name, string password)
    {
        if (savedata == null)
        {
            savedata = new TransitionData();
        }

        savedata.name = name;
        savedata.password = password;
    }

    public void InitSaveDataPath(string filepath, string fullpath)
    {
        if (savedata == null)
        {
            savedata = new TransitionData();
        }

        savedata.filepath = filepath;
        savedata.fullpath = fullpath;
    }

    public void InitSaveData(int courseid)
    {
        if (savedata == null)
        {
            savedata = new TransitionData();
        }

        savedata.courseid = courseid;
    }

    // 启动课件之前的关键数据保存
    public void SaveJsonDataNo()
    {
        if(savedata == null)
        {
            return;
        }

        string path = FileManager.getInstance().TransitionCache;  // 交换文件 缓存目录

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string jsondata = JsonDataHelp.getInstance().JsonSerialize<TransitionData>(savedata);

        FileOpenWrite.getInstance().WriteText(jsondata, path + "/Test.json");
    }

    // 获取交换文件缓存数据
    public TransitionData GetJsonDataNo()
    {
        string path = FileManager.getInstance().TransitionCache;  // 交换文件 缓存目录

        string jsondata = FileOpenWrite.getInstance().ReadText(path + "/Test.json");

        return JsonDataHelp.getInstance().JsonDeserialize<TransitionData>(jsondata);
    }

    public TransitionData GetJsonDataNo(string path)
    {
        string jsondata = FileOpenWrite.getInstance().ReadText(path);

        if(jsondata != null)
        {
            return JsonDataHelp.getInstance().JsonDeserialize<TransitionData>(jsondata);
        }
        else
        {
            return null;
        }
    }

    public string OpenExeCachePath()
    {
        string path = FileManager.getInstance().TransitionCache;
        return path + "/Test.json";
    }

    public void StartSelfByOther(string path)
    {
        isfromvrexe = true;

        if (savedata == null)
        {
            return;
        }

        // 向服务器请求

        if(courselist!=null)
        {
            courselist.SetActive(false);
        }

        if (login != null)
        {
            login.SetActive(false);
        }
    }

    bool isfromvrexe = false;
    public bool EnterLobby()
    {
        if(isfromvrexe)
        {
            if (courselist != null)
            {
                courselist.SetActive(false);
            }

            uimanage.GetComponent<OutUiManager>().ChooseCourse(savedata.courseid);
        }

        return isfromvrexe;
    }

    public void connectedhub()
    {
        if(savedata == null)
        {
            return;
        }

        //MsgModule.getInstance().registerMsgHandler(this);
        MsgModule.getInstance().playlogincoursewave(savedata.name,savedata.password);
    }

    public void req_msg(Hashtable msg)
    {
    }

    public void ret_msg(Hashtable msg)
    {
        LogText.getInstance().Log("返回啦");

        string uuid = (string)msg["uuid"];
        string duty = (string)msg["duty"];
        string loginjsondata = JsonDataHelp.getInstance().DecodeBase64(null, (string)msg["login"]);
        string baseinfor = JsonDataHelp.getInstance().DecodeBase64(null, (string)msg["baseinfor"]);
        string courseinfor = JsonDataHelp.getInstance().DecodeBase64(null, (string)msg["courseinfor"]);
        string questioninfor = (string)msg["questioninfor"];
        string materiallist = (string)msg["materiallist"];

        UserInfor.getInstance().UserUuid = uuid;

        LogText.getInstance().Log("数据解析" + uuid);

        DataType.PlayerLoginRetData login = JsonDataHelp.getInstance().JsonDeserialize<DataType.PlayerLoginRetData>(loginjsondata);

        LogText.getInstance().Log("login");

        UserInfor.getInstance().UserToken = login.data.access_token;
        UserInfor.getInstance().UserName = login.data.name;
        LogText.getInstance().Log("login name " + login.data.name);
        UserInfor.getInstance().UserId = Convert.ToInt64(login.data.id);

        LogText.getInstance().Log("login end");

        DataType.PlayerBaseInforRetData baseinfordata = JsonDataHelp.getInstance().JsonDeserialize<DataType.PlayerBaseInforRetData>(baseinfor);

        LogText.getInstance().Log("baseinfordata");

        if (duty == "teacher")
        {
            UserInfor.getInstance().UserDuty = "teacher";
            UserInfor.getInstance().isleader = true;
            UserInfor.getInstance().isTeacher = true;
        }

        if (duty == "student")
        {
            UserInfor.getInstance().UserDuty = "student";
            UserInfor.getInstance().isleader = false;
            UserInfor.getInstance().isTeacher = false;
        }

        UserInfor.getInstance().avatar = baseinfordata.data.avatar;

        LogText.getInstance().Log("baseinfordata end");

        if(courseinfor!=null)
        {
            DataType.CourseInforRetData courseinfordata = JsonDataHelp.getInstance().JsonDeserialize<DataType.CourseInforRetData>(courseinfor);

            LogText.getInstance().Log("courseinfordata");

            if (courseinfordata != null)
            {
                UserInfor.getInstance().courseinfor = courseinfordata.data;
                UserInfor.getInstance().RoomId = Convert.ToInt32(courseinfordata.data.course_id);
                UserInfor.getInstance().RoomConnecter = "NetMessage";
            }
        }

        LogText.getInstance().Log("courseinfordata end");

        QuestionManager.getInstance().InitQuestionList(questioninfor);

        LogText.getInstance().Log("UserInfor.getInstance().isTeacher" + UserInfor.getInstance().isTeacher);

        if (UserInfor.getInstance().isTeacher)
        {
            ClassManager.getInstance().InitDefault();

            // 初始化玩家列表
            UiDataManager.getInstance().InitPlayers();

            // 开启ui界面
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.Prepare);

            DownLoadDataManager.getInstance().InitMaterialList(materiallist);
        }
        else
        {
            // 开启学生ui界面
            OutUiManager.getInstance().ShowUI(OutUiManager.UIList.StudentUI);
        }
    }*/
}
