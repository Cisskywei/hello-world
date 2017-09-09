using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PushDataItem : MonoBehaviour {

    public Text filenameTxt;
    public Text btnTxt;

    private int fileid;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void Init(DownLoadItemInfor infor)
    {
        this.fileid = infor.fileid;
        filenameTxt.text = infor.filename;
        
        switch(infor.state)
        {
            case DownLoadItemInfor.DownLoadState.None:
                btnTxt.text = "推送";
                break;
            case DownLoadItemInfor.DownLoadState.Complete:
                btnTxt.text = "打开";
                break;
            default:
                btnTxt.text = "推送";
                break;
        }

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

    public void OnClick()
    {
        if(this.fileid < 0)
        {
            return;
        }

        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.DownLoadFileItem,this.fileid);
    }
}
