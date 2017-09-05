using UnityEngine;
using System.Collections.Generic;

public class SpringVr_HandActiveCtrl : SpringVr_Base
{
    [HideInInspector]
    public      GameObject ctrlHand;
    protected   Transform model;
    protected   SpringVr_HandActiveView handActiveView;
    protected   SpringVr_ReadDataBase readDataScript;
    protected   delegate List<Transform> AssignmentToJoints(float[] allData);
    protected   AssignmentToJoints m_delAssToJoints;
 
    protected override void OnStart()
    {
        model = SpringVr_SerialPortTool.Instance.transform;
        handActiveView = ctrlHand.GetComponent<SpringVr_HandActiveView>();
        HandViewCtrl();
        base.OnStart();
    }

    private void HandViewCtrl()
    {
        switch (SpringVr_SerialPortTool.Instance.numberOfSensors)
        {
            case SpringVr_SerialPortTool.NumberOfSensors.SixSensors:
                handActiveView.m_iBeginJoint = 2;
                m_delAssToJoints = handActiveView.SixSensorsDataDeal;
                break;
            case SpringVr_SerialPortTool.NumberOfSensors.SevenSensors:
                handActiveView.m_iBeginJoint = 2;
                m_delAssToJoints = handActiveView.SevenSensorsDataDeal;
                break;
            case SpringVr_SerialPortTool.NumberOfSensors.EightSensors:
                handActiveView.m_iBeginJoint = 0;
                m_delAssToJoints = handActiveView.EightSensorsDataDeal;
                break;
            case SpringVr_SerialPortTool.NumberOfSensors.NineSensors:
                handActiveView.m_iBeginJoint = 0;
                m_delAssToJoints = handActiveView.NineSensorsDataDeal;
                break;
            default:
                break;
        }
    }
}
