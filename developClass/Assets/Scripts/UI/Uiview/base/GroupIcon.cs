﻿using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class GroupIcon : MonoBehaviour {

    public Text namenum;

    private string _name;
    private int num;

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    public void Init(string name, int num)
    {
        this._name = name;
        this.num = num;

        namenum.text = name + "(" + num.ToString() + "人" + ")";

        // 设置位置 防止位置改变
        Vector3 p = gameObject.GetComponent<RectTransform>().anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;
    }

    public void ChooseSelf(Toggle go)
    {
        if(!go.isOn)
        {
            return;
        }

        EventDispatcher.GetInstance().MainEventManager.TriggerEvent<string>(EventId.ChooseGroup, this._name);
    }
}
