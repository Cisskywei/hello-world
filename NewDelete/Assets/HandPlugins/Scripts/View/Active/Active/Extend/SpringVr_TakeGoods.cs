using UnityEngine;
using System.Collections.Generic;

public class SpringVr_TakeGoods : SpringVr_Base
{
    #region 声明数据
    private SpringVr_HandActiveCtrlExtend rightHandActiveCtrlExtend;                                                                                 //"RightHandCtrl"脚本的父类
    private SpringVr_HandActiveCtrlExtend LeftHandActiveCtrlExtend;                                                                                  //"LeftHandCtrl"脚本的父类
    private GameObject handEulerCtrl;
    private Transform inHandGoods;
    private bool m_bolHandCanGetGoods = false;
    private bool m_bolCheckOtherFingerGetGoods;
    private bool m_bolCheckOtherFingerGiveUpGoods;
    private bool m_bolIsTakeGoods;
    private int m_iMark;
    [SerializeField, TooltipAttribute("同一只手上可以拿起物体的所有手指")]
    private SpringVr_TakeGoods[] otherCollider;
    [SerializeField, TooltipAttribute("是否是主要拿起物体手指")]
    private bool m_bolIsMainTakeGoodsFinger;
    public Transform takeGoods;
    public Transform parentTran;
    [SerializeField, TooltipAttribute("选择左右手")]
    public WitchHand handChoose;
    #endregion

    #region 函数

    #region 初始化
    void Start()
    {
        if (m_bolIsMainTakeGoodsFinger)
        {
            if (handChoose == WitchHand.RightHand)                                                                                    //如果“RightHandCtrl”脚本为打开状态，则将右手加入可以拿取物体的事件
            {
                SpringVr_RightHandCtrl.Instace.eve_CanGetGoods += GetGoods;       //将“拿取物体方法”加入到右手拿取物体的事件当中
            }
            else                                                                                      //如果“LeftHandCtrl”脚本为打开状态，则将左手加入可以拿取物体的事件
            {
                SpringVr_LeftHandCtrl.Instace.eve_CanGetGoods += GetGoods;       //将“拿取物体方法”加入到左手拿取物体的事件当中
            }
        }
    }
    #endregion

    void Update()
    {
        if (m_bolIsMainTakeGoodsFinger)
        {
            if (!m_bolHandCanGetGoods && takeGoods != null)
            {
                DiscardGoods();
                takeGoods = null;
            }
        }
    }
    /// <summary>
    /// 控制抓取物体的开关方法
    /// </summary>
    /// <param name="tempBol">是否可以抓取的开关标识</param>
    /// <param name="tempTran">整个手模型的每一个关节</param>
    private void GetGoods(bool tempBol, List<Transform> tempTran)
    {
        //print(transform.localEulerAngles);
        if (tempBol != m_bolHandCanGetGoods)
        {
            ++m_iMark;
            if (m_iMark > 5)
            {
                m_bolHandCanGetGoods = tempBol;
                m_iMark = 0;
            }
        }
        else
        {
            m_iMark = 0;
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if (takeGoods == null && collider.tag != "Player")
        {
            takeGoods = collider.transform;
        }
        if (m_bolIsMainTakeGoodsFinger && !m_bolIsTakeGoods && collider.tag != "Player")
        {
            m_bolCheckOtherFingerGetGoods = false;
            for (int i = 1; i < otherCollider.Length; i++)
            {
                m_bolCheckOtherFingerGetGoods = otherCollider[i].takeGoods == takeGoods ? true : m_bolCheckOtherFingerGetGoods;
            }
            //print(m_bolCheckOtherFingerGetGoods);
            if (m_bolCheckOtherFingerGetGoods)
            {
                TakeGoodsToHand();
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.transform == takeGoods)
        {
            if (m_bolIsMainTakeGoodsFinger)
            {
                if (otherCollider[0].m_bolIsTakeGoods)
                {
                    DiscardGoods();
                }
                else
                {
                    takeGoods = null;
                }
            }
            else
            {
                takeGoods = null;
                if (otherCollider[0].m_bolIsTakeGoods)
                {
                    m_bolCheckOtherFingerGetGoods = false;
                    for (int i = 1; i < otherCollider.Length; i++)
                    {
                        if (otherCollider[i] != this)
                        {
                            m_bolCheckOtherFingerGetGoods = otherCollider[i].takeGoods == collider.transform ? true : m_bolCheckOtherFingerGetGoods;
                        }
                    }
                    //print(m_bolCheckOtherFingerGetGoods);
                    if (!m_bolCheckOtherFingerGetGoods)
                    {
                        otherCollider[0].DiscardGoods();
                    }
                }
            }
        }
    }

    private void TakeGoodsToHand()
    {
        if (m_bolHandCanGetGoods)
        {
            m_bolIsTakeGoods = true;
            takeGoods.parent = parentTran;
            takeGoods.GetComponent<Rigidbody>().isKinematic = true;
            SpringVr_HandShake.Instance.HandShakeInterface(handChoose, 50);
        }
    }

    public void DiscardGoods()
    {
        if (m_bolIsTakeGoods)
        {
            m_bolIsTakeGoods = false;
            takeGoods.parent = null;
            takeGoods.GetComponent<Rigidbody>().isKinematic = false;
            takeGoods = null;
        }
    }
    #endregion
}
