using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指令定义  二级指令定义  后期可扩展成三级 等
/// </summary>
public class CommandDefine {

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

        //课程资料 题目 等的获取
        QuestionList,
        OnlinePlayers,
        OnlineOnePlayer,
        MaterialList,
        PushDataAll,
        PushDataOne,

        // 答题 打开文件 等交互
        OpenContent,
        VideoCtrl,
        PPtCtrl,

        BigScreen,   // 大屏显示相关指令

        Exit,
    }

    // 客户端服务器hashTable msg传输id
    public enum HashTableMsgType
    {
        None = -1,

        retLogin,
        retEnterLab,
        retEnterCourse,
        retEnterLobby,
        retEnterCourseWave,

        retExit,
    }
}
