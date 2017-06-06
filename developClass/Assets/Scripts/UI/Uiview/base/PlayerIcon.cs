using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour {

    public Text nameTxt;
    public Text dutyTxt;
    public Image iconImg;

    private string _name;
    private string _duty;
    private int _iconid;

    //// Use this for initialization
    //void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    public void Init(string name, string duty, int icon)
    {
        this._name = name;
        this._duty = duty;
        this._iconid = icon;

        this.nameTxt.text = name;
        this.dutyTxt.text = "(" + duty + ")";

        // 设置图片
        //TODO

        // 设置位置 防止位置改变
        Vector3 p = gameObject.GetComponent<RectTransform>().anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;
    }

    public void Init(string name, string duty, int icon, ToggleGroup tog)
    {
        Init(name, duty, icon);
        SetToggleGroup(tog);
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

        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<string>(EventId.ChoosePerson, this._name);
    }
}
