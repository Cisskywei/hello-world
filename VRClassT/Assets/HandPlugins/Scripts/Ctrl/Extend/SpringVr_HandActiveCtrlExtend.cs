using UnityEngine;
using System.Collections.Generic;
using System;
using SpringVr;
public class SpringVr_HandActiveCtrlExtend : SpringVr_HandActiveCtrl
{
    #region 枚举
    public enum AllGesture
    {
        /// <summary>手掌</summary>
        Palm,
        /// <summary>摇滚</summary>
        Rock,
        /// <summary>食指中指伸出</summary>
        IndexMiddle,
        /// <summary>食指伸出</summary>
        Index,
        /// <summary>握拳</summary>
        Boxing,
        /// <summary>拇指抬起食指伸出</summary>
        ThumbIndexSeparate,
        /// <summary>拇指并拢食指伸出</summary>
        ThumbIndexCloseTogether,
    }
    #endregion

    #region 委托事件
    public delegate void del_CanGetGoods(bool tempBol, List<Transform> arm);
    /// <summary>可以拿起物体事件</summary>
    public event del_CanGetGoods eve_CanGetGoods;
    public delegate void del_Boxing(bool tempBol, List<Transform> arm);
    /// <summary>握拳事件</summary>
    public event del_Boxing eve_Boxing;
    public delegate void del_IndexFinger(bool tempBol, List<Transform> arm);
    /// <summary>只伸出食指事件</summary>
    public event del_IndexFinger eve_IndexFinger;
    public delegate void del_IndexMiddleFinger(bool tempBol, List<Transform> arm);
    /// <summary>伸出食指和中指事件</summary>
    public event del_IndexMiddleFinger eve_IndexMiddleFinger;
    public delegate void del_PalmDlg(bool tempBol, List<Transform> arm);
    /// <summary>手掌打开事件</summary>
    public event del_PalmDlg eve_PalmDlg;
    public delegate void del_RockDlg(bool tempBol, List<Transform> arm);
    /// <summary>摇滚手势事件</summary>
    public event del_RockDlg eve_RockDlg;
    public delegate void del_ThumbStage(SpringVr_GestureCalculateBase.ForearmKinestate tempTrend, List<Transform> arm);
    /// <summary>拇指运动状态</summary>
    public event del_ThumbStage eve_ThumbStage;
    public delegate void del_IndexThumbSeparateToClose(bool tempBol, List<Transform> arm);
    /// <summary>食指伸出情况下，动态的拇指由伸出到并拢</summary>
    public event del_IndexThumbSeparateToClose eve_IndexThumbSeparateToClose;
    public delegate void del_IndexThumbSeparate(bool tempBol, List<Transform> arm);
    /// <summary>食指伸出情况下，静态的拇指伸出</summary>
    public event del_IndexThumbSeparate eve_IndexThumbSeparate;
    public delegate void del_OK(bool tempBol, List<Transform> arm);
    /// <summary>食指伸出情况下，静态的拇指伸出</summary>
    public event del_OK eve_OK;
    public delegate void del_FingerMove(float tempFlo, List<Transform> arm);
    /// <summary>食指伸出情况下，静态的拇指伸出</summary>
    public event del_FingerMove eve_FingerMove;

    public delegate void del_PalmChangeStage(bool tempBol, List<Transform> arm);
    /// <summary>手掌改变状态</summary>
    public event del_PalmChangeStage eve_PalmChangeStage;
    /// <summary>食指伸开手势</summary>
    private SpringVr_GestureChoose shootGetstueChoose;
    /// <summary>食指中指伸开</summary>
    private SpringVr_GestureChoose indexMiddleGetsturCalculate;
    /// <summary>握拳手势</summary>
    private SpringVr_GestureChoose laserGetsturCalculate;
    /// <summary>拿取物体手势</summary>
    private SpringVr_GestureChoose getGoodsGetsturCalculate;
    /// <summary>手掌手势</summary>
    private SpringVr_GestureChoose palmGetsturCalculate;
    /// <summary>竖中指</summary>
    private SpringVr_GestureChoose middleGetsturCalculate;
    /// <summary>小臂运动状态</summary>
    private SpringVr_GestureChoose armTrendGetsturCalculate;
    /// <summary>拇指运动状态</summary>
    private SpringVr_GestureChoose thumbGetsturCalculate;
    /// <summary>手指动态运动</summary>
    private SpringVr_GestureChoose fingerMoveGetsturCalculate;
    /// <summary>手掌卷握</summary>
    private SpringVr_GestureChoose plamChangeGetsturCalculate;
    /// <summary>OK手势</summary>
    private SpringVr_GestureChoose oKGestureCalculate;
    /// <summary>食指伸出大拇指抬起</summary>
    private SpringVr_GestureChoose thumbIndexSeparate;
    /// <summary>食指伸出大拇指放下</summary>
    private SpringVr_GestureChoose thumbIndexCloseTogether;
    #endregion

