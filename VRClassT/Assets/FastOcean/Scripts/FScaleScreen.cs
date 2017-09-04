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
    [ExecuteInEditMode]
    public class FScaleScreen : FPostEffectsBase
    {
        [Range(0,2)]
        public int scale = 1;

        void OnEnable()
        {
            if (rt == null || rt.width != (Screen.width >> scale) || rt.height != (Screen.height >> scale))
            {
                Object.DestroyImmediate(rt);
                rt = new RenderTexture(Screen.width >> scale, Screen.height >> scale, 16);
            }

            // Misc
            m_Camera = GetComponent<Camera>();

            m_Camera.enabled = true;
            //See "Performance Tunning for Tile-Based Architecture"
            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.depthTextureMode = DepthTextureMode.None;

            Camera.main.targetTexture = rt;

            FOcean.target = rt;
        }

        protected override bool CheckResources()
        {
            return rt != null;
        }

        RenderTexture rt = null;

        void Update()
        {
            if (Camera.main == null)
                return;

            if (FOcean.instance == null)
                return;

            if ((FOcean.instance.mobile || !FOcean.instance.supportSM3) && scale == 0)
                scale = 1;

            if (scale == 0)
            {
                m_Camera.enabled = false;
                Camera.main.cullingMask = 0x7FFFFFFF;
                Camera.main.cullingMask &= ~(1 << FOcean.instance.layerDef.traillayer);
                Camera.main.targetTexture = null;

                if (rt != null)
                {
                    Object.DestroyImmediate(rt);
                    rt = null;
                }

                FOcean.instance.targetResWidth = -1;
                FOcean.instance.targetResHeight = -1;
                
                FOcean.target = null;
                return;
            }


            m_Camera.enabled = true;
            m_Camera.cullingMask = 1 << FOcean.instance.layerDef.uilayer;

#if UNITY_EDITOR
            m_Camera.transform.position = Vector3.zero;
            m_Camera.transform.rotation = Quaternion.identity;
            m_Camera.transform.localScale = Vector3.zero;
#endif

            if (rt == null || rt.width != (Screen.width >> scale) || rt.height != (Screen.height >> scale))
            {
                Camera.main.targetTexture = null;
                Object.DestroyImmediate(rt);
                rt = new RenderTexture(Screen.width >> scale, Screen.height >> scale, 16);
            }

            FOcean.instance.targetResWidth = Screen.width >> scale;
            FOcean.instance.targetResHeight = Screen.height >> scale;

            FOcean.target = rt;

            Camera.main.targetTexture = rt;
            Camera.main.cullingMask = ~(1 << FOcean.instance.layerDef.uilayer);
            Camera.main.cullingMask &= ~(1 << FOcean.instance.layerDef.traillayer);
        }

        void OnDisable()
        {
            GetComponent<Camera>().enabled = false;

            if (Camera.main != null)
            {
                Camera.main.cullingMask = 0x7FFFFFFF;
                Camera.main.targetTexture = null;
            }

            if(rt != null)
                Object.DestroyImmediate(rt);

            if (FOcean.instance != null)
            {
                FOcean.instance.targetResWidth = -1;
                FOcean.instance.targetResHeight = -1;
            }

            FOcean.target = null;

            rt = null;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            //ugly hack
            FOcean.target = null;
            FOcean.BlitDontClear(rt, destination, null);
            FOcean.target = rt;
        }

    }
}
