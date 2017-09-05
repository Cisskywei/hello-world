using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class ClassIcon : MonoBehaviour {

    public Text nameTxt;
    public Text numTxt;

    private string _name;
    private int num;
    private int classid = -1;

    public void Init(string name, int num)
    {
        this._name = name;
        this.num = num;

        nameTxt.text = name;
        numTxt.text = num.ToString()+"人";

        // 设置位置 防止位置改变
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;
    }

    // 直接文字初始化
    public void Init(string name, string group, int classid)
    {
        this._name = name;

        nameTxt.text = name;
        numTxt.text = group;
        this.classid = classid;

        // 设置位置 防止位置改变
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;
    }

    public void Init(ClassInfor ci)
    {
        if (ci == null)
        {
            return;
        }
        Init(ci.classname, ci.Count);

        this.classid = ci.classid;
    }

    public void OnClick()
    {
        if (classid < 0)
        {
            return;
        }

        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<int>(EventId.ChooseClass, this.classid);
    }
}
