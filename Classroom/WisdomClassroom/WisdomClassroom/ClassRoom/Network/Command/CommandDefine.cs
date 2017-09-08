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

            All,

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

            //课程资料 题目 等的获取
            QuestionList,
            OnlinePlayers,
            OnlineOnePlayer,
            MaterialList,
            PushDataAll,
            PushDataOne,

            // 答题 打开文件 等交互
            OpenContent,
            OpenPPt,
            VideoCtrl,
            PPtCtrl,

            TestInClass,
            AnswerQuestion,
            EndQuestion,

            // 老师学生交互
            StudentDoubt,
            StudentLike,

            BigScreen,   // 大屏显示相关指令

            // 多人协同下的指令
            ObjectOperate,   // 物体的操作相关  对应 IObjectOperate 接口的相关函数  arraylist : 一级指令/二级指令/objectid/ObjectOperate类型/userid/操作结果/其他参数
            PlayerOrder, // 玩家模型的操作指令

            // 返回
            BackToLobby,

            Exit,
        }
    }
}
