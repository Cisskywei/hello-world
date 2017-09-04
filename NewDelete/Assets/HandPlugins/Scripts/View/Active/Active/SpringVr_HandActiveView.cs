using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SpringVr;

public abstract class SpringVr_HandActiveView : MonoBehaviour
{
    #region 枚举
    public enum HandDistinguish
    {
        RightHand,
        LeftHand
    }

    public enum HandModelChoose
    {
        RealPersonHand,
        OhterHand
    }
    #endregion

    #region 成员变量

    [Header("是否打开手指航向角,勾选为打开手指航向角，不勾选为关闭手指航向角")]
    public      bool                m_bolOpenOrCloseCourseAngle;
    [HideInInspector]
    public      float               m_floHorizontalValue;
    [HideInInspector]
    public      List<Transform>     allHandJoint;
    [HideInInspector]
    public      Quaternion[]        allJointWorldCodSys;
    [HideInInspector]
    public      Quaternion          m_qThumbExcursion1;
    [HideInInspector]
    public      Quaternion          m_qThumbOffset;
    [Header("手模型类型选择")]
    public      HandModelChoose     handModelChoose; 
    [HideInInspector]
    public      int                 m_iBeginJoint;
    private     float               m_floRotateSpeed;
    private     float               m_floPitch;
    private     float               m_floYaw;
    private     Quaternion[]        allCalculateJointWorldCodSys;
    private     Quaternion          m_qThumbExcursion;
    private     Quaternion          m_qThumbExcursion0;
    private     Vector3             m_v3ThumbMiddleJointOffset;
    private     Vector3[]           m_v3sGetLocalJointsVec3;

    private     Vector3             m_vThumb3SelfJointOffset;
    protected   Transform           bigArm;
    protected   Transform           littleArm;
    protected   Transform           hand;
    protected   List<Transform>     allHandFirstJoint;
    protected   List<Transform>     allHandLastJoint;
    protected   List<Transform>     allFarClientJoints;
    protected   HandDistinguish     handDistinguish;
    #endregion

    #region 委托
    public delegate void AssignmentToFirstThumbJoint(int beginJoint);
    public AssignmentToFirstThumbJoint m_delAssToFirstThumbJoint;
    #endregion

    #region 函数

    #region 初始化函数

    private void Awake()
    {
        OnAwake();
        DataInit();
    }
    protected virtual void OnAwake() { }
    private void Start()
    {
        AssignmentFunctionInit();
    }
 
