using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using TinyFrameWork;
using UnityEngine;

public class HandCtrl : MonoBehaviour {

    public enum HandCtrlType
    {
        None = -1,

        Palm,
        Power,
        PointTo,
        HideSkin,
    }

    public enum LeftRight
    {
        Left = 0,
        Right,
    }

    public Animator anima;
    private HandCtrlType _state = HandCtrlType.Palm;

    // 用于显示隐藏手
    public SkinnedMeshRenderer skin;

    [SerializeField]
    private LeftRight _leftRight = LeftRight.Left;

    private void Start()
    {
        if(skin == null)
        {
            skin = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        if(anima == null)
        {
            anima = gameObject.GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        RegCommandListener();
        RegEventListener();
    }

    private void OnDisable()
    {
        UnRegCommandListener();
        UnRegEventListener();
    }

    private void RegEventListener()
    {
        switch(_leftRight)
        {
            case LeftRight.Left:
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.leftGripDown, GripDown);
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.leftGripUp, GripUp);
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.leftTriggerDown, TriggerDown);
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.leftTriggerUp, TriggerUp);
                break;
            case LeftRight.Right:
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.rightGripDown, GripDown);
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.rightGripUp, GripUp);
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.rightTriggerDown, TriggerDown);
                EventDispatcher.GetInstance().MainEventManager.AddEventListener(EventId.rightTriggerUp, TriggerUp);
                break;
            default:
                break;
        }
    }

    private void UnRegEventListener()
    {
        switch (_leftRight)
        {
            case LeftRight.Left:
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.leftGripDown, GripDown);
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.leftGripUp, GripUp);
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.leftTriggerDown, TriggerDown);
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.leftTriggerUp, TriggerUp);
                break;
            case LeftRight.Right:
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.rightGripDown, GripDown);
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.rightGripUp, GripUp);
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.rightTriggerDown, TriggerDown);
                EventDispatcher.GetInstance().MainEventManager.RemoveEventListener(EventId.rightTriggerUp, TriggerUp);
                break;
            default:
                break;
        }
    }

    private void GripDown()
    {
    }

    private void GripUp()
    {
    }

    private void TriggerDown()
    {
        if(anima == null)
        {
            return;
        }

        anima.SetTrigger("Power");
    }

    private void TriggerUp()
    {
        if (anima == null)
        {
            return;
        }

        anima.SetTrigger("PowerBack");
    }

    private void RegCommandListener()
    {

    }

    private void UnRegCommandListener()
    {

    }

    private HashSet<GameObject> rightGrabbingSet = new HashSet<GameObject>();
    private HashSet<GameObject> leftGrabbingSet = new HashSet<GameObject>();
    public void OnGrabbed(BasicGrabbable grabbedObj)
    {
        ViveColliderButtonEventData viveEventData;
        if (!grabbedObj.grabbedEvent.TryGetViveButtonEventData(out viveEventData)) { return; }

        switch (viveEventData.viveRole.ToRole<HandRole>())
        {
            case HandRole.RightHand:
                if (_leftRight == LeftRight.Right && rightGrabbingSet.Add(grabbedObj.gameObject) && rightGrabbingSet.Count == 1)
                {
                    skin.enabled = false;
                }
                break;

            case HandRole.LeftHand:
                if (_leftRight == LeftRight.Left && leftGrabbingSet.Add(grabbedObj.gameObject) && leftGrabbingSet.Count == 1)
                {
                    skin.enabled = false;
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
                if (_leftRight == LeftRight.Right && rightGrabbingSet.Remove(releasedObj.gameObject) && rightGrabbingSet.Count == 0)
                {
                    skin.enabled = true;
                }
                break;

            case HandRole.LeftHand:
                if (_leftRight == LeftRight.Left && leftGrabbingSet.Remove(releasedObj.gameObject) && leftGrabbingSet.Count == 0)
                {
                    skin.enabled = true;
                }
                break;
        }
    }

}
