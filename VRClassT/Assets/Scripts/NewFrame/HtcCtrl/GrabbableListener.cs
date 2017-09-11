using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableListener : MonoBehaviour {

    public int selfid = -1;

    private HashSet<GameObject> rightGrabbingSet = new HashSet<GameObject>();
    private HashSet<GameObject> leftGrabbingSet = new HashSet<GameObject>();

    private void Start()
    {
        SyncObject so = gameObject.GetComponent<SyncObject>();
        if(so != null)
        {
            this.selfid = so.selfid;
        }
    }

    public void OnGrabbed(BasicGrabbable grabbedObj)
    {
        ViveColliderButtonEventData viveEventData;
        if (!grabbedObj.grabbedEvent.TryGetViveButtonEventData(out viveEventData)) { return; }

        switch (viveEventData.viveRole.ToRole<HandRole>())
        {
            case HandRole.RightHand:
                if (rightGrabbingSet.Add(grabbedObj.gameObject) && rightGrabbingSet.Count == 1)
                {
                }
                break;

            case HandRole.LeftHand:
                if (leftGrabbingSet.Add(grabbedObj.gameObject) && leftGrabbingSet.Count == 1)
                {
                }
                break;
        }

        // 网络消息相关
        if(selfid>=0)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Course);
            msg.Add((Int64)CommandDefine.SecondLayer.Hold);
            msg.Add((Int64)selfid);
            NetworkCommunicate.getInstance().ReqCommand(UserInfor.getInstance().RoomId,UserInfor.getInstance().UserId,msg);
        }
    }

    public void OnRelease(BasicGrabbable releasedObj)
    {
        ViveColliderButtonEventData viveEventData;
        if (!releasedObj.grabbedEvent.TryGetViveButtonEventData(out viveEventData)) { return; }

        switch (viveEventData.viveRole.ToRole<HandRole>())
        {
            case HandRole.RightHand:
                if (rightGrabbingSet.Remove(releasedObj.gameObject) && rightGrabbingSet.Count == 0)
                {
                }
                break;

            case HandRole.LeftHand:
                if (leftGrabbingSet.Remove(releasedObj.gameObject) && leftGrabbingSet.Count == 0)
                {
                }
                break;
        }

        // 网络消息相关
        if (selfid >= 0)
        {
            ArrayList msg = new ArrayList();
            msg.Add((Int64)CommandDefine.FirstLayer.Course);
            msg.Add((Int64)CommandDefine.SecondLayer.Release);
            msg.Add((Int64)selfid);
            NetworkCommunicate.getInstance().ReqCommand(UserInfor.getInstance().RoomId, UserInfor.getInstance().UserId, msg);
        }
    }

    public void OnDrop(BasicGrabbable releasedObj)
    {
    }

}
