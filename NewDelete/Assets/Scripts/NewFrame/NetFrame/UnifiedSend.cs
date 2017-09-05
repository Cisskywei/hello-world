using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnifiedSend {

    public static UnifiedSend getInstance()
    {
        return Singleton<UnifiedSend>.getInstance();
    }

    public UnifiedSend()
    {
        NetMainLoop.Instance().AddUpdate(OnUpdate);
    }

    public Hashtable senddata = new Hashtable();
    public Hashtable objectdata = new Hashtable();
    public Hashtable playerdata = new Hashtable();

    private int framespace = 5;  // 每5帧同步
    private int frame = 0;

    void OnUpdate()
    {
        if (frame++ > framespace)
        {
            frame = 0;
            Send();
        }
    }

    public void Send()
    {
        //if(senddata == null || senddata.Count <= 0)
        //{
        //    return;
        //}

        //TODO
        if(senddata == null)
        {
            senddata = new Hashtable();
        }

        if(playerdata != null && playerdata.Count > 0)
        {
            if (senddata.ContainsKey("player"))
            {
                senddata["player"] = playerdata;
            }
            else
            {
                senddata.Add("player", playerdata);
            }
        }

        if (objectdata != null && objectdata.Count > 0)
        {
            if (senddata.ContainsKey("objects"))
            {
                senddata["objects"] = objectdata;
            }
            else
            {
                senddata.Add("objects", objectdata);
            }
        }

        if(senddata.Count > 0)
        {
            NetworkCommunicate.getInstance().ReqChangeClientAllOnce(227, 67, senddata);

            foreach(DictionaryEntry v in objectdata)
            {
                ((Hashtable)v.Value).Clear();
            }
            objectdata.Clear();

            foreach (DictionaryEntry v in playerdata)
            {
                Hashtable h = (Hashtable)v.Value;

                Debug.Log(v.Key + " -- 外部的 ");

                foreach(DictionaryEntry l in h)
                {
                    Debug.Log(l.Key + " -- 内部的 ");

                    ((Hashtable)l.Value).Clear();
                }

                h.Clear();
            }
            playerdata.Clear();

            senddata.Clear();
        }
    }

    public void AddObject(int id, Hashtable data)
    {
        if(objectdata == null)
        {
            objectdata = new Hashtable();
        }

        string idkey = id.ToString();
        if(objectdata.ContainsKey(idkey))
        {
            objectdata[idkey] = data;
        }
        else
        {
            objectdata.Add(idkey, data);
        }
    }

    public void AddPlayer(int id, Hashtable data)
    {
        if (playerdata == null)
        {
            playerdata = new Hashtable();
        }

        string idkey = id.ToString();
        if (playerdata.ContainsKey(idkey))
        {
            playerdata[idkey] = data;
        }
        else
        {
            playerdata.Add(idkey, data);
        }
    }
}
