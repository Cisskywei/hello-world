using UnityEngine;
using System.Collections;

public class SpringVr_HandShakeCtrl : SpringVr_Base {
    public WitchHand handChoose;
    [SerializeField,Range(25,50)]
    public int m_iShakeStrength = 50;
    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag != "Player")
        {
            SpringVr_HandShake.Instance.HandShakeInterface(handChoose, m_iShakeStrength);
        }
    }
}  