    private void DataInit()
    {
        SpringVrDataDeal.Init();
        allHandJoint                    = new List<Transform>();
        allHandFirstJoint               = new List<Transform>();
        allHandLastJoint                = new List<Transform>();
        allJointWorldCodSys             = new Quaternion[9];
        allCalculateJointWorldCodSys    = new Quaternion[10];
        m_v3sGetLocalJointsVec3         = new Vector3[17];
        m_floRotateSpeed                = 0.3333f;
        switch (handDistinguish)
        {
            case HandDistinguish.RightHand:
                m_qThumbExcursion0          = SpringVrDataDeal.m_qThumbExcursion0_Right;
                m_qThumbExcursion           = SpringVrDataDeal.m_qThumbExcursion_Right;
                m_qThumbOffset              = new Quaternion(SpringVrDataDeal.m_floX_Right, SpringVrDataDeal.m_floY_Right, SpringVrDataDeal.m_floZ_Right, SpringVrDataDeal.m_floW_Right);

                m_v3ThumbMiddleJointOffset  = SpringVrDataDeal.m_vThumb3MiddleJointOffset_Right;
                m_vThumb3SelfJointOffset    = SpringVrDataDeal.m_vThumb3SelfJointOffset_Right;
                break;
            case HandDistinguish.LeftHand:
                m_qThumbExcursion0          = SpringVrDataDeal.m_qThumbExcursion0_Left;
                m_qThumbExcursion           = SpringVrDataDeal.m_qThumbExcursion_Left;
                m_qThumbOffset              = new Quaternion(SpringVrDataDeal.m_floX_Left, SpringVrDataDeal.m_floY_Left, SpringVrDataDeal.m_floZ_Left, SpringVrDataDeal.m_floW_Left);
                m_v3ThumbMiddleJointOffset  = SpringVrDataDeal.m_vThumb3MiddleJointOffset_Left;
                m_vThumb3SelfJointOffset    = SpringVrDataDeal.m_vThumb3SelfJointOffset_Left;
                break;
            default:
                m_qThumbOffset = new Quaternion(SpringVrDataDeal.m_floX_Left, SpringVrDataDeal.m_floY_Left, SpringVrDataDeal.m_floZ_Left, SpringVrDataDeal.m_floW_Left);
                break;
        }
        
        m_qThumbExcursion1              = Quaternion.Inverse(Quaternion.Inverse(m_qThumbExcursion) * m_qThumbExcursion0);

        littleArm = bigArm.GetChild(0);
        hand = littleArm.GetChild(0);

        allHandJoint.Add(bigArm);
        allHandJoint.Add(littleArm);
        allHandJoint.Add(hand);
        allHandJoint.Add(hand.GetChild(0).GetChild(0));

        allHandFirstJoint.Add(hand.GetChild(0));
        allHandLastJoint.Add(hand.GetChild(0).GetChild(0).GetChild(0));

        for (int i = 1; i < 5; i++)
        {
            allHandFirstJoint.Add(hand.GetChild(i));
            allHandJoint.Add(hand.GetChild(i).GetChild(0));
            allHandLastJoint.Add(hand.GetChild(i).GetChild(0).GetChild(0));
        }
    }
    private void AssignmentFunctionInit()
    {
        switch (handModelChoose)
        {
            case HandModelChoose.RealPersonHand:
                m_delAssToFirstThumbJoint = RealPersonHandAssThumbJoint;
                break;
            case HandModelChoose.OhterHand:
                m_delAssToFirstThumbJoint = OtherHandAssThumbJoint;
                break;
            default:
                break;
        }
        allFarClientJoints = new List<Transform>();
        allFarClientJoints.Add(bigArm);
        allFarClientJoints.Add(littleArm);
        allFarClientJoints.Add(hand);
        allFarClientJoints.Add(hand.GetChild(0));
        allFarClientJoints.Add(hand.GetChild(0).GetChild(0).GetChild(0));
        for (int i = 1; i < 5; i++)
        {
            allFarClientJoints.Add(hand.GetChild(i));
            allFarClientJoints.Add(hand.GetChild(i).GetChild(0));
            allFarClientJoints.Add(hand.GetChild(i).GetChild(0).GetChild(0));
        }
    }
    #endregion

    #region 赋值函数

    #region 本地客户端赋值函数
    public List<Transform> SixSensorsDataDeal(float[] quatData)
    {
        if (quatData == null) return null;
        DataDeal(quatData, 6, m_iBeginJoint);
        switch (handModelChoose)
        {
            case HandModelChoose.RealPersonHand:
                switch (handDistinguish)
                {
                    case HandDistinguish.RightHand:
                        m_floPitch = -allHandFirstJoint[0].localEulerAngles.x;
                        break;
                    case HandDistinguish.LeftHand:
                        m_floPitch = allHandFirstJoint[0].localEulerAngles.x;
                        break;
                    default:
                        break;
                }
                allCalculateJointWorldCodSys[5] = Thumb3JointOffset(m_floPitch);
                allHandLastJoint[0].localRotation = Quaternion.Lerp(allHandLastJoint[0].localRotation, allCalculateJointWorldCodSys[5], m_floRotateSpeed);
                break;
            case HandModelChoose.OhterHand:
                break;
            default:
                break;
        }
        return allHandJoint;
    }

