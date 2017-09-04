using UnityEngine;
using System.Collections;

public class GlitterCameraEffect : MonoBehaviour {
	[Range(0,2)]
	public int DownSample = 1;
	public Shader GlitterPostprocessShader = null;
	public Shader MixGlittersPostprocessShader = null;
	

	Material m_GlitterMaterial = null;
	protected Material glitterMaterial {
		get {
			if (m_GlitterMaterial == null) {
				m_GlitterMaterial = new Material(GlitterPostprocessShader);
				m_GlitterMaterial.hideFlags = HideFlags.DontSave;
			}
			return m_GlitterMaterial;
		}
	}


	Material m_MixMaterial = null;
	protected Material mixMaterial {
		get {
			if (m_MixMaterial == null) {
				m_MixMaterial = new Material(MixGlittersPostprocessShader);
				m_MixMaterial.hideFlags = HideFlags.DontSave;
			}
			return m_MixMaterial;
		}
	}
	
	protected void OnDisable() {
		if ( m_GlitterMaterial ) {
			DestroyImmediate( m_GlitterMaterial );
		}
		if (m_MixMaterial){
			DestroyImmediate( m_MixMaterial );
		}
	}


	protected void Start()
	{
		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects) {
			enabled = false;
			return;
		}
		// Disable if the shader can't run on the users graphics card
		if (!GlitterPostprocessShader || !glitterMaterial.shader.isSupported || !mixMaterial.shader.isSupported) {
			enabled = false;
			return;
		}

	}


	RenderTexture glitterMaskRT;


	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{

		//downsample
		int rtW = source.width >> DownSample;
		int rtH = source.height >> DownSample;
		//float widthMod = 1.0f / (1.0f * (1<<DownSample));

		RenderTexture	glitterMaskRT = RenderTexture.GetTemporary (rtW, rtH, 0, 
                                              SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf) 
                                              ? RenderTextureFormat.R8
                                              : RenderTextureFormat.ARGB32);
		glitterMaskRT.filterMode = FilterMode.Bilinear;

		mixMaterial.SetTexture("_GlitterMaskTex", glitterMaskRT);


		// make stars
		Graphics.Blit (source, glitterMaskRT, glitterMaterial);
		Graphics.Blit (source, destination, mixMaterial);

		RenderTexture.ReleaseTemporary(glitterMaskRT);

	}

}
