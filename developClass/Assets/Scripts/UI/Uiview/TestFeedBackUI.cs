using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class TestFeedBackUI : uibase {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEventListener();
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    public void TestFeedBack(int option)
    {
        // 答题返回
    }
}
