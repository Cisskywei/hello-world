using UnityEngine;
using System.Collections;

public class SpringVr_Base : MonoBehaviour {
    #region 枚举
    public enum WitchHand
    {
        RightHand,
        LeftHand
    }
    #endregion

    protected SpringVr_LeftHandActiveView LeftHandActiveView;
    protected SpringVr_RightHandActiveView RightHandActiveView;
    protected SpringVr_HandPositionView HandPositionView;
    private Transform Shoudler;
    private void Awake()
    {
        Shoudler = GameObject.Find("Shoudler").transform;
        HandPositionView = Shoudler.GetComponent<SpringVr_HandPositionView>();
        LeftHandActiveView = Shoudler.Find("LeftHand").GetComponent<SpringVr_LeftHandActiveView>();
        RightHandActiveView = Shoudler.Find("RightHand").GetComponent<SpringVr_RightHandActiveView>();
        OnAwake();
    }
    private void OnEnable()
    {
        Enable();
    }
    private void Start()
    {
        OnStart();
    }
    
    #region 虚函数
    protected virtual void OnAwake() { }
    protected virtual void Enable() { }
    protected virtual void OnStart() { }
    #endregion
}
