using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    /// <summary>
    /// 每个教室的消息管理器
    /// </summary>
    class CommandManager
    {
        public static CommandManager getInstance()
        {
            return Singleton<CommandManager>.getInstance();
        }

        private Dictionary<int, CommandReceive> reveiver = new Dictionary<int, CommandReceive>();

        private void AddReceiver(int roomid, CommandReceive r)
        {
            if(!reveiver.ContainsKey(roomid))
            {
                reveiver.Add(roomid, r);
            }
        }

        private void RemoveReceiver(int roomid)
        {
            if(reveiver == null || reveiver.Count <= 0)
            {
                return;
            }

            if (reveiver.ContainsKey(roomid))
            {
                reveiver.Remove(roomid);
            }
        }

        public CommandReceive ApplyReceiver(int roomid)
        {
            if(reveiver.ContainsKey(roomid))
            {
                return reveiver[roomid];
            }

            CommandReceive r = new CommandReceive();

            reveiver.Add(roomid, r);

            return r;
        }

        public CommandReceive FindReceiver(int roomid)
        {
            if (reveiver.ContainsKey(roomid))
            {
                return reveiver[roomid];
            }

            return null;
        }
    }
}
