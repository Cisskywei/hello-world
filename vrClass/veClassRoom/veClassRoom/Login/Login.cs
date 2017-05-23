using common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom
{
    class Login : imodule
    {
        public static string _selfmodelname = "Login";

        public void player_login(string token, string name, Int64 identity, string modelname, string callbackname)
        {
            var client_uuid = hub.hub.gates.current_client_uuid;

            if (modelname == null)
            {
                modelname = "cMsgConnect";
            }

            if (callbackname == null)
            {
                callbackname = "ret_msg";
            }

            Console.WriteLine("用户登陆 : " + client_uuid);

            Config.PlayerIdentity id = (Config.PlayerIdentity)identity;

            switch(id)
            {
                case Config.PlayerIdentity.STUDENT:
                    // 学生登陆
                    student_login(token, name, client_uuid, modelname, callbackname);
                    break;
                case Config.PlayerIdentity.TEACHER:
                    // 教师登陆
                    tacher_login(token, name, client_uuid, modelname, callbackname);
                    break;
                case Config.PlayerIdentity.ASSISTANT:
                    // 助教登陆
                    break;
                default:
                    // 其他
                    break;
            }
        }

   //     public void player_login(Hashtable msg)
   //     {

   //         string token = (string)msg["token"];
   //         string name = (string)msg["name"];
   ////         string identity = (int)msg["identity"];
   //         string modelname = (string)msg["modelname"];
   //         string callbackname = (string)msg["callbackname"];

   //         player_login(token, name, (int)Config.PlayerIdentity.NONE, modelname, callbackname);

   //     }

        public void tacher_login(string token, string name, string uuid, string modelname, string callbackname)
        {
            //只为测试 固定老师名字
            token = "onlyyou";
            name = "fine";
            Console.WriteLine("教师登陆 ： " + name + " token is : " + token);
            server.teachers.teacher_login(token, name, uuid, modelname, callbackname);
        }

        public void student_login(string token, string name, string uuid, string modelname, string callbackname)
        {
            server.students.student_login(token, name, uuid, modelname, callbackname);
        }


        // 进入房间
        public void student_enter_class(string token, string name, string uuid, string teachername)
        {
            //
            Console.WriteLine("学生进入教室 老师的名字是 ： " + teachername);

            server.students.student_enter_room(token, name, uuid, teachername);
        }

        // 教师退出程序
        public void teacher_leave_class(string token, string name, string uuid)
        {
            //只为测试 固定老师名字
            token = "onlyyou";
            name = "fine";
            Console.WriteLine("教师退出 ： " + name + " token is : " + token);

            Teacher t = server.teachers.get_teacher_token(token);
            if(t != null)
            {
                t.teacher_leave_class(token, name, uuid, null);

 //               server.teachers.clearAllTeacher();
                server.students.clearAllStudent();
            }
        }

        // 教师退出程序
        public void student_leave_class(string token, string name, string uuid, string teachername = null)
        {
            Console.WriteLine("学生退出 ： " + name + " token is : " + token);

            if(teachername == null)
            {
                teachername = "onlyyou";
            }

            Teacher t = server.teachers.get_teacher_token(teachername);
            if (t != null)
            {
                t.student_leave_class(token, name, uuid);
            }

            server.students.student_leave_class(token, name, uuid);
        }


        // 场景管理相关
        public void player_enter_scene(string token, string name, string uuid, string scenename)
        {
            Console.WriteLine("进来场景 token : " + token);

            Scene s = server.scenes.FindSceneByName(scenename);

            string retmodule = null;

            if(s == null)
            {
                // 场景不存在
                Console.WriteLine(name + "进入场景  但是场景" + scenename + "不存在");

                // 创建新的场景
                if(server.scenes.CreateSceneByName(scenename))
                {
                    Console.WriteLine("创建场景" + scenename + "成功");

                    s = server.scenes.FindSceneByName(scenename);
                }
            }

            if(s != null)
            {
                retmodule = s.PlayerEnterScene(token, name, uuid);
            }

            //else
            //{
            //    retmodule = s.PlayerEnterScene(token, name, uuid);
            //}

            Hashtable msg = new Hashtable();
            if (retmodule != null)
            {
                msg.Add("result", "succeed");
                msg.Add("connector", retmodule);
            }else
            {
                msg.Add("result", "failed");
            }

            hub.hub.gates.call_client(uuid, "cMsgConnect", "ret_msg", msg);

        }

    }
}
