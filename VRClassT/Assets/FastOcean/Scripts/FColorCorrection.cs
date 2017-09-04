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
    public class FColorCorrection : FPostEffectsBase
	{
		[Range(0.1f,2f)]
        public float saturation = 1.0f;
		
        public AnimationCurve redChannel = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
        public AnimationCurve greenChannel = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
        public AnimationCurve blueChannel = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
		
        private Material ccMaterial;

		private Texture2D rgbChannelTex;
        public Shader simpleColorCorrectionCurvesShader = null;

        private bool  updateTexturesOnStartup = true;

        protected override void Start ()
		{
            base.Start ();
            updateTexturesOnStartup = true;
        }

        void Awake () {	}

        void OnDestroy()
        {
            if(rgbChannelTex != null)
               DestroyImmediate(rgbChannelTex);
        }

        protected override bool CheckResources ()
		{

            ccMaterial = CheckShaderAndCreateMaterial (simpleColorCorrectionCurvesShader, ccMaterial);

            if (!rgbChannelTex)
                rgbChannelTex = new Texture2D (256, 4, TextureFormat.ARGB32, false, true);
#if UNITY_EDITOR
            rgbChannelTex.hideFlags = HideFlags.DontSave;
#endif
            rgbChannelTex.wrapMode = TextureWrapMode.Clamp;

            if (!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }

        public void UpdateParameters ()
		{
            CheckResources(); // textures might not be created if we're tweaking UI while disabled

            if (redChannel != null && greenChannel != null && blueChannel != null)
			{
                for (float i = 0.0f; i <= 1.0f; i += 1.0f / 255.0f)
				{
                    float rCh = Mathf.Clamp (redChannel.Evaluate(i), 0.0f, 1.0f);
                    float gCh = Mathf.Clamp (greenChannel.Evaluate(i), 0.0f, 1.0f);
                    float bCh = Mathf.Clamp (blueChannel.Evaluate(i), 0.0f, 1.0f);

                    rgbChannelTex.SetPixel ((int) Mathf.Floor(i*255.0f), 0, new Color(rCh,rCh,rCh) );
                    rgbChannelTex.SetPixel ((int) Mathf.Floor(i*255.0f), 1, new Color(gCh,gCh,gCh) );
                    rgbChannelTex.SetPixel ((int) Mathf.Floor(i*255.0f), 2, new Color(bCh,bCh,bCh) );
                }

                rgbChannelTex.Apply ();
            }
        }

        void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
            if (CheckResources()==false)
			{
                FOcean.BlitDontClear(source, destination, null);
                return;
            }
            

            if (updateTexturesOnStartup)
			{
                UpdateParameters ();
                updateTexturesOnStartup = false;
            }
            
			ccMaterial.SetTexture ("_RgbTex", rgbChannelTex);
			ccMaterial.SetFloat ("_Saturation", saturation);

            FOcean.BlitDontClear(source, destination, ccMaterial);

        }
    }
}
