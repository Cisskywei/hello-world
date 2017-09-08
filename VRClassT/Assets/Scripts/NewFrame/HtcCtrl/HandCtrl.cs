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

    public bool isSubject = false;

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
        RegEventListener();
    }

    private void OnDisable()
    {
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

    public void GripDown()
    {
    }

    public void GripUp()
    {
    }

    public void TriggerDown()
    {
        if(anima == null)
        {
            return;
        }

        anima.SetTrigger("Power");

        if(isSubject)
        {
            ArrayList a = new ArrayList();
            a.Add((Int64)CommandDefine.FirstLayer.Lobby);
            a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
            a.Add((Int64)_leftRight);
            a.Add((Int64)HandCtrlType.Power);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
        }
    }

    public void TriggerUp()
    {
        if (anima == null)
        {
            return;
        }

        anima.SetTrigger("Palm");

        if (isSubject)
        {
            ArrayList a = new ArrayList();
            a.Add((Int64)CommandDefine.FirstLayer.Lobby);
            a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
            a.Add((Int64)_leftRight);
            a.Add((Int64)HandCtrlType.Palm);
            CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
        }
    }

    public void Palm()
    {
        if (anima == null)
        {
            return;
        }

        anima.SetTrigger("Palm");
    }

    public void Power()
    {
        if (anima == null)
        {
            return;
        }

        anima.SetTrigger("Power");
    }

    public void PointTo()
    {
        if (anima == null)
        {
            return;
        }

        anima.SetTrigger("PointTo");
    }

    public void ShowHideSkin(bool isshow = false)
    {
        if(skin == null)
        {
            return;
        }

        if(skin.enabled != isshow)
        {
            skin.enabled = isshow;
        }
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
                    if (isSubject)
                    {
                        ArrayList a = new ArrayList();
                        a.Add((Int64)CommandDefine.FirstLayer.Lobby);
                        a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
                        a.Add((Int64)_leftRight);
                        a.Add((Int64)HandCtrlType.HideSkin);
                        a.Add((Int64)0);
                        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
                    }
                }
                break;

            case HandRole.LeftHand:
                if (_leftRight == LeftRight.Left && leftGrabbingSet.Add(grabbedObj.gameObject) && leftGrabbingSet.Count == 1)
                {
                    skin.enabled = false;
                    if (isSubject)
                    {
                        ArrayList a = new ArrayList();
                        a.Add((Int64)CommandDefine.FirstLayer.Lobby);
                        a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
                        a.Add((Int64)_leftRight);
                        a.Add((Int64)HandCtrlType.HideSkin);
                        a.Add((Int64)0);
                        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
                    }
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
                    if (isSubject)
                    {
                        ArrayList a = new ArrayList();
                        a.Add((Int64)CommandDefine.FirstLayer.Lobby);
                        a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
                        a.Add((Int64)_leftRight);
                        a.Add((Int64)HandCtrlType.HideSkin);
                        a.Add((Int64)1);
                        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
                    }
                }
                break;

            case HandRole.LeftHand:
                if (_leftRight == LeftRight.Left && leftGrabbingSet.Remove(releasedObj.gameObject) && leftGrabbingSet.Count == 0)
                {
                    skin.enabled = true;
                    if (isSubject)
                    {
                        ArrayList a = new ArrayList();
                        a.Add((Int64)CommandDefine.FirstLayer.Lobby);
                        a.Add((Int64)CommandDefine.SecondLayer.PlayerOrder);
                        a.Add((Int64)_leftRight);
                        a.Add((Int64)HandCtrlType.HideSkin);
                        a.Add((Int64)1);
                        CommandSend.getInstance().Send((int)UserInfor.getInstance().RoomId, (int)UserInfor.getInstance().UserId, a);
                    }
                }
                break;
        }
    }

}
