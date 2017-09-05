using ko.NetFram;
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
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int, int>(EventId.PPTCtrl, this.PPTCtrl);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int, int>(EventId.PPTCtrl, this.PPTCtrl);
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
            MsgModule.getInstance().reqPPtCtrl(1, currentpptindex);
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
            MsgModule.getInstance().reqPPtCtrl(1, currentpptindex);
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
