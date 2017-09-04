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
	public enum GlareEffectResolution
	{
	    Low,
	    Normal,
	    High,
	}

	public class FGlareEffect : FPostEffectsBase 
	{
        public bool enableGlare = true;

        [Range(0, 2)]
	    public float attenuation = 0.95f;
        [Range(0, 1)]
	    public float intensity = 0.25f;
        [Range(0, 1)]
	    public float cutoff = 1.0f;

	    public GlareEffectResolution resolution = GlareEffectResolution.Normal;

		public Shader glareBlendShader;
	    private Material glareBlendMaterial;
		
		public Shader combineShader;
		private Material combineMaterial;
		
		protected override  bool CheckResources () {
			CheckSupport ();

	        glareBlendMaterial = CheckShaderAndCreateMaterial(glareBlendShader, glareBlendMaterial);
	        combineMaterial = CheckShaderAndCreateMaterial(combineShader, combineMaterial);
			
			if(!isSupported)
				ReportAutoDisable ();
			return isSupported;
		}

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
                InvokeRepeating("CheckEnable", 0f, 0.001f);
        }

        protected override void CheckEnable()
        {
            if (FOcean.instance != null)
            {
                bool glareChanged = false;

                //avoid GC Alloc when understate changed
                bool bNeedIndeed = enableGlare && !FOcean.instance.mobile && FOcean.instance.supportSM3 &&
                    FOcean.instance.envParam.sunMode != eFSunMode.eSM_None;

                enabled = bNeedIndeed && !FOcean.instance.IntersectWater();

                if (enabled)
                {
                    if (!FOcean.instance.needDepthBehaviour.Contains(this))
                        FOcean.instance.needDepthBehaviour.Add(this);

                    if (!FOcean.instance.needGlareBehaviour.Contains(this))
                    {
                        FOcean.instance.needGlareBehaviour.Add(this);
                        glareChanged = true;
                    }
                }
                else
                {
                    FOcean.instance.needDepthBehaviour.Remove(this);
                    if (FOcean.instance.needGlareBehaviour.Contains(this) && !bNeedIndeed)
                    {
                        FOcean.instance.needGlareBehaviour.Remove(this);
                        glareChanged = true;
                    }
                }

                if(glareChanged)
                {
                    FOcean.instance.ForceReload(false);
                    glareChanged = false;
                }
            }
        }

        void OnDestroy()
        {
            if (glareBlendMaterial != null)
                DestroyImmediate(glareBlendMaterial);

            if (combineMaterial != null)
                DestroyImmediate(combineMaterial);

            CancelInvoke();

            if (FOcean.instance != null)
            {
                FOcean.instance.needDepthBehaviour.Remove(this);
                FOcean.instance.needGlareBehaviour.Remove(this);
                FOcean.instance.ForceReload(false);
            }
        }

        void OnRenderImage (RenderTexture source, RenderTexture destination) {
	        if (glareBlendMaterial == null || CheckResources() == false || FOcean.instance == null || 
                !FOcean.instance.gameObject.activeSelf || FOcean.instance.envParam.sunLight == null)
	        {
                FOcean.BlitDontClear(source, destination, null);
				return;
			}

	        int divider = 4;
	        if (resolution == GlareEffectResolution.Normal)
	            divider = 2;
	        else if (resolution == GlareEffectResolution.High)
	            divider = 1;


	        RenderTextureFormat rtFormat = FOcean.instance.rtR8Format;
	        float fDownRes = 1.0f / (float)divider;
	        int rtW4 = (int)(source.width * fDownRes);
	        int rtH4 = (int)(source.height * fDownRes);

            glareBlendMaterial.SetTexture("_MainTex", FOcean.instance.glaremap);
	        glareBlendMaterial.SetTexture("_OceanMap", FOcean.instance.oceanmap);
	        glareBlendMaterial.SetFloat("_Attenuation", attenuation);
	        glareBlendMaterial.SetFloat("_CutOff", cutoff);
	        glareBlendMaterial.SetVector("_TexelSize", new Vector4(1f / source.width, 1f / source.height, fDownRes / source.width, fDownRes / source.height));

			// Downsample
			RenderTexture quarterRezColor = RenderTexture.GetTemporary (rtW4, rtH4, 0, rtFormat);
            FOcean.Blit(null, quarterRezColor, glareBlendMaterial, 0);

	        RenderTexture streakBuffer1 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
	        RenderTexture streakBuffer2 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
	        RenderTexture streakBuffer3 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
	        RenderTexture streakBuffer4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
	        RenderTexture rtDown4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);

	        // Streak filter top Right
	        glareBlendMaterial.SetTexture("_MainTex", quarterRezColor);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(0.5f, 0.5f, 1.0f, 0.0f));
            FOcean.Blit(null, rtDown4, glareBlendMaterial, 1);

	        glareBlendMaterial.SetTexture("_MainTex", rtDown4);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(0.5f, 0.5f, 2.0f, 0.0f));
            FOcean.Blit(null, streakBuffer1, glareBlendMaterial, 1);

	        // Streak filter Bottom left
	        glareBlendMaterial.SetTexture("_MainTex", quarterRezColor);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(-0.5f, -0.5f, 1.0f, 0.0f));
            FOcean.Blit(null, rtDown4, glareBlendMaterial, 1);

	        glareBlendMaterial.SetTexture("_MainTex", rtDown4);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(- 0.5f, -0.5f, 2.0f, 0.0f));
            FOcean.Blit(null, streakBuffer2, glareBlendMaterial, 1);

	        // Streak filter Bottom right
	        glareBlendMaterial.SetTexture("_MainTex", quarterRezColor);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(0.5f, -0.5f, 1.0f, 0.0f));
            FOcean.Blit(null, rtDown4, glareBlendMaterial, 1);

	        glareBlendMaterial.SetTexture("_MainTex", rtDown4);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(0.5f, -0.5f, 2.0f, 0.0f));
            FOcean.Blit(null, streakBuffer3, glareBlendMaterial, 1);

	        // Streak filter Top Left
	        glareBlendMaterial.SetTexture("_MainTex", quarterRezColor);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(-0.5f, 0.5f, 1.0f, 0.0f));
            FOcean.Blit(null, rtDown4, glareBlendMaterial, 1);

	        glareBlendMaterial.SetTexture("_MainTex", rtDown4);
	        glareBlendMaterial.SetVector("_Direction", new Vector4(-0.5f, 0.5f, 2.0f, 0.0f));
            FOcean.Blit(null, streakBuffer4, glareBlendMaterial, 1);

	        //combine
	        combineMaterial.SetTexture("_MainTex", source);
	        combineMaterial.SetTexture("_StreakBuffer1", streakBuffer1);
	        combineMaterial.SetTexture("_StreakBuffer2", streakBuffer2);
	        combineMaterial.SetTexture("_StreakBuffer3", streakBuffer3);
	        combineMaterial.SetTexture("_StreakBuffer4", streakBuffer4);
	        combineMaterial.SetFloat("_Intensity", intensity);
            Color sunColor = FOcean.instance.envParam.sunLight.color;
            combineMaterial.SetColor("_FoSunColor", sunColor);

            FOcean.BlitDontClear(null, destination, combineMaterial);
	        
	        RenderTexture.ReleaseTemporary(rtDown4);
			RenderTexture.ReleaseTemporary(quarterRezColor);
	        RenderTexture.ReleaseTemporary(streakBuffer1);
	        RenderTexture.ReleaseTemporary(streakBuffer2);
	        RenderTexture.ReleaseTemporary(streakBuffer3);
	        RenderTexture.ReleaseTemporary(streakBuffer4);
		}
	}
}
