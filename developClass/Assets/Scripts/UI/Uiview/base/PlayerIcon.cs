using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour {

    public Text name;
    public Text duty;
    public Image icon;

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

        this.name.text = name;
        this.duty.text = "(" + duty + ")";

        // 设置图片
        //TODO
    }
}
