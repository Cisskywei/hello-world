using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TinyFrameWork;

public class syncCommond : MonoBehaviour, sync_commond
{

    // Use this for initialization
    void Start()
    {
        MsgModule.getInstance().registerSyncCommond(this);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    // 处理手势消息
    public void dealhandctrl(string commond, string name, string objectname)
    {
        //if(commond == "close")
        //{
        //    if(ac != null)
        //    {
        //        ac.shrink();
        //    }
        //}else if (commond == "open")
        //{
        //    if (ac != null)
        //    {
        //        ac.expend();
        //    }
        //}

        EventDispatcher.GetInstance().MainEventManager.TriggerEvent(EventId.triggerdown);
    }

    public void ret_sync_commond(string typ, string commond, string name, string objectname)
    {
        Debug.Log("同步指令收到了呦");
        switch (typ)
        {
            case "animation":
                this.dealhandctrl(commond, name, objectname);
                break;
            default:
                break;
        }
    }
}
