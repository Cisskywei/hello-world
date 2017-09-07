using HTC.UnityPlugin.Vive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableListener : MonoBehaviour {

    private HashSet<GameObject> rightGrabbingSet = new HashSet<GameObject>();
    private HashSet<GameObject> leftGrabbingSet = new HashSet<GameObject>();

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
    }

    public void OnDrop(BasicGrabbable releasedObj)
    {
    }

}
