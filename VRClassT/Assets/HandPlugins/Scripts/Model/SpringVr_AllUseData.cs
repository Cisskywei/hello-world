using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class SpringVr_AllUseData : MonoBehaviour {

    #region 成员变量
    public SpringVr_RightHandActiveView RightHandActiveView;
    public SpringVr_LeftHandActiveView  LeftHandActiveView;
    public SpringVr_LeftHandCtrl        LeftHandCtrl;
    public SpringVr_RightHandCtrl       RightHandCtrl;
    public SpringVr_ReadLeftHandData    ReadLeftHandData;
    public SpringVr_ReadRightHandData   ReadRightHandData;
    public SpringVr_SerialPortTool      SerialPortTool;
    public SpringVr_Summary             Summary;
    public SpringVr_CommandShow         CommandShow;
    public SpringVr_GUICtrl             GUICtrl;
    public SpringVr_HandPositionView    HandPositionView;
    public Transform[]                  m_tranAllJoint;
    [HideInInspector]
    public int                          m_iClientID;
    [HideInInspector]
    public bool                         m_bolIsLocal;
    [HideInInspector]
    //public bool                         m_bolAwakeRun;

    private Vector3[]                   m_v3AllJoint;
    private Transform                   ReadData;
    private Transform                   Shoudler;
    private Transform                   RightHand;
    private Transform                   LeftHand;
    private Transform                   HandEulerCtrl;
    private Transform                   UIPanel;
    private int                         m_iJointNum;

    public Vector3[] V3AllJoint
    {
        get
        {
            if (m_v3AllJoint == null)
            {
                m_v3AllJoint = new Vector3[m_tranAllJoint.Length];
                Debug.Log(m_v3AllJoint.Length);
            }
            return m_v3AllJoint;
        }

        set
        {
            m_v3AllJoint = value;
        }
    }
    #endregion

    private void Awake()
    {
        m_bolIsLocal = true;
        if (RightHandActiveView == null)
        {
            ReadData = transform.Find("Model/SVReadData");
            Shoudler = transform.Find("View/Shoudler");
            RightHand = Shoudler.Find("RightHand");
            LeftHand = Shoudler.Find("LeftHand");
            HandEulerCtrl = transform.Find("Ctrl/HandEulerCtrl");
            UIPanel = transform.Find("View/Other/SpringVrUI");
            RightHandActiveView = RightHand.GetComponent<SpringVr_RightHandActiveView>();
            LeftHandActiveView = LeftHand.GetComponent<SpringVr_LeftHandActiveView>();
            LeftHandCtrl = HandEulerCtrl.GetComponent<SpringVr_LeftHandCtrl>();
            RightHandCtrl = HandEulerCtrl.GetComponent<SpringVr_RightHandCtrl>();
            ReadLeftHandData = ReadData.GetComponent<SpringVr_ReadLeftHandData>();
            ReadRightHandData = ReadData.GetComponent<SpringVr_ReadRightHandData>();
            SerialPortTool = ReadData.GetComponent<SpringVr_SerialPortTool>();
            Summary = transform.GetComponent<SpringVr_Summary>();
            CommandShow = ReadData.GetComponent<SpringVr_CommandShow>();
            GUICtrl = UIPanel.GetComponent<SpringVr_GUICtrl>();
        }
        m_iJointNum = m_tranAllJoint.Length;
        m_v3AllJoint = new Vector3[m_iJointNum];
    }

    #region 联网需要打开以下代码
    //private void FixedUpdate()
    //{
    //    if (m_bolIsLocal)
    //    {
    //        if (Time.frameCount % 5 == 0)
    //        {
    //            for (int i = 0; i < m_iJointNum; i++)
    //            {
    //                m_v3AllJoint[i] = m_tranAllJoint[i].eulerAngles;
    //            }
    //        }
    //    }
    //}
    #endregion

}
