using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TinyFrameWork
{
    public enum HandleType
    {
        Add = 0,
        Remove = 1,
    }

    public class EventSystemDefine
    {
        public static Dictionary<int, string> dicHandleType = new Dictionary<int, string>()
        {
            { (int)HandleType.Add, "Add"},
            { (int)HandleType.Remove, "Remove"},
        };
    }

    public enum EventId
    {
        None = 0,
        // Test User Input
        TestUserInput,
        // UIFrameWork Event id
        PopRootWindowAdded,
        // Common Event id
        CoinChange,
        DiamondChange,
        // Player Common Event id
        PlayerHitByAI,
        // Net message
        NetUpdateMailContent,

        // 手柄操作相关
        triggerdown,

        // 网络相关
        ConnectedHub,

        //界面与界面数据通信相关
        SwitchMode,
        ChoosePerson,    // 选择个人
        ChooseGroup,    // 选择小组
        RefreshState,   //更新教学状态 教学模式、出勤率、点赞率
        TipMessage,     // 上课过程中的通知消息
        DoubtFeedBack,       // 学生疑问反馈
        TestFeedBack,   // 随堂测试反馈
        ResetScene,

        // 细节 界面相关
        ChooseQuestion,   // 选择了某个问题
    }
}
