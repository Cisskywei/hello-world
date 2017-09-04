using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ko.NetFram
{
    public class DataType
    {
        [Serializable]
        public class LoginData
        {
            public string id;
            public string name;
            public string access_token;
        }

        [Serializable]
        public class PlayerLoginRetData
        {
            public string message;
            public string code;
            public string type;
            public LoginData data;
        }

        [Serializable]
        public class TeacherInforBase
        {
            public string teacher_id;
            public string school_id;
            public string college_id;
            public string name;
            public string teacher_number;
            public string rank;
        }

        [Serializable]
        public class StudentInforBase
        {
            public string student_id;
            public string school_id;
            public string college_id;
            public string class_id;
            public string name;
            public string student_number;
        }

        [Serializable]
        public class BaseInforData
        {
            public string user_id;
            public string user_name;
            public string telephone;
            public string email;
            public string sex; // male 男 female 女
            public string avatar; // 头像
            public string reg_time; // 注册时间
            public string last_login_time; // 最后登录时间
            public TeacherInforBase teacher;
            public StudentInforBase student;
        }

        [Serializable]
        public class PlayerBaseInforRetData
        {
            public string message;
            public string code;
            public string type;
            public BaseInforData data;
        }

        //课程列表信息
        [Serializable]
        public class CourseListData
        {
            public string course_id;
            public string mode;
            public string course_name;
            public string course_teacher;
            public string cover; // 封面图
        }

        [Serializable]
        public class CourseListRetData
        {
            public string message;
            public string code;
            public string type;
            public CourseListData[] data;
        }

        //课程详细信息

        //班级资料，字段分别是ID、名称、代码、入学年月（格式是：yyyy-mm）
        [Serializable]
        public class Classes
        {
            public string class_id;
            public string name;
            public string symbol;
            public string level;
        }

        //学生所属院（系），字段是ID和名称
        [Serializable]
        public class College
        {
            public string college_id;
            public string name;
        }

        //学生所属学校
        [Serializable]
        public class School
        {
            public string school_id;
            public string name;
        }

        [Serializable]
        public class StudentInfor
        {
            //学生的用户ID、头像、用户名、学生姓名
            public string user_id;
            public string avatar;
            public string user_name;
            public string student_name;
            public string sex;
            public Classes classes;
            public College college;
            public School school;
        }

        [Serializable]
        public class CourseInfor
        {
            public string course_id;
            public string mode;
            public string course_name;
            public string course_teacher;
            public string school;
            public string background;
            public string introduce;
            public string target;
            public string cover;
            public string video;

            public StudentInfor[] students;
        }

        [Serializable]
        public class CourseInforRetData
        {
            public string message;
            public string code;
            public string type;
            public CourseInfor data;
        }
        //老师信息

        //学生信息

        // 题目信息
        [Serializable]
        public class QuestionInfor
        {
            public string question_id;
            public string type;
            public string stem;
            public string options;
            public string answer;
            public string explanation;
            public string difficulty;
        }

        [Serializable]
        public class QuestionInforRetData
        {
            public string message;
            public string code;
            public string type;
            public QuestionInfor[] data;
        }

        // 课程资料信息
        [Serializable]
        public class MaterialItemInfor
        {
            public string file_id;
            public string user_id;
            public string folder_id;
            public string type;
            public string file_name;
            public string file_path;
            public string size;
            public string extend;
            public string package;
            public string create_time;
            public string update_time;
            public string converted;
        }

        [Serializable]
        public class MaterialItemInforRetData
        {
            public string message;
            public string code;
            public string type;
            public MaterialItemInfor[] data;
        }
    }
}

