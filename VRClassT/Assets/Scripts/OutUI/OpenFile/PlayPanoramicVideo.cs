using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanoramicVideo : MonoBehaviour {

    public UniversalMediaPlayer ump;

    public Text totaltime;
    public Text lefttime;
    public Slider progress;

    public OpenFileManager.VoidCallBack closecallback;

    public Transform viverig; // htc 摄像机
    public Vector3 topos = new Vector3(0,100,0);

    private Vector3 originpos;
    private Quaternion originrot;

    private string videopath = string.Empty;// 用于播放器切换

    //private Vector3 originscal;

    //// Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    void Update()
    {
        onUpdateProgress();
        onUpdateTime();
    }

    void OnEnable()
    {
        RegListener();
  //      RegisterEvent();
    }

    void OnDisable()
    {
        RemoveListener();
  //      UnRegisterEvent();
    }

    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int>(EventId.VideoCtrl, this.VideoCtrl);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int>(EventId.VideoCtrl, this.VideoCtrl);
    }

    // 注册 取消 网络消息监听模块
    private void RegListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.VideoCtrl, VideoCtrlListener);
    }

    private void RemoveListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.VideoCtrl, VideoCtrlListener);
    }

    private void VideoCtrlListener(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 3)
        {
            return;
        }

        Int64 typ = (Int64)msg[2];
        Int64 value = (Int64)msg[3];

        VideoCtrl((int)typ, (int)value);
    }

    // 视频控制相关 typ 1 开关 2 进度 3 音量 4 亮度 5 value 开关 1 开 0 关 进度*100
    private void VideoCtrl(int typ, int value)
    {
        switch (typ)
        {
            case 1:
                OpenCloseStudent(value);
                break;
            case 2:
                float v = value * 0.01f;
                PositionChangeStudent(v);
                break;
            case 5:
                if(value == 2)
                {
                    SwitchClick();
                }
                break;
            default:
                break;

        }
    }

    public void Init(string path)
    {
        if (ump == null)
        {
            return;
        }

        ump.Path = path;
    }

    public void Open(string path, bool autoplay = true)
    {
        videopath = path;

        path = "file:///" + path.Replace('\\', '/');

        Init(path);

        // 保存初始位置
        originpos = viverig.position;
        originrot = viverig.rotation;

        viverig.transform.position = topos;
        transform.position = new Vector3(topos.x,topos.y + 3,topos.z);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (autoplay)
        {
            ump.Play();
        }
    }

    public void Close()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        ump.Stop();

  //      videopath = string.Empty;

        viverig.position = originpos;
        viverig.rotation = originrot;

        if (closecallback != null)
        {
            closecallback.Invoke();
        }
    }

    public void onUpdateProgress()
    {
        if (ump == null)
        {
            return;
        }

        progress.value = ump.Position;
    }

    public void onUpdateTime()
    {
        if (ump == null)
        {
            return;
        }

        float t = ump.Position * ump.Length;
        lefttime.text = (int)(t / 60) + ":" + (int)(t % 60);
    }

    public float lastv = 0;
    public float nowv = 0;
    public void PositionChange(Slider s)
    {
        nowv = s.value;
        if (nowv - lastv > 0.1f)
        {
            if (UserInfor.getInstance().isTeacher)
            {
                ArrayList msg = new ArrayList();
                msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
                msg.Add((Int64)CommandDefine.SecondLayer.VideoCtrl);
                msg.Add((Int64)2);
                msg.Add((Int64)(nowv * 100));
                CommandSend.getInstance().Send((int)UserInfor.getInstance().UserId, (int)UserInfor.getInstance().RoomId, msg);
                //MsgModule.getInstance().reqCtrlVideo(2, (int)(nowv * 100));
            }

            ump.Position = s.value;
        }

        lastv = nowv;
    }

    private void PositionChangeStudent(float v)
    {
        progress.value = v;
    }

    private void OpenCloseStudent(int v)
    {
        switch (v)
        {
            case 0:
                CloseClick();
                break;
            case 1:
                break;
            default:
                break;
        }
    }

    public void CloseClick()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.VideoCtrl);
            msg.Add((Int64)1);
            msg.Add((Int64)0);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().UserId, (int)UserInfor.getInstance().RoomId, msg);
        }

        Close();
    }
    
    public void SwitchClick()
    {
        Close();

        if(UserInfor.getInstance().isTeacher)
        {
            //全景普通切换 1 切到全景 2 切到普通
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.VideoCtrl);
            msg.Add((Int64)5);
            msg.Add((Int64)2);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().UserId, (int)UserInfor.getInstance().RoomId, msg);
        }

        OpenFileManager.getInstance().OpenOrdinaryVideo(videopath);
    }
}
