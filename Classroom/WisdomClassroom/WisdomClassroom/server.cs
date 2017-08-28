using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WisdomClassroom.ClassRoom;

namespace WisdomClassroom
{
    class server
    {
        static void Main(string[] args)
        {
            //测试
            args = new string[] { "D:\\programs\\baiduyun\\download\\myuse\\myuse\\abelkhan\\bin\\hello_world_config.txt", "lobby" };

            Console.WriteLine("args ：" + args.Length);

            if (args.Length <= 0)
            {
                return;
            }

            hub.hub _hub = new hub.hub(args);

            add_necessary_model();

            init_manage();

            Int64 oldtick = 0;
            Int64 tick = 0;
            while (true)
            {
                oldtick = tick;
                tick = _hub.poll();

                if (hub.hub.closeHandle.is_close)
                {
                    log.log.trace(new System.Diagnostics.StackFrame(true), tick, "server closed, hub server:{0}", hub.hub.uuid);
                    break;
                }

                Int64 ticktime = (tick - oldtick);
                if (ticktime < 50)
                {
                    Thread.Sleep(15);
                }
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
            NetworkMessage net = new NetworkMessage();
            add_Hub_Model(NetworkMessage.selfmodelname, net);

            Console.WriteLine("添加基础且必须的model至Hub");
        }

        public static void init_manage()
        {
        }
    }
}
