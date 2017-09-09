using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PPtShowUI : OutUIBase {

    // ppt 显示image
    public Image pptshow;

    public FileDataListUI filelist;
    public GameObject paneladdbtn;

    public OpenFileManager.VoidCallBack closecallback;

    private Sprite[] pptimage;
    public bool ishaveppt = false;
    public int currentpptindex = 0;

    public override void ShowSelf(params object[] args)
    {
        string filename = (string)args[0];

        if(filename != null)
        {
            Open(FileManager.getInstance().GetPPTImagePath(filename));
        }
        else
        {
            base.ShowSelf(args);
        }
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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int>(EventId.PPTCtrl, this.PPTCtrl);
    }

    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int>(EventId.PPTCtrl, this.PPTCtrl);
    }

    // 注册 取消 网络消息监听模块
    private void RegListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PPtCtrl, PPTCtrlListener);
    }

    private void RemoveListener()
    {
        if (UserInfor.getInstance().isTeacher)
        {
            return;
        }

        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.PPtCtrl, PPTCtrlListener);
    }

    private void PPTCtrlListener(int userid, ArrayList msg)
    {
        if (msg == null || msg.Count <= 3)
        {
            return;
        }

        Int64 typ = (Int64)msg[2];
        Int64 value = (Int64)msg[3];

        PPTCtrl((int)typ, (int)value);
    }

    // ppt 控制相关 typ 1 ppt 翻页控制 value 翻页页数
    private void PPTCtrl(int typ, int value)
    {
        switch (typ)
        {
            case 1:
                currentpptindex = value;
                if (currentpptindex >= pptimage.Length)
                {
                    currentpptindex = pptimage.Length - 1;
                }else if (currentpptindex <= 0)
                {
                    currentpptindex = 0;
                }
                pptshow.overrideSprite = pptimage[currentpptindex];
                break;
            default:
                break;

        }
    }

    // 打开ppt 文件
    public void Open(string path)
    {
        if(path == null)
        {
            return;
        }
        Debug.Log("打开ppt" + path);

        pptimage = PPTToPicture.getInstance().LoadImage2SpriteByIO(path);

        if(pptimage == null || pptimage.Length <= 0)
        {
            return;
        }

        pptshow.overrideSprite = pptimage[0];
        currentpptindex = 0;

        ishaveppt = true;

        if (paneladdbtn != null && paneladdbtn.activeSelf)
        {
            paneladdbtn.SetActive(false);
        }

        base.ShowSelf();
    }

    public void NextPage()
    {
        if (currentpptindex == pptimage.Length - 1)
        {
            return;
        }

        currentpptindex++;
        if(currentpptindex >= pptimage.Length)
        {
            currentpptindex = pptimage.Length - 1;
        }

        if(UserInfor.getInstance().isTeacher)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.PPtCtrl);
            msg.Add((Int64)1);
            msg.Add((Int64)currentpptindex);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().UserId, (int)UserInfor.getInstance().RoomId, msg);
        }

        pptshow.overrideSprite = pptimage[currentpptindex];
    }

    public void PreviousPage()
    {
        if (currentpptindex == 0)
        {
            currentpptindex = 0;
            return;
        }

        currentpptindex--;
        if (currentpptindex <= 0)
        {
            currentpptindex = 0;
        }

        if (UserInfor.getInstance().isTeacher)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Lobby);
            msg.Add((Int64)CommandDefine.SecondLayer.PPtCtrl);
            msg.Add((Int64)1);
            msg.Add((Int64)currentpptindex);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().UserId, (int)UserInfor.getInstance().RoomId, msg);
        }

        pptshow.overrideSprite = pptimage[currentpptindex];
    }

    public void OnClickPanleAdd()
    {
        if(ishaveppt)
        {
            if(paneladdbtn != null && paneladdbtn.activeSelf)
            {
                paneladdbtn.SetActive(false);
            }

            return;
        }

        if (filelist == null)
        {
            return;
        }

        filelist.ShowSelf(new System.Object[] { ComonEnums.ContentDataType.PPt });

        // 弹出文件列表界面
        if (filelist.clickfilelisten == null)
        {
            filelist.RegisterOpenListen(AddPPt);
        }
    }

    private void AddPPt(DownLoadItemInfor fileinfor)
    {
        if (paneladdbtn != null && paneladdbtn.activeSelf)
        {
            paneladdbtn.SetActive(false);
        }

        // 打开ppt
        Open(FileManager.getInstance().GetPPTImagePath(fileinfor.filename));
    }
}
