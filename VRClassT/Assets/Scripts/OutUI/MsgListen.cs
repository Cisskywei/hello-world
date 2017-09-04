using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class MsgListen : MonoBehaviour {

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
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.OpenContentStudent, this.OpenContent);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.OpenContentStudent, this.OpenContent);
    }

    // 监听老师操作相关
    private void OpenContent(int fileid)
    {
        Debug.Log("学生端打开文件 " + fileid);

        DownLoadItemInfor infor = DownLoadDataManager.getInstance().GetContentById(fileid);
        Enums.ContentDataType typ = FileManager.getInstance().GetFileContenType(infor.filename, infor.type);

        switch (typ)
        {
            case Enums.ContentDataType.Exe:
                OpenFileManager.getInstance().OpenExe(infor.fullfilepath);
                break;
            case Enums.ContentDataType.PanoramicVideo:
                OpenFileManager.getInstance().OpenPanoramicVideo(infor.fullfilepath);
                break;
            case Enums.ContentDataType.OrdinaryVideo:
                OpenFileManager.getInstance().OpenOrdinaryVideo(infor.fullfilepath, gameObject);
                break;
            case Enums.ContentDataType.Panorama:
                break;
            case Enums.ContentDataType.Picture:
                break;
            case Enums.ContentDataType.PPt:
                OpenFileManager.getInstance().OpenPPt(infor.fullfilepath);
                OutUiManager.getInstance().ShowUI(OutUiManager.UIList.DrawingBoardUI);
                break;
            default:
                break;
        }
    }
}
