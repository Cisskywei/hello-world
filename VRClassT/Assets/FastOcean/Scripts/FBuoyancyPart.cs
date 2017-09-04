/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace FastOcean
{
    public class FBuoyancyPart : MonoBehaviour
    {
        public float radius = 0.3f;

        [Range(0.01f, 10f)]
        public float density = 5f;

        [Range(0.0f, 10.0f)]
        public float stickyness = 5f;

        [Range(0.0f, 10.0f)]
        public float dragCoefficient = 3f;

        [Range(0.0f, 1.0f)]
        public float upwardsFactor = 0.9f;

        private float volume;
        
        public float mass { get; private set; }

        private Vector3 buoyantForce;

        private Vector3 dragForce;

        private Vector3 stickForce;
        
        private Vector3 outpos;
        private Vector3 normal;

        [NonSerialized]
        public bool usingGravity = false;

        public Vector3 totalForces { get { return buoyantForce + dragForce + stickForce; } }

        void Start()
        {
            UpdateProperties();
        }

        public void UpdateProperties()
        {
            volume = (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(radius, 3);

            mass = volume * density;
        }

        public void UpdateForces(Rigidbody body)
        {
            if (FOcean.instance == null || FOcean.instance.mainPG == null)
            {
                buoyantForce = Vector3.zero;
                dragForce = Vector3.zero;
                stickForce = Vector3.zero;
                return;
            }

            Vector3 pos = transform.position;
            FOceanGrid grid = FOcean.instance.ClosestGrid(this.transform);
            FOcean.instance.GetSurPointNormal(pos, out outpos, out normal, grid);

            Vector3 delta = outpos - pos;
            float mergedVolume = CalculateMersionVolume(radius, delta.y);

            //assume that waterDensity = 1
            float Fb = mergedVolume;
            if(usingGravity)
            {
                Fb *= FOcean.g;
            }

            //make more upwards
            normal = Vector3.Lerp(normal, Vector3.up, upwardsFactor);

            buoyantForce = normal * Fb;

            Vector3 velocity = body.velocity - grid.GetDrift(pos);

            float vm = velocity.magnitude;
            velocity = -velocity.normalized * vm * vm;

            dragForce = 0.5f * dragCoefficient * Fb * velocity;

            stickForce = Vector3.Scale(normal,delta) * mass * stickyness;

        }

        float CalculateMersionVolume(float r, float dy)
        {
            float h = dy + radius;

            float d = 2.0f * r - h;

            if (d <= 0.0f)
            {
                return volume;
            }
            else if (d > 2.0f * r)
            {
                return 0.0f;
            }

            float c = Mathf.Sqrt(h * d);

            return Mathf.PI / 6.0f * h * ((3.0f * c * c) + (h * h));
        }

        void OnDrawGizmos()
        {
            if (!enabled) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.DrawLine(outpos, outpos + normal);
        }


    }
}
