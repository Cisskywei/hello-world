using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 管理课程数据 包括学生列表 题目信息 等
    /// </summary>
    class ClassData
    {
        public BackDataType.CourseInforRetData.CourseInfor courseinfor;
        public Dictionary<int, UserInfor> allstudents = new Dictionary<int, UserInfor>();

        public void InitAllStudents(BackDataType.CourseInforRetData course)
        {
            if(course == null)
            {
                return;
            }

            courseinfor = course.data;

            if(courseinfor == null || courseinfor.students == null || courseinfor.students.Length <= 0)
            {
                return;
            }

            int id = 0;
            for(int i=0;i< courseinfor.students.Length;i++)
            {
                id = Convert.ToInt32(courseinfor.students[i].user_id);
                if (allstudents.ContainsKey(id))
                {
                    allstudents[id].InitByStudentInfor(courseinfor.students[i]);
                }
                else
                {
                    UserInfor u = new UserInfor();
                    u.InitByStudentInfor(courseinfor.students[i]);
                    allstudents.Add(id, u);
                }
            }
        }

        public UserInfor AddUserInfor(UserInfor u)
        {
            int id = u.selfid;
            if(allstudents.ContainsKey(id))
            {
                UserInfor old = allstudents[id];
                u.groupid = old.groupid;
                u.groupname = old.groupname;

                allstudents.Remove(id);
            }

            allstudents.Add(id, u);

            return u;
        }

        public UserInfor FindUserInforById(int id)
        {
            if(allstudents == null || allstudents.Count <= 0)
            {
                return null;
            }

            if(allstudents.ContainsKey(id))
            {
                return allstudents[id];
            }

            return null;
        }
    }
}
