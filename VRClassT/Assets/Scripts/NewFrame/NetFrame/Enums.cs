using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums {

    // 网络相关


    /// <summary>
    /// 登陆类型 区分 玩家 大屏 节点服务器
    /// </summary>
    public enum LoginType
    {
        None = -1,

        Player,
        Screen,
        NodeServer,
    }

    /// <summary>
    /// 用于和服务器交互的 五大教学模式  及其细化
    /// </summary>
    public enum TeachingMode
    {
        None = 0,
        WatchLearnModel_Sync = 2,                                       // 观学模式  同步
        WatchLearnModel_Async,                                          // 观学模式  异步
        GuidanceMode_Personal = 4,                                      // 指导模式  个人
        GuidanceMode_Group,                                             // 指导模式  小组
        SelfTrain_Personal = 8,                                         // 自主训练模式 个人
        SelfTrain_Group,                                                // 自主训练模式 小组
        SelfTrain_All,                                                  // 自主训练模式 全部
        VideoOnDemand_Full = 16,                                        // 视频点播 全景
        VideoOnDemand_General,                                          // 视频点播 普通
        VideoOnLive_Full,                                               // 视频直播 全景
        VideoOnLive_General,                                            // 视频直播 普通
    }

    /// <summary>
    /// 物体网络状态
    /// </summary>
    public enum ObjectState
    {
        None = -1,

        CanReceive,
        CanSend,
    }

    /// <summary>
    /// 指令定义中 ObjectOperate （多人协同下的指令） 的具体操作指令
    /// </summary>
    public enum ObjectOperate
    {
        None = -1,

        Hold,
        Release,
    }
}
