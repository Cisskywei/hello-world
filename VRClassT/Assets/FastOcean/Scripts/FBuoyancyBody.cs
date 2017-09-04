/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FastOcean
{

	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
    public class FBuoyancyBody : MonoBehaviour
    {

        public float maxAngularVelocity = 0.05f;

        FBuoyancyPart[] m_buoyancy;

        void Start()
        {
            m_buoyancy = GetComponentsInChildren<FBuoyancyPart>();
        }

        void FixedUpdate()
        {

            Rigidbody body = GetComponent<Rigidbody>();

            if (body == null)
                body = gameObject.AddComponent<Rigidbody>();

            float mass = 0.0f;

            int count = m_buoyancy.Length;
            for (int i = 0; i < count; i++)
            {
                if (!m_buoyancy[i].enabled) continue;

                m_buoyancy[i].usingGravity = body.useGravity;
                m_buoyancy[i].UpdateProperties();
                mass += m_buoyancy[i].mass;
            }

            body.mass = mass;

            Vector3 pos = transform.position;
            Vector3 force = Vector3.zero;
            Vector3 torque = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                if (!m_buoyancy[i].enabled) continue;

                m_buoyancy[i].UpdateForces(body);

                Vector3 p = m_buoyancy[i].transform.position;
                Vector3 f = m_buoyancy[i].totalForces;
                Vector3 r = p - pos;

                force += f;
                torque += Vector3.Cross(r, f);

            }

            body.maxAngularVelocity = maxAngularVelocity;
            body.AddForce(force);
            body.AddTorque(torque);

        }

    }
}
