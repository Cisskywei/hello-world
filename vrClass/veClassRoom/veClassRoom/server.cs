using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using veClassRoom.Room;

namespace veClassRoom
{
    class server
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            //// 测试
            //BackDataService.getInstance().CheckPlayerLogin("lixin", "1", server.loginback);
    //        BackDataService.getInstance().CheckPlayerLogin("红色诺言", "123456", server.loginback, server.loginbackf);

            hub.hub _hub = new hub.hub(args);

            add_necessary_model();

            init_manage();

            Int64 tick = Environment.TickCount;
            Int64 tickcount = 0;
            while (true)
            {
                Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
                if (tmptick < tick)
                {
                    tickcount += 1;
                    tmptick = tmptick + tickcount * UInt32.MaxValue;
                }
                tick = tmptick;

                try
                {
                    _hub.poll();
                }catch (Exception e)
                {
                    Console.WriteLine("网络异常 " + e);
                }

                if (hub.hub.closeHandle.is_close)
                {
                    Console.WriteLine("server closed, hub server " + hub.hub.uuid);
                    break;
                }

                tmptick = (Environment.TickCount & UInt32.MaxValue);
                if (tmptick < tick)
                {
                    tickcount += 1;
                    tmptick = tmptick + tickcount * UInt32.MaxValue;
                }
                Int64 ticktime = (tmptick - tick);
                tick = tmptick;

                //if (ticktime < 10)
                //{
                //    Thread.Sleep(3);
                //}
            }
        }

        public static void add_Hub_Model(string modelname, imodule model)
        {
            Console.WriteLine("添加model至Hub ：" + modelname);
            hub.hub.modules.add_module(modelname, model);
        }

        public static void clear_Hub_Model(string modelname, imodule model)
        {
            Console.WriteLine("从Hub移除model ：" + modelname);
            hub.hub.modules.remove_module(modelname);
        }

        /// <summary>
        /// 添加必要的model
        /// </summary>
        private static void add_necessary_model()
        {
            Console.WriteLine("添加基础且必须的model至Hub");

            WisdomLogin _login = new WisdomLogin();
            //        Login _login = new Login();
            hub.hub.modules.add_module(WisdomLogin.selfmodelname, _login);

            Login _login2 = new Login();
            hub.hub.modules.add_module(Login._selfmodelname, _login2);

        }

        public static void init_manage()
        {
            teachers = new TeacherManager();

            students = new StudentManage();

            scenes = new ScenesManager();

            rooms = new RoomManager();

            // 初始化房间消息分发器
            NetMessage.getInstance();
        }


        public static TeacherManager teachers;
        public static StudentManage students;

        public static ScenesManager scenes;
        public static RoomManager rooms;

        //        public static playermng players;

        // 测试
        public static void ons(BackDataType.QuestionInforRetData v, string tag = "xx")
        {
            Console.WriteLine(" Thread ons 获取课程题目数据 : " + "tag : " + tag + " -- " + v.data[0].options);
        }

        public static void onsTeacherCour(BackDataType.CourseListRetData v, string tag = "xx")
        {
            Console.WriteLine(" Thread ons 老师获取课程数据 : " + "tag : " + tag + " -- " + v.data[0].cover);

            BackDataService.getInstance().GetCourseStudentList(token, v.data[0].course_id, server.onsStudentList, server.loginbackf);
        }

        public static void onsStudentList(BackDataType.CourseInforRetData v, string tag = "xx")
        {
            Console.WriteLine(" Thread ons 老师获取课程学生列表数据 : " + "tag : " + tag + " -- " + v.data.cover);
        }

        public static string token;
        public static void loginback(BackDataType.PlayerLoginRetData v, string tag = "xx")
        {
            Console.WriteLine(" Thread ons 登陆 token : " + "tag : " + tag + " -- " + v.data.access_token);

            token = v.data.access_token;

            BackDataService.getInstance().GetPlayerBaseInfor(v.data.access_token, server.onsinfor);

            BackDataService.getInstance().GetTeacherCourseList(v.data.access_token, "expe", server.onsTeacherCour, server.loginbackf);

            BackDataService.getInstance().GetCourseQuestionList(v.data.access_token, server.ons, server.loginbackf);
        }

        public static void loginbackf(BackDataType.MessageRetHead msg, string tag = "xx")
        {
            Console.WriteLine(" Thread ons loginbackf : " + "tag : " + tag + " -- " + msg.message);
        }

        public static void onsinfor(BackDataType.PlayerBaseInforRetData v, string tag = "xx")
        {
            Console.WriteLine(" Thread ons 获取用户基本信息数据 : " + "tag : " + tag + " -- " + v.data.user_name);

            if(v.data.teacher != null)
            {
                Console.WriteLine(" Thread ons 老师 : " + v.data.teacher.teacher_number);
            }

            if (v.data.student != null)
            {
                Console.WriteLine(" Thread ons 学生 : " + v.data.student.student_number);
            }
        }

    }
}
