using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOperate : MonoBehaviour, NetObjectInterFace.IObjectOperate
{
    public SyncObject so;

    private void Start()
    {
        if(so == null)
        {
            so = gameObject.GetComponent<SyncObject>();
        }
    }

    void NetObjectInterFace.IObjectOperate.ObjectOperate(int userid, ArrayList msg)
    {
        Int64 typ = (Int64)msg[3];
        Enums.ObjectOperate operatetyp = (Enums.ObjectOperate)typ;
        Int64 ret = (Int64)msg[5];

        switch(operatetyp)
        {
            case Enums.ObjectOperate.Hold:
                Hold(userid, (int)ret);
                break;
            case Enums.ObjectOperate.Release:
                Release(userid, (int)ret);
                break;
            default:
                break;
        }
    }

    public void Hold(int userid, int ret)
    {
        if (userid != UserInfor.getInstance().UserId)
        {
            // 不是自己拿
            if (ret == 1)
            {
                so.locker = userid;
                so.ChangeObjectState(Enums.ObjectState.CanReceive);
            }
        }
        else
        {
            // 是自己拿
            if (ret == 1)
            {
                so.locker = userid;
                so.ChangeObjectState(Enums.ObjectState.CanSend);
            }
        }
    }

    public void Release(int userid, int ret)
    {
        if (userid != UserInfor.getInstance().UserId)
        {
            // 不是自己拿
            if (ret == 1)
            {
                so.locker = -1;
                so.ChangeObjectState(Enums.ObjectState.CanReceive);
            }
        }
        else
        {
            // 是自己拿
            if (ret == 1)
            {
                so.locker = -1;
            }
        }
    }
}
