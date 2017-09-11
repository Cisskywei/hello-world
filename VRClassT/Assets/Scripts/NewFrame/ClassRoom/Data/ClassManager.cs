using ko.NetFram;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class ClassManager {

    public static ClassManager getInstance()
    {
        return Singleton<ClassManager>.getInstance();
    }

    // 当前课程德基本心息
    public string course_id;
    //public string mode;
    public string course_name;
    public string course_teacher;
    public string school;
    //public string background;
    //public string introduce;
    //public string target;
    //public string cover;
    //public string video;

    // 房间老师
    public PlayerInfor teacher;

    public Dictionary<int, ClassInfor> classlist = new Dictionary<int, ClassInfor>();

    // 为学生用户·保存当前玩家信息
    private ArrayList _playeronline;

    // 初始化所有班级
    public void InitClass(DataType.CourseInfor courseinfor = null)
    {
        if(courseinfor == null)
        {
            return;
        }

        this.course_id = courseinfor.course_id;
        this.course_name = courseinfor.course_name;
        this.course_teacher = courseinfor.course_teacher;
        this.school = courseinfor.school;

        // 初始化班级
        DataType.StudentInfor[] students = courseinfor.students;

        if(students == null || students.Length <= 0)
        {
            return;
        }

        for(int i=0;i<students.Length;i++)
        {
            PlayerInfor p = new PlayerInfor(students[i]);
            ClassInfor ci = findClassById(Convert.ToInt32(p.classes.class_id));
            ci.AddStudent(p);
        }
    }

    public void InitTeacher()
    {

    }

    // 寻找class 如果没有就生成一个
    private ClassInfor findClassById(int id)
    {
        if(classlist.Count <= 0 || !classlist.ContainsKey(id))
        {
            ClassInfor ci = new ClassInfor();
            ci.InitClass(id);
            classlist.Add(id, ci);
        }

        return classlist[id];
    }

    public ClassInfor GetClassById(int id)
    {
        if(classlist.ContainsKey(id))
        {
            return classlist[id];
        }

        return null;
    }

    public Dictionary<int, ClassInfor> GetAllClass()
    {
        return classlist;
    }

    public List<ClassInfor> GetAllClassList()
    {
        return new List<ClassInfor>(classlist.Values);
    }

    // 有新的学生上线  arraylist: userid/name/duty/sex/avator/classid
    public void Playeronline(ArrayList playerinfor)
    {
        Int64 userid = (Int64)playerinfor[0];
        string name = (string)playerinfor[1];
        Int64 duty = (Int64)playerinfor[2];
        string sex = (string)playerinfor[3];
        string ava = (string)playerinfor[4];
        Int64 classid = (Int64)playerinfor[5];
  //      PlayerInfor p = new PlayerInfor();
        ClassInfor ci = findClassById((int)classid);
        if(ci == null)
        {
            // TODO
        }
        else
        {
            ci.Playeronline((int)userid);
        }
    }

    // arraylist: {userid/name/duty/sex/avator/classid}
    public void Playersonline(ArrayList playersinfor)
    {
        for(int i=0;i<playersinfor.Count;i++)
        {
            ArrayList playerinfor = (ArrayList)playersinfor[i];
            Int64 userid = (Int64)playerinfor[0];
            string name = (string)playerinfor[1];
            Int64 duty = (Int64)playerinfor[2];
            string sex = (string)playerinfor[3];
            string ava = (string)playerinfor[4];
            Int64 classid = (Int64)playerinfor[5];
            ClassInfor ci = findClassById((int)classid);
            if (ci == null)
            {
                // TODO
            }
            else
            {
                ci.Playeronline((int)userid);
            }
        }
    }

    public PlayerInfor FindPlayerById(int userid)
    {
        if(classlist == null || classlist.Count <= 0)
        {
            return null;
        }

        PlayerInfor p = null;

        foreach(ClassInfor ci in classlist.Values)
        {
            if(ci.Count <= 0)
            {
                continue;
            }

            p = ci.FindPlayerById(userid);

            if(p != null)
            {
                break;
            }
        }

        return p;
    }

    // 初始化在线玩家列表
    public void InitOnlinePlayer(ArrayList users)
    {
        _playeronline = users;

        if(classlist == null || classlist.Count <= 0)
        {
            return;
        }

        for(int i=0; i<users.Count; i++)
        {
            Int64 id = (Int64)users[i];
            PlayerInfor pi = FindPlayerById((int)id);
            if(pi != null)
            {
                pi.isonline = true;
            }

            // 修改名字显示
            GameObject go = RoleManager.getInstance().GetPlayerById((int)id);
            if(go != null)
            {
                RoleCtrl rc = go.GetComponent<RoleCtrl>();
                if(rc != null)
                {
                    rc.Init((int)id,pi.student_name);
                }
            }
        }
    }

    // 获取所有玩家
    public Dictionary<int, PlayerInfor> GetAllPlayers()
    {
        if(classlist == null || classlist.Count <= 0)
        {
            return null;
        }

        Dictionary<int, PlayerInfor> players = new Dictionary<int, PlayerInfor>();

        foreach (ClassInfor ci in classlist.Values)
        {
            ci.GetAllPlayers(ref players);
        }

        return players;
    }

    private Dictionary<string, GroupInfor> groups = new Dictionary<string, GroupInfor>();
    public Dictionary<string, GroupInfor> GetAllGroups()
    {
        if(groups.Count <= 0)
        {
            GroupInfor gi = new GroupInfor("组1");
            gi.members = GetAllPlayers();
            groups.Add("组1", gi);
        }
        return null;
    }

    public Dictionary<int, PlayerInfor> GetMembersOfGroup(string groupname)
    {
        if (groups.Count <= 0)
        {
            GroupInfor gi = new GroupInfor("组1");
            gi.members = GetAllPlayers();
            groups.Add("组1", gi);
        }
        return null;
    }
}
