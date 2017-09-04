using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayOrdinaryVideo : MonoBehaviour {

    public UniversalMediaPlayer ump;

    public Text totaltime;
    public Text lefttime;
    public Slider progress;
    public RectTransform screen;

    public OpenFileManager.VoidCallBack closecallback;

    private string videopath = string.Empty;// 用于播放器切换

    //// Use this for initialization
    //void Start () {

    //}

    //Update is called once per frame
    void Update()
    {
        onUpdateProgress();
        onUpdateTime();
    }

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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int>(EventId.VideoCtrl, this.VideoCtrl);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int>(EventId.VideoCtrl, this.VideoCtrl);
    }

    // 视频控制相关 typ 1 开关 2 进度 3 音量 4 亮度 5 value 开关 1 开 0 关 进度*100
    private void VideoCtrl(int typ, int value)
    {
        switch(typ)
        {
            case 1:
                OpenCloseStudent(value);
                break;
            case 2:
                float v = value * 0.01f;
                PositionChangeStudent(v);
                break;
            case 5:
                if(value == 1)
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
        if(ump == null)
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

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if(autoplay)
        {
            ump.Play();

   //         fitSize();

            //float t = ump.Length;
            //totaltime.text = (int)(t / 60) + ":" + (int)(t % 60);
        }
    }

    public void Close()
    {
        ump.Stop();

        if (closecallback != null)
        {
            closecallback();
        }

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

 //       videopath = string.Empty;
        
    }

    public void onUpdateProgress()
    {
        if(ump == null)
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

    public void CloseClick()
    {
        if(UserInfor.getInstance().isTeacher)
        {
            MsgModule.getInstance().reqCtrlVideo(1, 0);
        }
        Close();
    }

    public void SwitchClick()
    {
        Close();

        if (UserInfor.getInstance().isTeacher)
        {
            //全景普通切换 1 切到全景 2 切到普通
            MsgModule.getInstance().reqCtrlVideo(5, 1);
        }

        OpenFileManager.getInstance().OpenPanoramicVideo(videopath);
    }

    public float lastv = 0;
    public float nowv = 0;
    public void PositionChange(Slider s)
    {
        nowv = s.value;
        if (nowv - lastv > 0.1f)
        {
            if(UserInfor.getInstance().isTeacher)
            {
                MsgModule.getInstance().reqCtrlVideo(2, (int)(nowv * 100));
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
        switch(v)
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

    private void fitSize()
    {
        if(ump == null || screen == null)
        {
            return;
        }

        float h = ump.VideoHeight;
        float w = ump.VideoWidth;
        float scaly = w / h;

        if(scaly > 0)
        {
            screen.localScale = new Vector3(screen.localScale.y * scaly, screen.localScale.y, screen.localScale.z);
        }
    }
}
