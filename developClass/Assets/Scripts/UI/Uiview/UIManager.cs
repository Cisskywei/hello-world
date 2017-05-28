using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private bool _isTeacher = false; // 标记是否是老师 用于控制界面的显示 隐藏
    public bool IsTeacher
    {
        get
        {
            return _isTeacher;
        }

        set
        {
            _isTeacher = value;

            InitUIByDuty();
        }
    }

    ///    ui 界面预设
    // 小组信息界面
    public GroupListUI groups;
    // 成员信息界面
    public PlayerListUI players;

	// Use this for initialization
	void Start () {
		
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}

    private void InitUIByDuty()
    {
        if(_isTeacher)
        {
            // 显示老师相关界面
        }
        else
        {
            // 显示学生相关界面
        }
    }

    void OnEnable()
    {
        RegisterEvent();
        UiDataManager.getInstance().RegisterEventListener();
    }

    void OnDisable()
    {
        UnRegisterEvent();
        UiDataManager.getInstance().UnRegisterEventListener();
    }
    /// <summary>
    /// register the target event message, set the call back method with params and event name.
    /// </summary>
    public void RegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Enums.InClassTestType, int>(EventId.ChooseQuestion, this.ChooseQuestion);
    }

    /// <summary>
    /// unregister the target event message.
    /// </summary>
    public void UnRegisterEvent()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Enums.InClassTestType,int>(EventId.ChooseQuestion, this.ChooseQuestion);
    }

    // 监听函数
    public void SwitchMode(Enums.TeachingMode mode)
    {
        // 判断弹出对应界面
        switch(mode)
        {
            case Enums.TeachingMode.WatchLearnModel_Sync:
                break;
            case Enums.TeachingMode.WatchLearnModel_Async:
                break;
            case Enums.TeachingMode.GuidanceMode_Personal:
                //弹出学生列表界面
                break;
            case Enums.TeachingMode.GuidanceMode_Group:
                //弹出小组选择界面
                break;
            case Enums.TeachingMode.SelfTrain_Personal:
                //弹出学生列表界面
                break;
            case Enums.TeachingMode.SelfTrain_Group:
                //弹出小组选择界面
                break;
            case Enums.TeachingMode.SelfTrain_All:
                break;
            case Enums.TeachingMode.VideoOnDemand_General:
                break;
            case Enums.TeachingMode.VideoOnDemand_Full:
                break;
            case Enums.TeachingMode.VideoOnLive_General:
                break;
            case Enums.TeachingMode.VideoOnLive_Full:
                break;
            default:
                break;
        }
    }

    // 选择了某个试题对应的弹出相应界面
    public void ChooseQuestion(Enums.InClassTestType typ, int questionid)
    {
        switch(typ)
        {
            case Enums.InClassTestType.Test:
                //测试反馈
                break;
            case Enums.InClassTestType.Ask:
                // 学生选择界面
                break;
            case Enums.InClassTestType.Fast:
                // 抢答学生界面
                break;
            default:
                break;
        }
    }

    //接收学生点赞 举手
    public void ReceiveLike()
    {
        // TODO
    }

    public void ReceiveDoubt()
    {
        // TODO
    }

    // 学生点赞 举手
    public void SendLike()
    {
        // TODO
    }

    public void SendDoubt()
    {
        // TODO
    }

    // 中间的按钮模块
    // 测试按钮
    public void TestBtn()
    {

    }
    // 抢答按钮
    public void FastBtn()
    {

    }
    //提问按钮
    public void AskBtn()
    {

    }
    //返回大厅
    public void BackLobby()
    {
        // 返回大厅
    }

    // 重置场景
    public Dropdown reset;
    public void ResetScene()
    {
        if(reset == null)
        {
            return;
        }

        switch (reset.value)
        {
            case 0:
                // 重置全部
                UiDataManager.getInstance().ResetSceneAll();
                break;
            case 1:
                // 重置组
                // 显示组选择界面
                break;
            case 2:
                // 重置学生
                // 显示所有学生界面
                break;
            default:
                break;
        }
    }
}
