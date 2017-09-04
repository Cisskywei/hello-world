using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringVr_HandPositionView : MonoBehaviour
{
    #region 成员变量
    private Transform       RightHand;
    private Transform       LeftHand;
    private Transform       RightArm;
    private Transform       LeftArm;
    private Vector3         m_v3ShoulderOffest;
    private float           m_floShoulderDis;
    private float           m_floTempShoulderDis;
    private float           m_floUpdateSpeed;
    private Quaternion      m_quaTrackerOffset;
    #endregion

    #region 函数

    #region 初始化函数
    private void Awake()
    {
        RightArm  = transform.Find("RightHand");
        LeftArm = transform.Find("LeftHand");
        RightHand = RightArm.GetChild(0).GetChild(0);
        LeftHand = LeftArm.GetChild(0).GetChild(0);
        m_v3ShoulderOffest = new Vector3(0, 0.25f, 0);
        m_floShoulderDis = 20.0f;
        m_quaTrackerOffset = Quaternion.Euler(-14, 90, 90);
        m_floUpdateSpeed = 0.9f;
    }
    #endregion

    #region 本地赋值函数
    /// <summary>八传感器控制两个胳膊大臂的位置 </summary>
    /// <param name="rightArmTarget">右臂位置</param>
    /// <param name="leftArmTarget">左臂位置</param>
    public void ArmPositionAssignment(Transform rightArmTarget, Transform leftArmTarget)
    {
        RightArm.position = rightArmTarget.position;
        LeftArm.position = leftArmTarget.position;
    }
    /// <summary>八传感器控制肩膀的位置和角度</summary>
    /// <param name="headPosition">头的位置</param>
    public void ArmPositionAssignment(Transform headPosition)
    {
        transform.position = headPosition.position - m_v3ShoulderOffest;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, headPosition.eulerAngles.y - 90, transform.eulerAngles.z);
        if (m_floShoulderDis != m_floTempShoulderDis)
        {
            RightArm.localPosition = new Vector3(0, 0, -m_floShoulderDis / 100);
            LeftArm.localPosition = new Vector3(0, 0, m_floShoulderDis / 100);
            m_floTempShoulderDis = m_floShoulderDis;
        }
    }
    /// <summary>ViveTracker控制手的位置</summary>
    /// <param name="rightHandTarget">被Tracker赋值的Transform</param>
    /// <param name="leftHandTarget">被Tracker赋值的Transform</param>
    public void ViveTrackerHandPositionAssignment(Transform rightHandTarget, Transform leftHandTarget)
    {
        RightHand.position = Vector3.Lerp(RightHand.position, rightHandTarget.position, m_floUpdateSpeed);
        RightHand.rotation = Quaternion.Lerp(RightHand.rotation, rightHandTarget.rotation * m_quaTrackerOffset, m_floUpdateSpeed);
        LeftHand.position = Vector3.Lerp(LeftHand.position, leftHandTarget.position, m_floUpdateSpeed);
        LeftHand.rotation = Quaternion.Lerp(LeftHand.rotation, leftHandTarget.rotation * m_quaTrackerOffset, m_floUpdateSpeed);
    }
    /// <summary>其他定位控制手的位置</summary>
    /// <param name="rightHandTarget">被Tracker赋值的Transform</param>
    /// <param name="leftHandTarget">被Tracker赋值的Transform</param>
    public void OtherTrackerHandPositionAssignment(Transform rightHandTarget, Transform leftHandTarget)
    {
        RightHand.position = Vector3.Lerp(RightHand.position, rightHandTarget.position, m_floUpdateSpeed);
        RightHand.rotation = Quaternion.Lerp(RightHand.rotation, rightHandTarget.rotation, m_floUpdateSpeed);
        LeftHand.position = Vector3.Lerp(LeftHand.position, leftHandTarget.position, m_floUpdateSpeed);
        LeftHand.rotation = Quaternion.Lerp(LeftHand.rotation, leftHandTarget.rotation, m_floUpdateSpeed);
    }
    #endregion

    #region 远程客户端赋值函数

    /// <summary>胳膊位置的赋值</summary>
    /// <param name="rightArmTarget">右手大臂的位置</param>
    /// <param name="leftArmTarget">左手大臂的位置</param>
    public void ArmPositionAssignment(Vector3 rightArmTarget, Vector3 leftArmTarget)
    {
        RightArm.position = rightArmTarget;
        LeftArm.position = leftArmTarget;
    }
   /// <summary>肩膀位置的赋值</summary>
   /// <param name="headPosition">头的位置</param>
   /// <param name="headRotation">头的角度</param>
    public void ArmPositionAssignment(Vector3 headPosition,Quaternion headRotation)
    {
        transform.position = headPosition - m_v3ShoulderOffest;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, headRotation.eulerAngles.y - 90, transform.eulerAngles.z);
        if (m_floShoulderDis != m_floTempShoulderDis)
        {
            RightArm.localPosition = new Vector3(0, 0, m_floShoulderDis / 100);
            LeftArm.localPosition = new Vector3(0, 0, -m_floShoulderDis / 100);
            m_floTempShoulderDis = m_floShoulderDis;
        }
    }
    /// <summary>ViveTracker的赋值方法</summary>
    /// <param name="rightHandPosition">右手Tracker位置</param>
    /// <param name="rightHandRotation">右手Tracker角度</param>
    /// <param name="leftHandPosition">左手Tracker位置</param>
    /// <param name="LeftHandRotation">左手Tracker角度</param>
    public void ViveTrackerHandPositionAssignment(Vector3 rightHandPosition,Quaternion rightHandRotation, Vector3 leftHandPosition,Quaternion LeftHandRotation)
    {
        RightHand.position = Vector3.Lerp(RightHand.position, rightHandPosition, m_floUpdateSpeed);
        RightHand.rotation = Quaternion.Lerp(RightHand.rotation, rightHandRotation * m_quaTrackerOffset, m_floUpdateSpeed);
        LeftHand.position = Vector3.Lerp(LeftHand.position, leftHandPosition, m_floUpdateSpeed);
        LeftHand.rotation = Quaternion.Lerp(LeftHand.rotation, LeftHandRotation * m_quaTrackerOffset, m_floUpdateSpeed);
    }
    /// <summary>其他定位得赋值方法</summary>
    /// <param name="rightHandPosition">右手Tracker位置</param>
    /// <param name="rightHandRotation">右手Tracker角度</param>
    /// <param name="leftHandPosition">左手Tracker位置</param>
    /// <param name="LeftHandRotation">左手Tracker角度</param>
    public void OtherTrackerHandPositionAssignment(Vector3 rightHandPosition, Quaternion rightHandRotation, Vector3 leftHandPosition, Quaternion LeftHandRotation)
    {
        RightHand.position = Vector3.Lerp(RightHand.position, rightHandPosition, m_floUpdateSpeed);
        RightHand.rotation = Quaternion.Lerp(RightHand.rotation, rightHandRotation, m_floUpdateSpeed);
        LeftHand.position = Vector3.Lerp(LeftHand.position, leftHandPosition, m_floUpdateSpeed);
        LeftHand.rotation = Quaternion.Lerp(LeftHand.rotation, LeftHandRotation, m_floUpdateSpeed);
    }
    #endregion

    #endregion
}
