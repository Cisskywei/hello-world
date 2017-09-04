using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDownLoadIcon : MonoBehaviour {

    public Text nameTxt;
    public Text downTxt;
    public Image iconImg;

    private Int64 _id;
    private string avatar;

    public void Init(PlayerInfor infor, string downloadtext = "下载进度")
    {
        this.avatar = infor.avatar;

        if (gameObject.activeInHierarchy)
        {
            InitHeadIcon(this.avatar);
        }

        nameTxt.text = infor.name;
        downTxt.text = downloadtext;
        _id = infor.userid;
        this.avatar = infor.avatar;

        if (infor.isonline)
        {
            Utility.getInstance().SetImageGray(iconImg, 1);
        }
        else
        {
            Utility.getInstance().SetImageGray(iconImg);
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

    private void OnEnable()
    {
        InitHeadIcon(this.avatar);
    }

    public void InitHeadIcon(string avatar)
    {
        if (UserInfor.RootUrl != null && avatar != null)
        {
            string url = UserInfor.RootUrl + avatar; // avatar; UserInfor.getInstance().courseinfor_rooturl
            Debug.Log(url);
            StartCoroutine(LoadIamge(url));
        }
    }

    public void ChooseSelf()
    {
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64>(EventId.ChoosePerson, this._id);
    }

    /// 辅助函数
    IEnumerator LoadIamge(string path)
    {
        WWW www = new WWW(path);
        yield return www;
        Texture2D texture = www.texture;

        if (texture != null)
        {
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (iconImg != null)
            {
                iconImg.overrideSprite = sp;
            }
        }

    }
}
