using ko.NetFram;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class UiDataManager {

    public static UiDataManager getInstance()
    {
        return Singleton<UiDataManager>.getInstance();
    }

    // 提供界面调用接口

    // 计算出勤率
    public string CalculateTheAttendance()
    {
        string rate = null;

        rate = studentonlinecount.ToString() + "/" + studentallcount.ToString();

        return rate;
    }

    // 获取点赞次数显示
    public string GetLikeCount()
    {
        string ret = null;

        ret = likecount.ToString() + "/" + studentonlinecount.ToString();

        return ret;
    }

    // 获取小组信息
    public Dictionary<string, GroupInfor> GetGroupList()
    {
        return grouplist;
    }

    // 获取小组信息
    public List<string> GetGroupNameList()
    {
        List<string> groupname = new List<string>();

        foreach(string name in grouplist.Keys)
        {
            groupname.Add(name);
        }
        return groupname;
    }

    // 获取小组成员信息
    public Dictionary<string, PlayerInfor> GetGroupMemeber(string groupname)
    {
        if(grouplist.Count <= 0 || !grouplist.ContainsKey(groupname))
        {
            return null;
        }

        if(groupname == null)
        {
            //默认获取第一个小组信息
            return null;
        }

        return grouplist[groupname].members;
    }

    // 获取所有成员列表
    public Dictionary<string, PlayerInfor> GetAllPlayerList()
    {
        return playerlist;
    }

    // 界面改变当前模式
    public void SwitchTeachingMode(Enums.TeachingMode mode)
    {
        if(this.mode != mode)
        {
            this.mode = mode;
        }
    }

    // 提供见面调用 和服务器交互的接口

    // 主动通知界面更新数据

    // 界面 和 服务器的交互

    public void ChooseGroup(string token)
    {
        switch (operateattribute)
        {
            case Enums.OperatingAttribute.ResetScene:
                break;
            case Enums.OperatingAttribute.SwitchMode:
                break;
            default:
                break;
        }
    }

    public void ChoosePerson(string token)
    {
        switch (operateattribute)
        {
            case Enums.OperatingAttribute.ResetScene:
                break;
            case Enums.OperatingAttribute.SwitchMode:
                break;
            default:
                break;
        }
    }

    // 重置场景
    public void ResetScene(int id)
    {
        operateattribute = Enums.OperatingAttribute.ResetScene;
        switch (id)
        {
            case 0:
                // 重置全部
                break;
            case 1:
                // 重置组
                break;
            case 2:
                // 重置学生
                break;
            default:
                break;
        }
    }

    // 重置所有人的场景
    public void ResetSceneAll()
    {
        // TODO
    }

    // 随堂测试相关  ---- --- -- -随堂测试相关
    public void TestInClass(int id)
    {
        // TODO
    }

    public void FastTestInClass(int id)
    {
        // TODO
    }

    public void AskTestInClass(int id)
    {
        // TODO
    }

    // 网络数据返回
    public void TestFeedBack(int option)
    {
        //
    }

    // 注册事件监听函数
    public void RegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchTeachingMode);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.ResetScene, this.ResetScene);
        EventDispatcher.GetInstance().MainEventManager.AddEventListener<int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    // 取消注册事件监听函数
    public void UnRegisterEventListener()
    {
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<Enums.TeachingMode>(EventId.SwitchMode, this.SwitchTeachingMode);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.ResetScene, this.ResetScene);
        EventDispatcher.GetInstance().MainEventManager.RemoveEventListener<int>(EventId.TestFeedBack, this.TestFeedBack);
    }

    public Enums.TeachingMode mode;
    public Dictionary<string, GroupInfor> grouplist = new Dictionary<string, GroupInfor>(); // 根据名字或者token的字典索引小组列表
    public Dictionary<string, PlayerInfor> playerlist = new Dictionary<string, PlayerInfor>(); // 所有学生列表
    public int groupcount;
    public int studentallcount;
    public int studentonlinecount;
    public int likecount;

    // 操作状态信息
    public Enums.OperatingAttribute operateattribute = Enums.OperatingAttribute.None;
}
