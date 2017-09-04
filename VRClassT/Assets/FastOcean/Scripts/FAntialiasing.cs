/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using UnityEngine;

namespace FastOcean
{
    [ExecuteInEditMode]
    public class FAntialiasing : FPostEffectsBase
    {
        [Range(0, 0.1f)]
        public float edgeThresholdMin = 0.05f;
        [Range(0, 1)]
        public float edgeThreshold = 0.2f;
        [Range(0, 10f)]
        public float edgeSharpness = 4.0f;

        public Shader shaderFXAAIII;
        private Material materialFXAAIII;
        
        protected override bool CheckResources()
        {
            CheckSupport(false);
            
            materialFXAAIII = CreateMaterial(shaderFXAAIII, materialFXAAIII);

            if (!shaderFXAAIII || !shaderFXAAIII.isSupported)
            {
                NotSupported();
                ReportAutoDisable();
                return false;
            }

            return isSupported;
        }


        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                FOcean.BlitDontClear(source, destination, null);
                return;
            }

            // ----------------------------------------------------------------
            // FXAA antialiasing modes
            if (materialFXAAIII != null)
            {
                materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
                materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
                materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);

                FOcean.BlitDontClear(source, destination, materialFXAAIII);
            }
            else
            {
                // none of the AA is supported, fallback to a simple blit
                FOcean.BlitDontClear(source, destination, null);
            }
        }
    }
}
