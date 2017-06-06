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
        // 登陆返回数据
        public string id;
        public string name;
        public string access_token;

        public bool islogin = false;
        public bool isleader = false;

        // 所在教室的名字  如果是老师 则从服务器获取
        public string roomname = "llll";

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