    public List<Transform> SevenSensorsDataDeal(float[] quatData)
    {
        if (quatData == null) return null;
        DataDeal(quatData, 7, m_iBeginJoint);
        m_floPitch = (Quaternion.Inverse(allHandJoint[3].rotation) * allJointWorldCodSys[6]).eulerAngles.z;
        if (handModelChoose == HandModelChoose.RealPersonHand)
        {
            allCalculateJointWorldCodSys[5] = SevenThumb3JointOffset(m_floPitch);
        }
        else
        {
            allCalculateJointWorldCodSys[5] = Thumb3JointOffset(m_floPitch);
        }
        allHandLastJoint[0].rotation = Quaternion.Lerp(allHandLastJoint[0].rotation, allCalculateJointWorldCodSys[5], m_floRotateSpeed);
        return allHandJoint;
    }

    public List<Transform> EightSensorsDataDeal(float[] quatData)
    {
        if (quatData == null) return null;
        DataDeal(quatData, 8, m_iBeginJoint);
        switch (handModelChoose)
        {
            case HandModelChoose.RealPersonHand:
                m_floPitch = EightThumb3JointOffset();
                allCalculateJointWorldCodSys[5] = Thumb3JointOffset(m_floPitch);
                allHandLastJoint[0].rotation = Quaternion.Lerp(allHandLastJoint[0].rotation, allCalculateJointWorldCodSys[5], m_floRotateSpeed);
                break;
            case HandModelChoose.OhterHand:
                break;
            default:
                break;
        }
        return allHandJoint;
    }

    public List<Transform> NineSensorsDataDeal(float[] quatData)
    {
        if (quatData == null) return null;
        DataDeal(quatData, 9, m_iBeginJoint);
        m_floPitch = (Quaternion.Inverse(allHandJoint[3].rotation) * allJointWorldCodSys[8]).eulerAngles.z;
        if (handModelChoose == HandModelChoose.RealPersonHand)
        {
            allCalculateJointWorldCodSys[5] = SevenThumb3JointOffset(m_floPitch);
        }
        else
        {
            allCalculateJointWorldCodSys[5] = Thumb3JointOffset(m_floPitch);
        }
        allHandLastJoint[0].rotation = Quaternion.Lerp(allHandLastJoint[0].rotation, allCalculateJointWorldCodSys[5], m_floRotateSpeed);
        return allHandJoint;
    }
    #endregion

    #region 远程客户端函数
    /// <summary>远程客户端赋值函数</summary>
    /// <param name="allJointsRot">Vector3数组的顺序为大臂/小臂/手掌/大拇指第一关节/大拇指第三关节/食指第1/2/3关节/中指第1/2/3关节/无名指第1/2/3关节/小拇指第1/2/3关节</param>
    /// <param name="frameRate">更新帧率（次数每秒）</param>
    /// <returns></returns>
    public List<Transform> FarClientAssignmentToJoints(Vector3[] allJointsRot,int frameRate)
    {
        for (int i = 0; i < allFarClientJoints.Count; i++)
        {
            allFarClientJoints[i].rotation = Quaternion.Lerp(allFarClientJoints[i].rotation, Quaternion.Euler(allJointsRot[i]), 10.0f / frameRate);
        }
        return allHandJoint;
    }
    /// <summary>获取所有关节的旋转角</summary>
    /// <returns>返回所有关节旋转角的数组</returns>
    public Vector3[] FarClientGetValueFromJoints()
    {
        for (int i = 0; i < 17; i++)
        {
            m_v3sGetLocalJointsVec3[i] = allFarClientJoints[i].eulerAngles;
        }
        return m_v3sGetLocalJointsVec3;
    }
    #endregion

    #endregion

    #region 数据处理函数

