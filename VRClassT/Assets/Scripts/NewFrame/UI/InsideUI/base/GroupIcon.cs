using System.Collections;
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
        RectTransform rf = gameObject.GetComponent<RectTransform>();
        Vector3 p = rf.anchoredPosition3D;
        p.z = 0;
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = p;

        rf.localRotation = Quaternion.identity;
        rf.localScale = Vector3.one;
    }

    public void Init(string name, int num, ToggleGroup tog)
    {
        this.Init(name, num);
        SetToggleGroup(tog);
    }

    public void SetToggleGroup(ToggleGroup tog)
    {
        if(tog == null)
        {
            return;
        }

        try
        {
            Transform go = transform.GetChild(0);
            Toggle to = go.GetComponent<Toggle>();
            to.group = tog;
        }catch
        {

        }
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
