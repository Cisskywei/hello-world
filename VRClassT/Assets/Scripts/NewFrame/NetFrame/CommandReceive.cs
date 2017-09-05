using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理一级指令的分发
/// </summary>
public class CommandReceive {

    public static CommandReceive getInstance()
    {
        return Singleton<CommandReceive>.getInstance();
    }

    public delegate void MsgReceive(int userid, ArrayList msg);

    private Dictionary<CommandDefine.FirstLayer, Dictionary<CommandDefine.SecondLayer, MsgReceive>> receiver
        = new Dictionary<CommandDefine.FirstLayer, Dictionary<CommandDefine.SecondLayer, MsgReceive>>();

    public void Receive(int userid, ArrayList msg)
    {
        if(msg == null || msg.Count <= 0)
        {
            return;
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

        } while (false);
    }

    public void AddReceiver(CommandDefine.FirstLayer first, CommandDefine.SecondLayer second, MsgReceive func)
    {
        if(receiver.ContainsKey(first))
        {
            Dictionary<CommandDefine.SecondLayer, MsgReceive> s = receiver[first];

            if(s == null)
            {
                s = new Dictionary<CommandDefine.SecondLayer, MsgReceive>();
                s.Add(second, func);
                receiver.Add(first, s);
            }
            else if(s.ContainsKey(second))
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


    // hashtable msg 接受函数
    public delegate void VoidHashTable(Hashtable msg);
    private Dictionary<CommandDefine.HashTableMsgType, VoidHashTable> _hashmsg = new Dictionary<CommandDefine.HashTableMsgType, VoidHashTable>();

    public void ReceiveHashMsg(CommandDefine.HashTableMsgType typ, Hashtable msg)
    {
        if(_hashmsg == null || _hashmsg.Count <= 0)
        {
            return;
        }

        if(!_hashmsg.ContainsKey(typ) || _hashmsg[typ] == null)
        {
            return;
        }

        _hashmsg[typ](msg);
    }

    public void AddHashMsgListener(CommandDefine.HashTableMsgType typ, VoidHashTable listener)
    {
        if(_hashmsg.ContainsKey(typ))
        {
            if(_hashmsg[typ] == null)
            {
                _hashmsg[typ] = listener;
            }
            else
            {
                _hashmsg[typ] += listener;
            }
        }
        else
        {
            _hashmsg.Add(typ, listener);
        }
    }

    public void RemoveHashMsgListener(CommandDefine.HashTableMsgType typ, VoidHashTable listener)
    {
        if (!_hashmsg.ContainsKey(typ))
        {
            return;
        }

        if (_hashmsg[typ] == null)
        {
            return;
        }

        _hashmsg[typ] -= listener;
    }

}
