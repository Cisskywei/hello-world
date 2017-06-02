using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 负责处理与后台服务器的交互
    /// </summary>
    class BackDataService
    {
        public static BackDataService getInstance()
        {
            return Singleton<BackDataService>.getInstance();
        }

        public UserInfor CheckUser(string token, params Object[] p)
        {
            // 只为测试
            UserInfor ret = new UserInfor();
            return ret;
        }

        public Hashtable GetUserList(string token, params Object[] p)
        {
            return null;
        }

        public Hashtable GetUserTokenNameList(string token)
        {
            Hashtable h = new Hashtable();
            h.Add("student1", "若雨");
            h.Add("student2", "寒露");

            return h;
        }

        // 和后台服务器的接口

        // 获取学生课程列表
        private readonly string url_student_courses = "";
        public void GetStudentCourseList(string token)
        {
            HttpWebResponse hr = HttpHelper.getInstance().CreateGetHttpResponse(url_student_courses,100,null,null);
        }
        // 获取老师创建课程列表
        private readonly string url_teacher_courses = "";
        public void GetTeacherCourseList(string token)
        {
            HttpWebResponse hr = HttpHelper.getInstance().CreateGetHttpResponse(url_teacher_courses, 100, null, null);
        }
        //获取某个课程下有哪些学生
        private readonly string url_course_student = "";
        public void GetCourseStudentList(string token, int classid)
        {
            HttpWebResponse hr = HttpHelper.getInstance().CreateGetHttpResponse(url_course_student, 100, null, null);
        }
    }
}
