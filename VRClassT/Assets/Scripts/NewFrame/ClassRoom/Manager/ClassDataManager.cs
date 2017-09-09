using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理教室题目数据 学生人员信息
/// </summary>
public class ClassDataManager {

    public static ClassDataManager getInstance()
    {
        return Singleton<ClassDataManager>.getInstance();
    }

    //课程列表
    public DataType.CourseListData[] courselist;
    public string courselist_rooturl;
    public DataType.CourseInfor courseinfor;
    public string courseinfor_rooturl;
    public static string RootUrl = "http://www.hdmool.com";

    // 根据学生信息寻找学生班级
    public int FindClassOfStudent(string studentid)
    {
        int classid = -1;

        if(courseinfor == null)
        {
            return classid;
        }

        DataType.StudentInfor[] si = courseinfor.students;

        for(int i=0;i<si.Length;i++)
        {
            if(si[i].user_id == studentid)
            {
                classid = Convert.ToInt32(si[i].classes.class_id);
                break;
            }
        }

        return classid;
    }
}
