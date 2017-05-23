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
            Separate = 2,     // IndependentMode
            SynchronousOne = 4,   // WatchAndLearnModel
            SynchronousMultiple = 8,   // GuidanceMode
            Collaboration = 16,   // CollaborativeModel
        }

        public enum DivideGroupRules
        {
            None = 0,
            Grade,
            Random,
        }
    }
}
