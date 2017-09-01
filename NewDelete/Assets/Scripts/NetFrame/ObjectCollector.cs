using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollector {

    public static ObjectCollector getInstance()
    {
        return Singleton<ObjectCollector>.getInstance();
    }

    public Dictionary<int, SyncObject> collector = new Dictionary<int, SyncObject>();

    private int _counter = 0;
    private int Counter()
    {
        _counter++;

        return _counter;
    }

    public int Add(SyncObject o)
    {
        int id = Counter();

        collector.Add(id, o);

        return id;
    }

    public void Remove(int id)
    {
        if(collector.ContainsKey(id))
        {
            collector.Remove(id);
        }
    }

    public SyncObject GetById(int id)
    {
        SyncObject so = null;

        if (collector.ContainsKey(id))
        {
            so = collector[id];
        }

        return so;
    }

    public Hashtable GetHash()
    {
        Hashtable h = new Hashtable();
        foreach(KeyValuePair<int,SyncObject> so in collector)
        {
            int id = (int)so.Key;
            SyncObject s = (SyncObject)so.Value;
            h.Add(id.ToString(), s.GetHash());
        }

        return h;
    }
}
