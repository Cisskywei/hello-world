using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadItem : MonoBehaviour {
    /*
        // ui 控件
        //public Text sectionTxt;
        public Text contentTxt;
        public Text progressTxt;
        public Text operateTxt;

        // 下载时的状态
        public enum DownLoadState
        {
            None = 0,
            DownLoading,
            Pause,
            Failure,
            Complete,
        }

        // 下载时的操作
        public enum DownLoadOperation
        {
            DownLoad = 0,
            Cancel,
            Pause,
            Continue,
            Open,
        }

        public DownLoadState state = DownLoadState.None;
        public DownLoadOperation operation = DownLoadOperation.DownLoad;

        //public string section;
        public string content;
        public float progress;
        public string operate;

        public DownLoadItemInfor infor;

        private static string progresstext = "{0}%";

        //// Use this for initialization
        //void Start () {

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}

        public void Init(string section, string content, int progress = 0)
        {
            //this.section = section;
            this.content = content;
            this.progress = progress;

            state = DownLoadState.None;
            operation = DownLoadOperation.DownLoad;

            this.operate = DownLoadItem.ConvertOperationToString(operation);

            // 控件显示
            //this.sectionTxt.text = this.section;
            this.contentTxt.text = this.content;
            this.progressTxt.text = string.Empty;
            this.operateTxt.text = this.operate;
        }

        public void Init(DownLoadItemInfor dii)
        {
            this.content = dii.filename;
        }

        public void UpdateProgress(float progress)
        {
            this.progress = progress;

            progressTxt.text = string.Format(progresstext, ((int)progress).ToString());
        }

        public void OnClick()
        {
            switch (operation)
            {
                case DownLoadOperation.DownLoad:
                    break;
                case DownLoadOperation.Cancel:
                    break;
                case DownLoadOperation.Pause:
                    break;
                case DownLoadOperation.Continue:
                    break;
                case DownLoadOperation.Open:
                    break;
                default:
                    break;
            }
        }

        public static string ConvertStateToString(DownLoadState state)
        {
            string ret = string.Empty;

            switch(state)
            {
                case DownLoadState.None:
                    break;
                case DownLoadState.DownLoading:
                    break;
                case DownLoadState.Pause:
                    ret = "暂停";
                    break;
                case DownLoadState.Failure:
                    ret = "下载失败";
                    break;
                case DownLoadState.Complete:
                    ret = "下载完成";
                    break;
                default:
                    break;
            }

            return ret;
        }

        public static string ConvertOperationToString(DownLoadOperation operation)
        {
            string ret = string.Empty;

            switch (operation)
            {
                case DownLoadOperation.DownLoad:
                    ret = "下载";
                    break;
                case DownLoadOperation.Cancel:
                    ret = "取消";
                    break;
                case DownLoadOperation.Pause:
                    ret = "暂停";
                    break;
                case DownLoadOperation.Continue:
                    ret = "继续";
                    break;
                case DownLoadOperation.Open:
                    ret = "打开";
                    break;
                default:
                    break;
            }

            return ret;
        }*/

    //public Text scetionTxt;
    public Text contentTxt;
    public Text progressTxt;
    public Text btnTxt;
    //public string scetion;
    public string content;

    public Enums.ContentDataType suffixtype = Enums.ContentDataType.None;

    public DownLoadItemInfor infor;

    private int id = -1;

    //   // Use this for initialization
    //   void Start () {

    //}

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
            progressTxt.text = string.Format("{0}%", (int)(infor.GetProgress() * 100));
        }
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
        if (infor == null)
        {
            btnTxt.transform.parent.gameObject.SetActive(false);
            return;
        }

        string txt = string.Empty;

        switch (infor.state)
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
        if (infor == null)
        {
            return;
        }

        if (this.id < 0)
        {
            return;
        }

        if (this.content == null)
        {
            return;
        }

        switch (infor.state)
        {
            case DownLoadItemInfor.DownLoadState.None:
                infor.StartDownLoad();
                break;
            case DownLoadItemInfor.DownLoadState.DownLoading:
                break;
            case DownLoadItemInfor.DownLoadState.Complete:
                EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Enums.ContentDataType, DownLoadItemInfor>(EventId.OpenContent, this.suffixtype, this.infor);
                break;
            default:
                break;
        }

        ShowBtnText();
    }
}
