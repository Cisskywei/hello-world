using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class UserInfor
    {
        // 登陆返回数据
        public string id;
        public string name;
        public string access_token;
        // 延伸字段
        public int selfid;
        // 获取基本信息返回数据
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string sex { get; set; } // male 男 female 女
        public string avatar { get; set; } // 头像
        public string reg_time { get; set; } // 注册时间
        public string last_login_time { get; set; } // 最后登录时间
        public BackDataType.PlayerBaseInforRetData.TeacherInfor teacher = null;
        public BackDataType.PlayerBaseInforRetData.StudentInfor student = null;
        // 延伸字段
        public Enums.DutyEnum identity = Enums.DutyEnum.Student;

        public string studentname = string.Empty;
        public BackDataType.StudentInfor.Classes user_class;
        public BackDataType.StudentInfor.College user_college;
        public BackDataType.StudentInfor.School user_school;

        // 后台服务器返回数据jsondata 保存
        public string _login_json = string.Empty;
        public string _baseinfor_json = string.Empty;
        public string _studentinfor_json = string.Empty;

        public void InitLoginRetData(BackDataType.PlayerLoginRetData login, string jsondata = null)
        {
            this.id = login.data.id;
            this.name = login.data.name;
            this.access_token = login.data.access_token;

            selfid = Convert.ToInt32(this.id);

            if (jsondata != null)
            {
                _login_json = jsondata;
            }
        }

        public void InitBaseInforRetData(BackDataType.PlayerBaseInforRetData baseinfor, string jsondata = null)
        {
            this.user_id = baseinfor.data.user_id;
            this.user_name = baseinfor.data.user_name;
            this.telephone = baseinfor.data.telephone;
            this.email = baseinfor.data.email;
            this.sex = baseinfor.data.sex;
            this.avatar = baseinfor.data.avatar;
            this.reg_time = baseinfor.data.reg_time;
            this.last_login_time = baseinfor.data.last_login_time;
            this.teacher = baseinfor.data.teacher;
            this.student = baseinfor.data.student;

            if (this.teacher != null)
            {
                identity = Enums.DutyEnum.Teacher;
            }

            if (this.student != null)
            {
                identity = Enums.DutyEnum.Student;
            }

            if (jsondata != null)
            {
                _baseinfor_json = jsondata;
            }
        }

        public void InitByStudentInfor(BackDataType.StudentInfor studentinfor, string jsondata = null)
        {
            this.user_id = studentinfor.user_id;
            this.user_name = studentinfor.user_name;
            this.sex = studentinfor.sex;
            this.studentname = studentinfor.student_name;

            this.user_class = studentinfor.classes;
            this.user_college = studentinfor.college;
            this.user_school = studentinfor.school;

            identity = Enums.DutyEnum.Student;

            if (jsondata != null)
            {
                _studentinfor_json = jsondata;
            }
        }

        public void InitCourseListRetData(BackDataType.CourseListRetData studentinfor, string jsondata = null)
        {

        }

        // 网络连接相关
        public string uuid;

        public int roomid = -1;
        public int groupid = -1;
        public bool islogin = false;
        public bool isenterlobby = false;
    }
}
