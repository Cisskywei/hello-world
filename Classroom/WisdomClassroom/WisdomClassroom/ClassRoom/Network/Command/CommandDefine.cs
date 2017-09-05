using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class CommandDefine
    {
        // 一级指令 限定操作范围
        public enum FirstLayer
        {
            None = -1,

            Login = 0,                      // 登陆面板

            Lab,                            // 实验室选择面板
            Course,                         // 课程选择面板
            Lobby,                          // 教学大厅
            CourseWave,                     // 课件内部

            Exit,
        }

        // 二级指令 限定具体操作
        public enum SecondLayer
        {
            None = -1,

            Login,

            //TODO
            InitScene,
            Hold,
            Release,
            ChangeMode,

            BigScreen,   // 大屏显示相关指令

            Exit,
        }
    }
}
