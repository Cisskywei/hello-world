using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComonEnums
{

	public enum SexEnum
    {
        None = 0,
        Male,
        Female,
    }

    public enum DutyEnum
    {
        None = 0,
        Student,
        GroupLeader,
        Assistant,
        Teacher,

        // 大屏和节点服务器
        BigScreen,
        NodeServer,
    }

    // 与界面 服务器 的交互 需要的枚举

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
    /// 随堂测验的试题类型
    /// </summary>
    public enum InClassTestType
    {
        Test = 1,    // 测试
        Fast,        // 抢答
        Ask,         // 提问
    }

    /// <summary>
    /// 分组规则枚举
    /// </summary>
    public enum DivideGroupRules
    {
        None = 0,
        Grade,
        Random,
    }

    /// <summary>
    /// 操作对象的范围
    /// </summary>
    public enum OperatingRange
    {
        None = 0,
        Personal,
        Team,
        All,
    }

    /// <summary>
    /// 当前操作状态 是 切换模式的人物选择 还是 重置场景的人物选择
    /// </summary>
    public enum OperatingAttribute
    {
        None = 0,
        SwitchMode,
        ResetScene,
    }

    public enum QuestionType
    {
        None = 0,
        TrueOrFalse,
        SingleChoice,
        MultipleChoice,
        ShortAnswer,
    }

    public enum ResetSceneType
    {
        All = 0,
        Group,
        Student,
    }

    public enum ContentDataType
    {
        None = -1,
        Exe = 0,
        PanoramicVideo,
        OrdinaryVideo,
        PPt,
        Docx,
        Txt,
        Panorama, // 全景图
        Picture,
        Zip, // 压缩文件  用于exe
    }
}
