using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    /// <summary>
    /// 负责平台数据的转换
    /// </summary>
    class UserInfor
    {
        // 网络连接相关
        public string uuid;
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
        public string identity = string.Empty;

        public string studentname = string.Empty;

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

            if(jsondata != null)
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

            if(this.teacher != null)
            {
                identity = "teacher";
            }

            if(this.student != null)
            {
                identity = "student";
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

            identity = "student";

            if (jsondata != null)
            {
                _studentinfor_json = jsondata;
            }
        }

        public void InitCourseListRetData(BackDataType.CourseListRetData studentinfor, string jsondata = null)
        {

        }

        public bool islogin = false;
        public bool isentercourse = false;
        public bool isleader = false;

        // 所在教室的名字  如果是老师 则从服务器获取
        public string roomname = "llll";
        public int roomid = -1;
        public string groupname;
        public int groupid;

        // 课程列表
        public ArrayList GetCourseList()
        {
            // 只为测试
            ArrayList ret = new ArrayList();
            ret.Add("TestCourse");

            return ret;
        }
    }
}