    protected Quaternion Thumb3JointOffset(float offsetValue)
    {
        return SpringVrDataDeal.Thumb3JointOffset(allHandJoint[3].rotation, m_v3ThumbMiddleJointOffset, m_vThumb3SelfJointOffset, offsetValue);
    }
    private void DataDeal(float[] QuatData, int NumberOfSensors,int beginJointNum)
    {
        for (int i = 0; i < NumberOfSensors; i++)
        {
            allJointWorldCodSys[i] = new Quaternion(QuatData[i * 4 + 1], QuatData[i * 4 + 3], QuatData[i * 4 + 2], -QuatData[i * 4]);
        }
        for (int i = 0; i < NumberOfSensors; i++)
        {
            allJointWorldCodSys[i] = Quaternion.Euler(allJointWorldCodSys[i].eulerAngles.x, allJointWorldCodSys[i].eulerAngles.y + m_floHorizontalValue, allJointWorldCodSys[i].eulerAngles.z);
        }
        for (int i = 4; i < allHandJoint.Count; i++)
        {
            m_floPitch = (Quaternion.Inverse(allJointWorldCodSys[2 - beginJointNum]) * allJointWorldCodSys[i - beginJointNum]).eulerAngles.z;
            m_floPitch = SpringVrDataDeal.QuatDataDeal(m_floPitch);
            if (m_bolOpenOrCloseCourseAngle)
            {
                m_floYaw = (Quaternion.Inverse(allJointWorldCodSys[2 - beginJointNum]) * allJointWorldCodSys[i - beginJointNum]).eulerAngles.y;
                if (m_floPitch < -100)
                {
                    allCalculateJointWorldCodSys[i - 3] = Quaternion.Euler(0, 0, m_floPitch * 2 / 5);
                }
                else
                {
                    allCalculateJointWorldCodSys[i - 3] = Quaternion.Euler(0, m_floYaw, m_floPitch * 2 / 5);
                }
            }
            else
            {
                allCalculateJointWorldCodSys[i - 3] = Quaternion.Euler(0, 0, m_floPitch * 2 / 5);
            }
            allJointWorldCodSys[i - beginJointNum] = Quaternion.Euler(0, 0, m_floPitch * 3 / 5);
            allCalculateJointWorldCodSys[i + 2] = Quaternion.Euler(0, 0, m_floPitch * 1 / 3);
        }
        FingersGetRotation(beginJointNum);
    }
    private void FingersGetRotation(int beginJointNum)
    {
        for (int i = beginJointNum; i < allHandJoint.Count - 5; i++)
        {
            allHandJoint[i].rotation = Quaternion.Lerp(allHandJoint[i].rotation, allJointWorldCodSys[i - beginJointNum], m_floRotateSpeed);
        }
        m_delAssToFirstThumbJoint(beginJointNum);
        allHandFirstJoint[0].localRotation = Quaternion.Lerp(allHandFirstJoint[0].localRotation, allCalculateJointWorldCodSys[0], m_floRotateSpeed);
        for (int i = 4; i < allHandJoint.Count; i++)
        {
            allHandFirstJoint[i - 3].localRotation = Quaternion.Lerp(allHandFirstJoint[i - 3].localRotation, allCalculateJointWorldCodSys[i - 3], m_floRotateSpeed);
            allHandJoint[i].localRotation = Quaternion.Lerp(allHandJoint[i].localRotation, allJointWorldCodSys[i - beginJointNum], m_floRotateSpeed);
            allHandLastJoint[i - 3].localRotation = Quaternion.Lerp(allHandLastJoint[i - 3].localRotation, allCalculateJointWorldCodSys[i + 2], m_floRotateSpeed);
        }
    }
    private void RealPersonHandAssThumbJoint(int beginJointNum)
    {
        allCalculateJointWorldCodSys[0] = SpringVrDataDeal.RealPersonHandAssThumbJoint(allHandJoint[2].rotation, allJointWorldCodSys[3 - beginJointNum], m_qThumbOffset, m_qThumbExcursion1);
    }

    private void OtherHandAssThumbJoint(int beginJointNum)
    {
        allCalculateJointWorldCodSys[0] = Quaternion.Inverse(allHandJoint[2].rotation) * allJointWorldCodSys[3 - beginJointNum];
    }
    public void ThumbOffsetCalibration()
    {
        m_qThumbOffset = SpringVrDataDeal.ThumbOffsetCalibration(allHandJoint[2].rotation, allJointWorldCodSys[3 - m_iBeginJoint], handDistinguish.ToString());
    }
    #endregion


    #region 抽象函数
    protected abstract Quaternion SevenThumb3JointOffset(float targetParame);
    protected abstract float EightThumb3JointOffset();
    #endregion

    #endregion
}
