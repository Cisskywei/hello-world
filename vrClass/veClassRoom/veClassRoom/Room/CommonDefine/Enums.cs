using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 公共的枚举类
    /// </summary>
    class Enums
    {
        /// <summary>
        /// 权限定义
        /// </summary>
        public enum PermissionEnum
        {
            None = 0,
            Student = 2,
            Group = 4,
            Teacher = 8,
            Max = 16,
        }

        public enum PermissionVerifyStatus
        {
            None = 0,
            Lower,
            Equal,
            Higher,
        }

        /// <summary>
        /// 智慧教室的模式
        /// </summary>
        public enum ModelEnums
        {
            None = 0,
            Separate = 2,     // IndependentMode  各自独立
            SynchronousOne = 4,   // WatchAndLearnModel   老师操作学生看  异步
            SynchronousOne_Fixed,   // WatchAndLearnModel   老师操作学生看  同步
            SynchronousMultiple = 8,   // GuidanceMode  老师选择学生或者小组操作
            Collaboration = 16,   // CollaborativeModel  多人协作
        }

        /// <summary>
        /// 与控制玩家操作 无关  用于和客户端交互的 五大教学模式  及其细化
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
    }
}
