using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollector {

    public static ObjectCollector getInstance()
    {
        return Singleton<ObjectCollector>.getInstance();
    }

    public Dictionary<int, NetObjectInterFace.IObjectSync> collector = new Dictionary<int, NetObjectInterFace.IObjectSync>();
    public Dictionary<int, NetObjectInterFace.IObjectOperate> objectoperate = new Dictionary<int, NetObjectInterFace.IObjectOperate>();

    private int _counter = 0;
    private int Counter()
    {
        _counter++;

        return _counter;
    }

    public int Add(GameObject o)
    {
        int id = Counter();

        NetObjectInterFace.IObjectSync ios = o.GetComponent<NetObjectInterFace.IObjectSync>();
        if(ios != null)
        {
            collector.Add(id, ios);
        }

        NetObjectInterFace.IObjectOperate io = o.gameObject.GetComponent<NetObjectInterFace.IObjectOperate>();
        if(io != null)
        {
            objectoperate.Add(id, io);
        }

        return id;
    }

    public void Add(int id, GameObject o)
    {
        NetObjectInterFace.IObjectSync ios = o.GetComponent<NetObjectInterFace.IObjectSync>();
        if (ios != null)
        {
            if(collector.ContainsKey(id))
            {
                collector[id] = ios;
            }
            else
            {
                collector.Add(id, ios);
            }
        }

        NetObjectInterFace.IObjectOperate io = o.gameObject.GetComponent<NetObjectInterFace.IObjectOperate>();
        if (io != null)
        {
            if (objectoperate.ContainsKey(id))
            {
                objectoperate[id] = io;
            }
            else
            {
                objectoperate.Add(id, io);
            }
        }
    }

    public void Remove(int id)
    {
        if(collector.ContainsKey(id))
        {
            collector.Remove(id);
        }

        if (objectoperate.ContainsKey(id))
        {
            objectoperate.Remove(id);
        }
    }

    public NetObjectInterFace.IObjectSync GetById(int id)
    {
        NetObjectInterFace.IObjectSync so = null;

        if (collector.ContainsKey(id))
        {
            so = collector[id];
        }

        return so;
    }

    public Hashtable GetHash()
    {
        Hashtable h = new Hashtable();
        foreach(KeyValuePair<int, NetObjectInterFace.IObjectSync> so in collector)
        {
            int id = (int)so.Key;
            NetObjectInterFace.IObjectSync s = (NetObjectInterFace.IObjectSync)so.Value;
            h.Add(id.ToString(), s.GetHash());
        }

        return h;
    }

    // 对于物体的操作指令解析
    private void RegListener()
    {
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.ObjectOperate, ObjectOperate);
        CommandReceive.getInstance().AddReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.ObjectOperate, ObjectOperate);
    }

    private void RemoveListener()
    {
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.Lobby, CommandDefine.SecondLayer.ObjectOperate, ObjectOperate);
        CommandReceive.getInstance().RemoveReceiver(CommandDefine.FirstLayer.CourseWave, CommandDefine.SecondLayer.ObjectOperate, ObjectOperate);
    }

    private void ObjectOperate(int userid, ArrayList msg)
    {
        if(msg == null || msg.Count <= 2)
        {
            return;
        }

        Int64 objectid = (Int64)msg[2];

        if(objectoperate.ContainsKey((int)objectid))
        {
            objectoperate[(int)objectid].ObjectOperate(userid, msg);
        }
    }
}
