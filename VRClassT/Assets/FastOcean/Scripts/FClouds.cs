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

namespace FastOcean
{
    public enum eCLQuality
    {
        eCL_High = 0,
        eCL_Medium = 1,
        eCL_Fast = 2,
    }

    [ExecuteInEditMode]
    public class FClouds : FPostEffectsBase
    {
        public bool enableCloud = true;
        public eCLQuality quality = eCLQuality.eCL_Fast;

        public Shader CloudShader;

        public Color BaseColor = new Color(0.5f,0.5f,0.5f, 0.3f);
        public Color ScatterColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

        public float MinHeight = 0.0f;
        public float MaxHeight = 5.0f;
        [Range(0,10)]
        public float FadeDist = 2;
        public float Scale = 5;
        [Range(0,0.1f)]
        public float Thickness = 0.01f;

        [Range(0, 360f)]
        public float WindAngle = 180;
        [Range(0,1f)]
        public float WindSpeed = 0.05f;

        public Texture ValueNoiseTable;

        public Light Sun;
        
        public Material material
        {
            get
            {
                if (_Material == null && CloudShader != null)
                {
                    _Material = new Material(CloudShader);
#if UNITY_EDITOR
                    _Material.hideFlags = HideFlags.HideAndDontSave;
#endif
                }

                if (_Material != null && CloudShader == null)
                {
                    DestroyImmediate(_Material);
                }

                if (_Material != null && CloudShader != null && CloudShader != _Material.shader)
                {
                    DestroyImmediate(_Material);
                    _Material = new Material(CloudShader);
#if UNITY_EDITOR
                    _Material.hideFlags = HideFlags.HideAndDontSave;
#endif
                }

                return _Material;
            }
        }
        private Material _Material;

        protected override void Start()
        {
            if (_Material)
                DestroyImmediate(_Material);
            
            if (Application.isPlaying)
                InvokeRepeating("CheckEnable", 0f, 0.001f);
        }

        bool isEnable()
        {
            return enableCloud && !FOcean.instance.mobile && FOcean.instance.supportSM3 && !FOcean.instance.OnlyUnderWater();
        }

        protected override void CheckEnable()
        {
            if (FOcean.instance != null)
            {
                enabled = isEnable();

                if(FOcean.instance.mobile)
                {
                    quality = eCLQuality.eCL_Fast;
                }

                if (enabled)
                {
                    if (!FOcean.instance.needDepthBehaviour.Contains(this))
                        FOcean.instance.needDepthBehaviour.Add(this);
                }
                else
                {
                    FOcean.instance.needDepthBehaviour.Remove(this);
                }

            }
        }

        void OnDestroy()
        {
            if (_Material)
                DestroyImmediate(_Material);
            
            CancelInvoke();

            if(FOcean.instance != null)
               FOcean.instance.needDepthBehaviour.Remove(this);
        }
        
        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (material == null || ValueNoiseTable == null || FOcean.instance == null || !isEnable())
            {
                FOcean.BlitDontClear(source, destination, null);
                return;
            }

            if (m_Camera == null)
                m_Camera = GetComponent<Camera>();

            material.SetTexture("_ValueNoise", ValueNoiseTable);
            material.SetFloat("_Thickness", Thickness);
            material.SetColor("_LightColor", Sun.color);
            material.SetColor("_BaseColor", BaseColor);
            material.SetColor("_ScatterColor", ScatterColor);

            if (Sun != null)
            {
                material.SetVector("_FoSunDir", -Sun.gameObject.transform.forward * BaseColor.a);
            }
            else
            {
                material.SetVector("_FoSunDir", -Vector3.up * BaseColor.a);
            }

            material.SetFloat("_MinHeight", MinHeight);
            material.SetFloat("_MaxHeight", MaxHeight);
            float stime = Time.time * WindSpeed;
            float winda = WindAngle* Mathf.Deg2Rad + stime;
            float windt = Mathf.Cos(stime);
            material.SetVector("_FadeScaleWind", new Vector4(1f / FadeDist, Scale, windt * Mathf.Cos(winda), windt * Mathf.Sin(winda)));

            material.SetMatrix("_FrustumCornersWS", GetFrustumCorners(m_Camera));

            if (eCLQuality.eCL_High == quality)
            {
                material.EnableKeyword("MAX_ITERATORS_SCATTER_ON");
                material.DisableKeyword("MAX_ITERATORS_ON");
            }
            else if (eCLQuality.eCL_Medium == quality)
            {
                material.EnableKeyword("MAX_ITERATORS_ON");
                material.DisableKeyword("MAX_ITERATORS_SCATTER_ON");
            }
            else
            {
                material.DisableKeyword("MAX_ITERATORS_ON");
                material.DisableKeyword("MAX_ITERATORS_SCATTER_ON");
            }

            FOcean.CustomGraphicsBlit(source, destination, material, 0);
        }

        private Matrix4x4 GetFrustumCorners(Camera cam)
        {
            Transform camtr = cam.transform;
            float camNear = cam.nearClipPlane;
            float camFar = cam.farClipPlane;
            float camFov = cam.fieldOfView;
            float camAspect = cam.aspect;

            Matrix4x4 frustumCorners = Matrix4x4.identity;

            float fovWHalf = camFov * 0.5f;

            Vector3 toRight = camtr.right * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            Vector3 toTop = camtr.up * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
            float camScale = topLeft.magnitude * camFar / camNear;

            topLeft.Normalize();
            topLeft *= camScale;

            Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
            topRight.Normalize();
            topRight *= camScale;

            Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= camScale;

            Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= camScale;

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            return frustumCorners;
        }

    }

}
