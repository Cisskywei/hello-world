using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 程序初始化 关于网络数据的监听函数注册
/// </summary>
public class GameCommandInit {

    public static GameCommandInit getInstance()
    {
        return Singleton<GameCommandInit>.getInstance();
    }

    //public delegate void GCI_VoidCallBack();
    //public GCI_VoidCallBack residentorderInit;

    public void InitResidentOrder()
    {
        BigScreen.getInstance().AddListener();
    }

    public void RemoveResidentOrder()
    {
        BigScreen.getInstance().RemoveListener();
    }
}
