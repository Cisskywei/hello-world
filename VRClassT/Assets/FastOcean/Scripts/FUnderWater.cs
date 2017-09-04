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
	public class FUnderWater : FPostEffectsBase
	{
		private Material UnderwaterMaterial;

	    private Vector2 rnd1;
        private Vector2 rnd2;

		// Use this for initialization
		protected override void Start () {
	        rnd1 = Random.insideUnitCircle;
            rnd2 = Random.insideUnitCircle;
		    CheckResources();
		}

	    protected override bool CheckResources()
	    {
            if (FOcean.instance == null)
                return false;

	        UnderwaterMaterial = CheckShaderAndCreateMaterial(FOcean.instance.envParam.underWaterShader, UnderwaterMaterial);

	        if (!isSupported)
	            ReportAutoDisable();
	        return isSupported;
	    }

        void OnDestroy()
        {
            if (UnderwaterMaterial != null)
                DestroyImmediate(UnderwaterMaterial);
        }
		
		// Update is called once per frame
		void OnRenderImage(RenderTexture source, RenderTexture destination) 
		{
	        if (UnderwaterMaterial == null || CheckResources() == false || null == FOcean.instance)
	        {
                FOcean.BlitDontClear(source, destination, null);
	            return;
	        }

            //camera.depthTextureMode |= DepthTextureMode.Depth;

            //downsample the frame to flood level
            RenderTexture rto = FOcean.instance.oceanmap;
	        RenderTexture blur = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default);

            UnderwaterMaterial.SetTexture("_MainTex", source);
	        UnderwaterMaterial.SetColor("_FoBaseColor", FOcean.instance.GetBaseColor());
	        UnderwaterMaterial.SetFloat("_DepthFade", FOcean.instance.envParam.depthFade);
	        UnderwaterMaterial.SetFloat("_SurFade", FOcean.instance.envParam.surfaceFade);
	        
	        UnderwaterMaterial.SetTexture("_OceanMap", rto);

            UnderwaterMaterial.SetVector("offsets", new Vector4(rnd1.x, rnd1.y, rnd2.x, rnd2.y));
            FOcean.Blit(source, blur, UnderwaterMaterial, 0);

	        UnderwaterMaterial.SetTexture("_MainTex", blur);
	        UnderwaterMaterial.SetTexture("_DistortMap", FOcean.instance.envParam.underDistortMap);
            UnderwaterMaterial.SetFloat("_DistortMag", Mathf.Clamp01(FOcean.instance.envParam.distortMag));
            UnderwaterMaterial.SetFloat("_DistortFrq", FOcean.instance.envParam.distortFrq * Time.time);

            FOcean.BlitDontClear(blur, destination, UnderwaterMaterial, 1);
	        RenderTexture.ReleaseTemporary(blur);
		}
        
    }
}
