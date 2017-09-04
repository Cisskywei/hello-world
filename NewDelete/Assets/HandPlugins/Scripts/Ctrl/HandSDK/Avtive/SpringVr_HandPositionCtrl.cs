using UnityEngine;
using System.Collections;

public class SpringVr_HandPositionCtrl : SpringVr_Base
{
    #region 枚举
    public enum PositioningSystem
    {
        ViveTracker,
        OtherTracker,
        HeadPositioning,
        NoPositioning
    }
    #endregion

    #region 成员变量
    [HideInInspector]
    public PositioningSystem positioningSystem;
    #endregion

    #region 函数

    #region 初始化
    #endregion

    #region 角度、位置更新
    void Update()
    {
        switch (positioningSystem)
        {
            case PositioningSystem.ViveTracker:
                if (SpringVr_Summary.Instance.RightHandTracker == null || SpringVr_Summary.Instance.LeftHandTracker == null)
                {
                    Debug.LogError("[Spring-Vr]请注意物体HandSDK上控制手位置的物体为空");
                }
                else
                {
                    if (SpringVr_GUICtrl.Instance.m_bolChangeLeftRightCtrl)
                    {
                        HandPositionView.ViveTrackerHandPositionAssignment(SpringVr_Summary.Instance.LeftHandTracker, SpringVr_Summary.Instance.RightHandTracker);
                    }
                    else
                    {
                        HandPositionView.ViveTrackerHandPositionAssignment(SpringVr_Summary.Instance.RightHandTracker, SpringVr_Summary.Instance.LeftHandTracker);
                    }
                }
                break;
            case PositioningSystem.HeadPositioning:
                if (SpringVr_Summary.Instance.HeadTracker == null)
                {
                    Debug.LogError("[Spring-Vr]请注意物体HandSDK上控制头位置的物体为空");
                }
                else
                {
                    HandPositionView.ArmPositionAssignment(SpringVr_Summary.Instance.HeadTracker);
                }
                break;
            case PositioningSystem.OtherTracker:
                if (SpringVr_Summary.Instance.RightHandTracker == null || SpringVr_Summary.Instance.LeftHandTracker == null)
                {
                    Debug.LogError("[Spring-Vr]请注意物体HandSDK上控制手位置的物体为空");
                }
                else
                {
                    if (SpringVr_GUICtrl.Instance.m_bolChangeLeftRightCtrl)
                    {
                        HandPositionView.OtherTrackerHandPositionAssignment(SpringVr_Summary.Instance.LeftHandTracker, SpringVr_Summary.Instance.RightHandTracker);
                    }
                    else
                    {
                        HandPositionView.OtherTrackerHandPositionAssignment(SpringVr_Summary.Instance.RightHandTracker, SpringVr_Summary.Instance.LeftHandTracker);
                    }
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #endregion
}