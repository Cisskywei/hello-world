using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class BackDataType
    {
        // 网络返回基本数据
        public class MessageRetHead
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            //public Object data { get; set; }
        }

        // {"message":"登录成功","code":0,"type":"","data":{"id":"5","name":"lixin","access_token":"uWBpifKV2D9p6UlHpXhrkf3zP_1X1MPc"}}
        public class PlayerLoginRetData
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public Data data { get; set; }
            public class Data
            {
                public string id { get; set; }
                public string name { get; set; }
                public string access_token { get; set; }
            }
        }

        public class PlayerBaseInforRetData
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public Data data { get; set; }
            public class Data
            {
                public string user_id { get; set; }
                public string user_name { get; set; }
                public string telephone { get; set; }
                public string email { get; set; }
                public string sex { get; set; } // male 男 female 女
                public string avatar { get; set; } // 头像
                public string reg_time { get; set; } // 注册时间
                public string last_login_time { get; set; } // 最后登录时间
                public TeacherInfor teacher { get; set; }
                public StudentInfor student { get; set; }
            }

            public class TeacherInfor
            {
                public string teacher_id { get; set; }
                public string school_id { get; set; }
                public string college_id { get; set; }
                public string name { get; set; }
                public string teacher_number { get; set; }
                public string rank { get; set; }
            }

            public class StudentInfor
            {
                public string student_id { get; set; }
                public string school_id { get; set; }
                public string college_id { get; set; }
                public string class_id { get; set; }
                public string name { get; set; }
                public string student_number { get; set; }
            }
        }

        public class CourseListRetData
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public Data[] data { get; set; }
            public class Data
            {
                public string course_id { get; set; }
                public string mode { get; set; }
                public string course_name { get; set; }
                public string course_teacher { get; set; }
                public string cover { get; set; } // 封面图
            }
        }

        public class StudentInfor
        {
            //学生的用户ID、头像、用户名、学生姓名
            public string user_id { get; set; }
            public string avatar { get; set; }
            public string user_name { get; set; }
            public string student_name { get; set; }
            public string sex { get; set; }
            public Classes classes { get; set; }
            public College college { get; set; }
            public School school { get; set; }

            //班级资料，字段分别是ID、名称、代码、入学年月（格式是：yyyy-mm）
            public class Classes
            {
                public string class_id { get; set; }
                public string name { get; set; }
                public string symbol { get; set; }
                public string level { get; set; }
            }
            //学生所属院（系），字段是ID和名称
            public class College
            {
                public string college_id { get; set; }
                public string name { get; set; }
            }
            //学生所属学校
            public class School
            {
                public string school_id { get; set; }
                public string name { get; set; }
            }
        }

        public class CourseInforRetData
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public CourseInfor data { get; set; }

            public class CourseInfor
            {
                public string course_id { get; set; }
                public string mode { get; set; }
                public string course_name { get; set; }
                public string course_teacher { get; set; }
                public string school { get; set; }
                public string background { get; set; }
                public string introduce { get; set; }
                public string target { get; set; }
                public string cover { get; set; }
                public string video { get; set; }

                public StudentInfor[] students { get; set; }
            }
        }

        // 题目数据
        public class QuestionInforRetData
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public QuestionInfor[] data { get; set; }

            public class QuestionInfor
            {
                public string question_id { get; set; }
                public string type { get; set; }
                public string stem { get; set; }
                public string options { get; set; }
                public string answer { get; set; }
                public string explanation { get; set; }
            }
        }

        // 课程资料信息
        public class MaterialItemInfor
        {
            public string file_id { get; set; }
            public string user_id { get; set; }
            public string folder_id { get; set; }
            public string type { get; set; }
            public string file_name { get; set; }
            public string file_path { get; set; }
            public string size { get; set; }
            public string extend { get; set; }
            public string package { get; set; }
            public string create_time { get; set; }
            public string update_time { get; set; }
            public string converted { get; set; }
        }

        public class MaterialItemInforRetData
        {
            public string message { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public MaterialItemInfor[] data { get; set; }
        }

        public static Hashtable StudentInfor_Serialize(StudentInfor data)
        {
            Hashtable h = new Hashtable();

            if (data.user_id != null)
            {
                h.Add("user_id", data.user_id);
            }

            if (data.avatar != null)
            {
                h.Add("avatar", data.avatar);
            }

            if (data.user_name != null)
            {
                h.Add("user_name", data.user_name);
            }

            if (data.student_name != null)
            {
                h.Add("student_name", data.student_name);
            }

            if (data.sex != null)
            {
                h.Add("sex", data.sex);
            }

            if (data.college != null)
            {
                Hashtable co = new Hashtable();

                if (data.college.college_id != null)
                {
                    co.Add("college_id", data.college.college_id);
                }

                if (data.college.name != null)
                {
                    co.Add("name", data.college.name);
                }

                if (co.Count > 0)
                {
                    h.Add("college", co);
                }
            }

            if (data.school != null)
            {
                Hashtable sc = new Hashtable();

                if (data.school.school_id != null)
                {
                    sc.Add("school_id", data.school.school_id);
                }

                if (data.school.name != null)
                {
                    sc.Add("name", data.school.name);
                }

                if (sc.Count > 0)
                {
                    h.Add("school", sc);
                }
            }

            if (data.classes != null)
            {
                Hashtable cl = new Hashtable();

                if (data.classes.class_id != null)
                {
                    cl.Add("class_id", data.classes.class_id);
                }

                if (data.classes.name != null)
                {
                    cl.Add("name", data.classes.name);
                }

                if (data.classes.symbol != null)
                {
                    cl.Add("symbol", data.classes.symbol);
                }

                if (data.classes.level != null)
                {
                    cl.Add("level", data.classes.level);
                }

                if (cl.Count > 0)
                {
                    h.Add("classes", cl);
                }
            }

            return h;
        }

        public static Hashtable CourseListRetData_Serialize(CourseListRetData data)
        {
            Hashtable h = new Hashtable();

            for (int i = 0; i < data.data.Length; i++)
            {
                Hashtable hh = new Hashtable();
                hh.Add("course_id", data.data[i].course_id);
                hh.Add("mode", data.data[i].mode);
                hh.Add("course_name", data.data[i].course_name);
                hh.Add("course_teacher", data.data[i].course_teacher);
                hh.Add("cover", data.data[i].cover);

                h.Add(i.ToString(), hh);
            }

            return h;
        }

        public static Hashtable CourseInforRetData_Serialize(CourseInforRetData data)
        {
            Hashtable hh = new Hashtable();
            hh.Add("course_id", data.data.course_id);
            hh.Add("mode", data.data.mode);
            hh.Add("course_name", data.data.course_name);
            hh.Add("course_teacher", data.data.course_teacher);
            hh.Add("school", data.data.school);

            StudentInfor[] s = data.data.students;
            Hashtable hhh = new Hashtable();
            for (int i = 0; i < s.Length; i++)
            {
                Hashtable h = StudentInfor_Serialize((StudentInfor)s[i]);
                if (h != null)
                {
                    hhh.Add(s[i].user_id, h);
                }
            }

            hh.Add("students", hhh);

            return hh;
        }

        // 对象拷贝
        public static void CopyValue(object origin, object target)
        {
            System.Reflection.PropertyInfo[] properties = (target.GetType()).GetProperties();
            System.Reflection.FieldInfo[] fields = (origin.GetType()).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    if (fields[i].Name == properties[j].Name && properties[j].CanWrite)
                    {
                        properties[j].SetValue(target, fields[i].GetValue(origin), null);
                    }
                }
            }
        }
    }
}
