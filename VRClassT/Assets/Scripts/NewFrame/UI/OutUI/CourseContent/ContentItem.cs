using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class ContentItem : MonoBehaviour {

    //public Text scetionTxt;
    public Text contentTxt;
    public Text progressTxt;
    public Text btnTxt;
    //public string scetion;
    public string content;

    public ComonEnums.ContentDataType suffixtype = ComonEnums.ContentDataType.None;

    public DownLoadItemInfor infor;

    private int id = -1;

    // Update is called once per frame
    void Update()
    {
        if (infor != null && infor.state == DownLoadItemInfor.DownLoadState.Complete)
        {
            progressTxt.gameObject.SetActive(false);
            ShowBtnText();
        }

        if (infor != null && infor.state == DownLoadItemInfor.DownLoadState.DownLoading)
        {
            progressTxt.text = string.Format("{0}%", (int)(infor.GetProgress()*100));
        }
    }

    public void Init(ContentInfor ci)
    {
        this.id = ci.id;
        this.content = ci.content;
        //this.scetion = ci.scetion;

        //if(scetionTxt != null)
        //{
        //    scetionTxt.text = this.scetion;
        //}

        ShowContent();
    }

    // 根据下载item信息初始化自己
    public void Init(DownLoadItemInfor dii)
    {
        this.id = dii.fileid;
        this.content = dii.filename;
        infor = dii;

        this.suffixtype = FileManager.getInstance().GetFileContenType(dii.filename, dii.type);

        ShowContent();

        ShowBtnText();

        setPosition();
    }

    private void setPosition()
    {
        // 设置位置 防止位置改变
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;
    }

    public void ShowContent()
    {
        if (this.content == null)
        {
            return;
        }

        if (this.content.Length > 10)
        {
            contentTxt.text = this.content.Substring(0, 10) + "...";
        }
        else
        {
            contentTxt.text = this.content;
        }
    }

    public void ShowBtnText()
    {
        if(infor == null)
        {
            btnTxt.transform.parent.gameObject.SetActive(false);
            return;
        }

        string txt = string.Empty;

        switch(infor.state)
        {
            case DownLoadItemInfor.DownLoadState.None:
                txt = "下载";
                break;
            case DownLoadItemInfor.DownLoadState.DownLoading:
                txt = "下载中";
                break;
            case DownLoadItemInfor.DownLoadState.Complete:
                txt = "打开";
                break;
            default:
                break;
        }

        if (txt == string.Empty)
        {
            btnTxt.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            btnTxt.text = txt;
        }

    }

    public void OpenClick()
    {
        if(infor == null)
        {
            return;
        }

        if(this.id < 0)
        {
            return;
        }

        if(this.content == null)
        {
            return;
        }

        switch (infor.state)
        {
            case DownLoadItemInfor.DownLoadState.None:
                infor.StartDownLoad();
                EventDispatcher.GetInstance().MainEventManager.TriggerEvent<ComonEnums.ContentDataType, DownLoadItemInfor>(EventId.DownLoadContent, this.suffixtype, this.infor);
                break;
            case DownLoadItemInfor.DownLoadState.DownLoading:
                break;
            case DownLoadItemInfor.DownLoadState.Complete:
                EventDispatcher.GetInstance().MainEventManager.TriggerEvent<ComonEnums.ContentDataType, DownLoadItemInfor>(EventId.OpenContent, this.suffixtype, this.infor);
                break;
            default:
                break;
        }

        ShowBtnText();
    }
}
