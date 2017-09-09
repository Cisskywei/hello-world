using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;
using ko.NetFram;

public class PlayerIcon : MonoBehaviour {

    public Text nameTxt;
    public Text dutyTxt;
    public Image iconImg;

    private string _name;
    private string _duty;
    private int _iconid;
    private Int64 _id;

    private string avatar;

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public void Init(string name, string duty, int icon, Int64 userid)
    {
        this._name = name;
        this._duty = duty;
        this._iconid = icon;
        this._id = userid;

        this.nameTxt.text = name;
        this.dutyTxt.text = "(" + duty + ")";

        // 设置图片
        //TODO

        // 设置位置 防止位置改变
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;
    }

    public void Init(string name, string duty, int icon, Int64 userid, ToggleGroup tog)
    {
        Init(name, duty, icon, userid);

        if(tog != null)
        {
            SetToggleGroup(tog);
        }
    }

    public void Init(PlayerInfor infor, ToggleGroup tog)
    {
        this.avatar = infor.avatar;

        if(gameObject.activeInHierarchy)
        {
            InitHeadIcon(this.avatar);
        }

        Init(infor.name, infor.GetDuty(), infor.iconid, infor.userid, tog);

        if(infor.isonline)
        {
            Utility.getInstance().SetImageGray(iconImg,1);
        }else
        {
            Utility.getInstance().SetImageGray(iconImg);
        }
    }

    // 抢答初始化
    public void Init(string name, string avatar)
    {
        this.avatar = avatar;

        if (gameObject.activeInHierarchy)
        {
            InitHeadIcon(this.avatar);
        }

        this.name = name;
        this.nameTxt.text = name;
    }

    private void OnEnable()
    {
        InitHeadIcon(this.avatar);
    }

    public void InitHeadIcon(string avatar)
    {
        if (ClassDataManager.RootUrl != null && avatar != null)
        {
            string url = ClassDataManager.RootUrl + avatar; // avatar; UserInfor.getInstance().courseinfor_rooturl
            Debug.Log(url);
            StartCoroutine(LoadIamge(url));
        }
    }

    public void SetToggleGroup(ToggleGroup tog)
    {
        if (tog == null)
        {
            return;
        }

        try
        {
            if(iconImg == null)
            {
                Transform go = transform.GetChild(0);
                iconImg = go.GetComponent<Image>();
                Toggle to = go.GetComponent<Toggle>();
                to.group = tog;
            }
            else
            {
                Toggle to2 = iconImg.gameObject.GetComponent<Toggle>();
                to2.group = tog;
            }
            
        }
        catch
        {

        }
    }

    public void ChooseSelf(Toggle go)
    {
        if(!go.isOn)
        {
            return;
        }
        Debug.Log("选择人物");
        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<Int64>(EventId.ChoosePerson, this._id);
    }

    /// 辅助函数
    IEnumerator LoadIamge(string path)
    {
        WWW www = new WWW(path);
        yield return www;
        Texture2D texture = www.texture;

        if(texture != null)
        {
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (iconImg != null)
            {
                Debug.Log("替换图片");
                iconImg.overrideSprite = sp;
            }
        }
        
    }
}
