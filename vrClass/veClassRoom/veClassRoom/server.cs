using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using veClassRoom.Market;
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

            MarketMsg market = new MarketMsg();
            hub.hub.modules.add_module(MarketMsg.selfmodelname, market);
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
    }
}
