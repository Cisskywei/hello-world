using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetConfig {

    public static string service_ip = "192.168.0.112";//"127.0.0.1";//"221.226.219.155";//"192.168.0.101";//"58.213.74.230"; // 192.168.157.162
    public static short service_port = 3236;//1800;

    public static string udp_ip = "192.168.0.112";//"127.0.0.1";
    public static short udp_port = 3237;

    // rpc 调用相关
    public static string lobby_module_name = "lobby";
    public static string class_module_name = "vrClass";

    // rpc 函数调用相关
    public static string Login_func = "PlayerLogin";                              // 登陆
    public static string Enter_Lab_func = "PlayerEnterLab";                       // 进入实验室
    public static string Enter_Course_func = "PlayerEnterCourse";                 // 进入课程
    public static string Enter_CourseWare_func = "EnterCourseWare";         // 进入课件

    public static string Back_Lobby_func = "BackLobby";                     // 返回课程大厅
    public static string Back_Course_func = "BackCourse";                   // 返回课程选择界面
    public static string Back_Lab_func = "BackLab";                         // 返回实验室选择
    public static string Exit_func = "PlayerExit";                                // 退出

    // 课件之间的交互 帧同步等等

    public static string InitScene_func = "InitScene";                      // 进入课件里的初始化场景
    public static string ChangeAllOnce_func = "ChangeClientAllOnce";        // 客户端一次发送修改的数据
    public static string Command_func = "Command";                          // 指令传输
    public static string Pipe_func = "ReceivePipeData";                          // 管道传输

    // 其他统一通过指令控制

}
