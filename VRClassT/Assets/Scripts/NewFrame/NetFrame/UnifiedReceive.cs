using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnifiedReceive {

    public static UnifiedReceive getInstance()
    {
        return Singleton<UnifiedReceive>.getInstance();
    }

    public void Receive(Hashtable data)
    {
        if(data == null || data.Count <= 0)
        {
            return;
        }

        Hashtable o = (Hashtable)data["objects"];

        if(o != null && o.Count > 0)
        {
            foreach(DictionaryEntry v in o)
            {
                int id = Convert.ToInt32(v.Key);
                NetObjectInterFace.IObjectSync so = ObjectCollector.getInstance().GetById(id);

                Debug.Log("data o " + v.Key);

                if (so != null)
                {
                    so.ReceiveSync((Hashtable)v.Value);
                }
                else
                {
                    Debug.Log("so == null" + v.Key);
                }
            }
        }

        Hashtable p = (Hashtable)data["player"];
        RoleManager.getInstance().RecivePlayerInfor(p);
    }

    public void Receive(Hashtable objectdata, Hashtable playerdata)
    {
        if (objectdata != null && objectdata.Count > 0)
        {
            foreach (DictionaryEntry v in objectdata)
            {
                int id = Convert.ToInt32(v.Key);
                NetObjectInterFace.IObjectSync so = ObjectCollector.getInstance().GetById(id);

                Debug.Log("data o " + v.Key);

                if (so != null)
                {
                    so.ReceiveSync((Hashtable)v.Value);
                }
                else
                {
                    Debug.Log("so == null" + v.Key);
                }
            }
        }

        RoleManager.getInstance().RecivePlayerInfor(playerdata);
    }
}
