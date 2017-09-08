using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class NetConfig
    {
        // rpc 调用相关
        public static string client_module_name = "cMsgConnect";

        // rpc 函数调用相关
        public static string Login_func = "retPlayerLogin";                              // 登陆
        public static string reLogin_func = "retRePlayerLogin";                              // 登陆
        public static string Enter_Lab_func = "retPlayerEnterLab";                       // 进入实验室
        public static string Enter_Course_func = "retPlayerEnterCourse";                 // 进入课程
        //public static string Enter_CourseWare_func = "EnterCourseWare";         // 进入课件

        //public static string Back_Lobby_func = "BackLobby";                     // 返回课程大厅
        //public static string Back_Course_func = "BackCourse";                   // 返回课程选择界面
        //public static string Back_Lab_func = "BackLab";                         // 返回实验室选择
        public static string Exit_func = "retPlayerExit";                                // 退出
        public static string Error_func = "retErrorMsg";                                // 退出

        public static string InitScene_func = "RetInitScene";                      // 进入课件里的初始化场景
        public static string ChangeAllOnce_func = "RetChangeClientAllOnce";        // 客户端一次发送修改的数据
        public static string Command_func = "RetCommand";                          // 指令传输
        //public static string ChangeModel_func = "RetChangeModel";                          // 模式切换
        public static string Pipe_func = "RetPipe";                          // 管道传输
    }
}
