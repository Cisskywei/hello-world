using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomClassroom.ClassRoom
{
    class CommandReceive
    {
        public delegate void MsgReceive(int userid, ArrayList msg);

        private Dictionary<CommandDefine.FirstLayer, Dictionary<CommandDefine.SecondLayer, MsgReceive>> receiver
            = new Dictionary<CommandDefine.FirstLayer, Dictionary<CommandDefine.SecondLayer, MsgReceive>>();

        public bool Receive(int userid, ArrayList msg)
        {
            bool ret = false;

            if (msg == null || msg.Count <= 0)
            {
                return ret;
            }

            Int64 fi = (Int64)msg[0];
            Int64 si = (Int64)msg[1];
            CommandDefine.FirstLayer f = (CommandDefine.FirstLayer)fi;
            CommandDefine.SecondLayer s = (CommandDefine.SecondLayer)si;

            do
            {
                if (!receiver.ContainsKey(f))
                {
                    break;
                }

                Dictionary<CommandDefine.SecondLayer, MsgReceive> second = receiver[f];

                if (second == null || !second.ContainsKey(s) || second[s] == null)
                {
                    break;
                }

                second[s].Invoke(userid, msg);

                ret = true;

            } while (false);

            return ret;
        }

        public void AddReceiver(CommandDefine.FirstLayer first, CommandDefine.SecondLayer second, MsgReceive func)
        {
            if (receiver.ContainsKey(first))
            {
                Dictionary<CommandDefine.SecondLayer, MsgReceive> s = receiver[first];

                if (s == null)
                {
                    s = new Dictionary<CommandDefine.SecondLayer, MsgReceive>();
                    s.Add(second, func);
                    receiver.Add(first, s);
                }
                else if (s.ContainsKey(second))
                {
                    if (s[second] != null)
                    {
                        s[second] += func;
                    }
                    else
                    {
                        s[second] = func;
                    }
                }
                else
                {
                    s.Add(second, func);
                }
            }
            else
            {
                Dictionary<CommandDefine.SecondLayer, MsgReceive> s = new Dictionary<CommandDefine.SecondLayer, MsgReceive>();
                s.Add(second, func);
                receiver.Add(first, s);
            }
        }

        public void RemoveReceiver(CommandDefine.FirstLayer first, CommandDefine.SecondLayer second, MsgReceive func)
        {
            do
            {
                if (!receiver.ContainsKey(first))
                {
                    break;
                }

                Dictionary<CommandDefine.SecondLayer, MsgReceive> s = receiver[first];

                if (s == null || !s.ContainsKey(second) || s[second] == null)
                {
                    break;
                }

                s[second] -= func;

            } while (false);
        }
    }
}