    #region 成员变量
    protected List<Transform> targetTran = new List<Transform>();
    private int[] allFingerState;
    private AllGesture lastGesture;
    #endregion

    #region 函数

    #region 初始化
    protected override void OnStart()
    {
        base.OnStart();
        allFingerState = new int[8];
        thumbGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_ThumbGestureCalculate());
        shootGetstueChoose = new SpringVr_GestureChoose(new SpringVr_ShootGestureCalculate());
        laserGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_LaserGestureCalculate());
        getGoodsGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_GetGoodsGestureCalculate());
        palmGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_PalmGestureCalculate());
        middleGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_RockFingerGestureCalculate());
        indexMiddleGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_IndexMiddleGetsturCalculate());
        fingerMoveGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_FingerMoveGetsturCalculate());
        plamChangeGetsturCalculate = new SpringVr_GestureChoose(new SpringVr_PalmChangeGestureCalculate());
        thumbIndexSeparate = new SpringVr_GestureChoose(new SpringVr_ThumbIndexSeparateCalculate());
        thumbIndexCloseTogether = new SpringVr_GestureChoose(new SpringVr_ThumbIndexCloseTogetherCalculate());
        oKGestureCalculate = new SpringVr_GestureChoose(new SpringVr_OKGestureCalculate());
    }
    #endregion

    private void FixedUpdate()
    {
        if (readDataScript.aGroupByteData.Length == 0) return;
        HandRotationCtrl();
        GestureMovementRange(targetTran);
        ShootGetstueCalculate();
        LaserGetsturCalculate();
        GetGoodsGetsturCalculate();
        PalmGeststurCalculate();
        MiddleFingerGetsturCalculate();
        IndexMiddleGeststurCalculate();
        ThumbGetsturCalculate();
        FingerMoveGetsturCalculate();
        PalmChangeGetsturCalculate();
        IndexThumbSeparateToClose();
        IndexThumbSeparate();
        OKGestureCalculate();

        LastGestureCalculate();
    }

    /// <summary>手部旋转控制</summary>
    private void HandRotationCtrl()
    {
        targetTran = m_delAssToJoints(readDataScript.aGroupByteData);
    }

    #region 手势识别

    #region 动态
    private SpringVr_GestureCalculateBase.ForearmKinestate tempThumTrend;
    /// <summary>拇指运动状态</summary>
    private void ThumbGetsturCalculate()
    {
        tempThumTrend = thumbGetsturCalculate.GestureCalculate<SpringVr_GestureCalculateBase.ForearmKinestate>(allFingerState, targetTran, lastGesture);
        if (eve_ThumbStage != null)
        {
            eve_ThumbStage(tempThumTrend, targetTran);
        }
    }

    private float tempFingerMove;
    private void FingerMoveGetsturCalculate()
    {
        //Debug.Log(fingerMoveGetsturCalculate.GestureCalculate<float>(allFingerState, targetTran, lastGesture));
        tempFingerMove = fingerMoveGetsturCalculate.GestureCalculate<float>(allFingerState, targetTran, lastGesture);
        if (eve_FingerMove != null)
        {
            eve_FingerMove(tempFingerMove, targetTran);
        }
    }
    private bool tempPalmChange;
    private void PalmChangeGetsturCalculate()
    {
        tempPalmChange = plamChangeGetsturCalculate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_PalmChangeStage != null)
        {
            eve_PalmChangeStage(tempPalmChange, targetTran);
        }
    }
    private bool tempIndexThumbSeparateToClose;
    private void IndexThumbSeparateToClose()
    {
        tempIndexThumbSeparateToClose = thumbIndexCloseTogether.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_IndexThumbSeparateToClose != null)
        {
            eve_IndexThumbSeparateToClose(tempIndexThumbSeparateToClose, targetTran);
        }
        //Debug.Log(tempIndexThumbSeparateToClose);
    }
    #endregion

    #region 静态
    private bool tempMiddleFingerBol;
    private void MiddleFingerGetsturCalculate()
    {
        tempMiddleFingerBol = middleGetsturCalculate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_RockDlg != null)
        {
            eve_RockDlg(tempMiddleFingerBol, targetTran);
        }
    }
    private bool tempShootBol;
    private void ShootGetstueCalculate()
    {
        tempShootBol = shootGetstueChoose.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_IndexFinger != null)
        {
            eve_IndexFinger(tempShootBol, targetTran);
        }
    }
    private bool tempLaserBol;
    private void LaserGetsturCalculate()
    {
        tempLaserBol = laserGetsturCalculate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_Boxing != null)
        {
            eve_Boxing(tempLaserBol, targetTran);
        }
    }
    private bool tempGetGoodsBol = false;
    private void GetGoodsGetsturCalculate()
    {
        tempGetGoodsBol = getGoodsGetsturCalculate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_CanGetGoods != null)
        {
            eve_CanGetGoods(tempGetGoodsBol, targetTran);
        }
    }
    private bool tempPalmBol = false;
    private void PalmGeststurCalculate()
    {
        tempPalmBol = palmGetsturCalculate.GestureCalculate<bool>(allFingerState,targetTran, lastGesture);
        if (eve_PalmDlg != null)
        {
            eve_PalmDlg(tempPalmBol, targetTran);
        }
    }
    private bool tempIndexMiddleBol;
    private void IndexMiddleGeststurCalculate()
    {
        tempIndexMiddleBol = indexMiddleGetsturCalculate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_IndexMiddleFinger != null)
        {
            eve_IndexMiddleFinger(tempIndexMiddleBol, targetTran);
        }
    }
    private bool tempIndexThumbSeparate;
    private void IndexThumbSeparate()
    {
        tempIndexThumbSeparate = thumbIndexSeparate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        if (eve_IndexThumbSeparate != null)
        {
            eve_IndexThumbSeparate(tempIndexThumbSeparate, targetTran);
        }
        //Debug.Log(tempIndexThumbSeparate);
    }

    private bool tempOK;
    private void OKGestureCalculate()
    {
        tempOK = oKGestureCalculate.GestureCalculate<bool>(allFingerState, targetTran, lastGesture);
        //Debug.Log(tempOK);
        if (eve_OK != null)
        {
            eve_OK(tempOK, targetTran);
        }

    }
    private void LastGestureCalculate()
    {
        if (tempIndexMiddleBol)
        {
            lastGesture = AllGesture.IndexMiddle;
        }
        else if (tempPalmBol)
        {
            lastGesture = AllGesture.Palm;
        }
        else if (tempIndexThumbSeparate)
        {
            lastGesture = AllGesture.ThumbIndexSeparate;
        }
        else if (tempLaserBol)
        {
            lastGesture = AllGesture.Boxing;
        }
        else if (tempShootBol)
        {
            lastGesture = AllGesture.Index;
        }
        else if (tempMiddleFingerBol)
        {
            lastGesture = AllGesture.Rock;
        }
        
    }
    #endregion

    private void GestureMovementRange(List<Transform> allJoint)
    {
        allFingerState = SpringVrDataDeal.GestureMovementRange(allJoint, ctrlHand.name);
        //if (allFingerState[3] == 0)
        //{
        //    Debug.Log(ctrlHand.name + "并！");
        //}
        //else if (allFingerState[3] == 1)
        //{
        //    Debug.Log("开");
        //}
        //else if (allFingerState[3] == 2)
        //{
        //    Debug.Log("OK");
        //}
        //else if (allFingerState[3] == 3)
        //{
        //    Debug.Log("下");
        //}
    }
    #endregion

    #endregion

    // 只为测试
    public int[] getAllFingerState()
    {
        return allFingerState;
    }
}

