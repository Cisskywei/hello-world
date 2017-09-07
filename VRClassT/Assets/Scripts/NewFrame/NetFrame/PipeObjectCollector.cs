using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeObjectCollector {

    public static PipeObjectCollector getInstance()
    {
        return Singleton<PipeObjectCollector>.getInstance();
    }

    public Dictionary<int, NetObjectInterFace.IObjectPipeSync> collector = new Dictionary<int, NetObjectInterFace.IObjectPipeSync>();

    //private int _counter = 0;
    //private int Counter()
    //{
    //    _counter++;

    //    return _counter;
    //}

    //public int Add(GameObject o)
    //{
    //    int id = Counter();

    //    NetObjectInterFace.IObjectSync ios = o.GetComponent<NetObjectInterFace.IObjectSync>();
    //    if (ios != null)
    //    {
    //        collector.Add(id, ios);
    //    }

    //    NetObjectInterFace.IObjectOperate io = o.gameObject.GetComponent<NetObjectInterFace.IObjectOperate>();
    //    if (io != null)
    //    {
    //        objectoperate.Add(id, io);
    //    }

    //    return id;
    //}

    public void Add(int id, GameObject o)
    {
        NetObjectInterFace.IObjectPipeSync ios = o.GetComponent<NetObjectInterFace.IObjectPipeSync>();
        if (ios != null)
        {
            if (collector.ContainsKey(id))
            {
                collector[id] = ios;
            }
            else
            {
                collector.Add(id, ios);
            }
        }
    }

    public void Remove(int id)
    {
        if (collector.ContainsKey(id))
        {
            collector.Remove(id);
        }
    }

    public NetObjectInterFace.IObjectPipeSync GetById(int id)
    {
        NetObjectInterFace.IObjectPipeSync so = null;

        if (collector.ContainsKey(id))
        {
            so = collector[id];
        }

        return so;
    }
}
