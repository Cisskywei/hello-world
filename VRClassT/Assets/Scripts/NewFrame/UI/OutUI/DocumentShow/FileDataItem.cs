using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileDataItem : MonoBehaviour {

    public delegate void OnClickListen(DownLoadItemInfor fileinfor);
    public OnClickListen onclickcallback;

    public Text contentTxt;

    private DownLoadItemInfor _fileinfor;

    public void Init(DownLoadItemInfor infor)
    {
        contentTxt.text = infor.filename;
        _fileinfor = infor;
    }

    public void Init(DownLoadItemInfor infor, OnClickListen callback)
    {
        contentTxt.text = infor.filename;
        _fileinfor = infor;

        onclickcallback = callback;
    }

    public void OnClick()
    {
        if(onclickcallback != null)
        {
            onclickcallback(_fileinfor);
        }
    }

    public void RegisteOnClickListener(OnClickListen callback)
    {
        onclickcallback = callback;
    }
}
