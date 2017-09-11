using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenFileManager : MonoBehaviour {

    private static GameObject selfgo;
    private static OpenFileManager _instance;
    public static OpenFileManager getInstance()
    {
        if (_instance == null)
        {
            if (selfgo == null)
            {
                selfgo = GameObject.Find("OpenFileManager");
            }

            if (selfgo != null)
            {
                _instance = selfgo.GetComponent<OpenFileManager>();
            }
        }

        return _instance;

    }

    public delegate void VoidCallBack();

    public PlayOrdinaryVideo playervideo;
    public PlayPanoramicVideo player360video;
    public PPtShowUI showppt;
    public OpenVrExe openexe;

    private void Awake()
    {
        selfgo = gameObject;
        _instance = gameObject.GetComponent<OpenFileManager>();
    }

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // ppt 转换监听操作
    //    if(pptcache.Count>0)
    //    {
    //        PPtData p = pptcache.Dequeue();
    //        ConvertPPt(p.filename, p.typ, p.filefullpath);
    //    }
    //}

    //public class PPtData
    //{
    //    public string filename;
    //    public string typ;
    //    public string filefullpath;

    //    public PPtData(string filename, string typ, string filefullpath)
    //    {
    //        this.filename = filename;
    //        this.typ = typ;
    //        this.filefullpath = filefullpath;
    //    }
    //}

    //private Queue<PPtData> pptcache = new Queue<PPtData>();

    //public void ConvertPPt(string filename, string type, string filefullpath)
    //{
    //    // 如果是ppt执行转换操作
    //    ComonEnums.ContentDataType suffixtype = FileManager.getInstance().GetFileContenType(filename, type);
    //    if (suffixtype == ComonEnums.ContentDataType.PPt)
    //    {
    //        string imagepath = FileManager.getInstance().GetPPTImagePath(filename);

    //        PPTToPicture.getInstance().ConvertPPT2Image(filefullpath, imagepath);

    //        filefullpath = imagepath;
    //    }
    //}

    //public void AddPPt(string filename, string typ, string filefullpath)
    //{
    //    PPtData p = new PPtData(filename, typ, filefullpath);
    //    pptcache.Enqueue(p);
    //}

    private GameObject needhideui = null;
    public void OpenOrdinaryVideo(string path, GameObject needhideui = null)
    {
        if(playervideo == null)
        {
            return;
        }

        if(playervideo.closecallback == null)
        {
            playervideo.closecallback = this.CloseOrdinaryVideo;
        }

        playervideo.Open(path);

        if(needhideui != null)
        {
            needhideui.GetComponent<OutUIBase>().HideSelf();
            this.needhideui = needhideui;
        }
    }

    public void CloseOrdinaryVideo()
    {
        if(needhideui != null)
        {
            needhideui.GetComponent<OutUIBase>().ShowSelf();
            needhideui = null;
        }
    }

    public void OpenPanoramicVideo(string path, GameObject needhideui = null)
    {
        if (player360video == null)
        {
            return;
        }

        if (player360video.closecallback == null)
        {
            player360video.closecallback = this.ClosePanoramicVideo;
        }

        player360video.Open(path);

        if (needhideui != null)
        {
            needhideui.GetComponent<OutUIBase>().HideSelf();
            this.needhideui = needhideui;
        }
    }

    public void ClosePanoramicVideo()
    {
        if (needhideui != null)
        {
            needhideui.GetComponent<OutUIBase>().ShowSelf();
            needhideui = null;
        }
    }

    public void OpenPPt(string path, OutUIBase needhideui = null)
    {
        if (showppt == null)
        {
            return;
        }

        if (showppt.closecallback == null)
        {
            showppt.closecallback = this.ClosePPt;
        }

        showppt.Open(path);

        if (needhideui != null)
        {
            needhideui.HideSelf();
        }
    }

    public void ClosePPt()
    {
        if (needhideui != null)
        {
            needhideui.GetComponent<OutUIBase>().ShowSelf();
            needhideui = null;
        }
    }

    public void OpenExe(string path)
    {
        // 只为演示测试
        SceneManager.LoadScene("jisuanji");
        //if(openexe == null)
        //{
        //    return;
        //}

        //openexe.Open(path);
    }
}
