
Shader "FastOcean/FOceanParallax" { 
Properties {

	[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend Mode", Float) = 1
	[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend Mode", Float) = 0

	[HideInInspector][Enum(UnityEngine.Rendering.CullMode)] _CullAbove ("Cull Above", Float) = 0
	[HideInInspector][Enum(UnityEngine.Rendering.CullMode)] _CullUnder ("Cull Under", Float) = 0
			
	[Header(Colors)]
	[Space]
	_FoBaseColor ("Base Color", COLOR)  = (0, .49, .78, 1)	
	_FoDeepColor("Deep Color", COLOR) = (.12, .43, .27, 0.7)
	
	[Space]
	_ExtinctScale ("Extinct Scale", Float) = 10.0
	_ExtinctColor ("Extinct Color", COLOR) = (0.05, 0.84, 0.15, 1)
		
	[Header(Normals)]
	[Space(10)]
	[NoScaleOffset] _OceanNM ("Normal Map", 2D) = "bump" {}		

	[Space]
	_BumpTiling ("Bump Tiling & Speed", Vector) = (0.08 ,0.42, 0.1, 0.05)
	_DistortParams("Distortions(Trails, Reflection, Bump, Sharp)", Vector) = (0.01 ,0.05, 1.0, 3)

	_nSnell("Snell", Range(1.01, 3)) = 2.5
	_FresnelScale ("Fresnel Scale", Range (0.15, 4.0)) = 1	
	_FoFade ("Fade", Range (0.01, 0.1)) = 0.08

	[Space]
	_FoReflAmb2Scene("Ambient Factor", Range(0.1, 1)) = 0.86
	_FoShininess ("Shininess", Range (2.0, 500.0)) = 436.0	

	[Header(Parallax)]
	[Space]
	[NoScaleOffset]_ParallaxMap ("Parallax Map (A)", 2D) = "black" {}
	_FoParallax1("Parallax Offset 1", Range (0.01, 0.3)) = 0.15
	_FoParallax2("Parallax Offset 2", Range (0.01, 0.3)) = 0.15
	
	[Header(Transparents)]
	[Space(10)]
	_FoShallowColor ("Transparents Color", COLOR)  = ( .10, .4, .43, 1)
	_AboveDepth("Above Depth", Range(0.1, 10)) = 5
	_UnderDepth("Under Depth", Range(0.1, 1)) = 1
	_ShallowEdge("Shallow Edge", Range(0.01, 1)) = 1
		
	[Header(Foams)]
	[Space(10)]
	_Foam ("Foam(Peak, Intensity, Edge, Distort)", Vector) = (0.9, 0.1, 0.3, 0.01)
	_FoamTiling ("Foam Tiling & Speed", Vector) = (0.1 ,0.1, 0.01, 0.01)
	_FoamBSpeed("Foam Blend Speed", Range(0.01, 1)) = 0.2
		
	[Space]
	[NoScaleOffset] _FoamTex ("Foam Texture ", 2D) = "black" {}

	[Space]
	_FoamMaskScale("Foam Mask Scale", Range(0.01, 1)) = 0.1
	[NoScaleOffset] _FoamMask ("Foam Mask ", 2D) = "black" {}

	[Space]
	_FoamGSpeed("Foam Gradient Speed", Range(0.01, 1)) = 0.05
	_FoamGScale("Foam Gradient Scale", Range(0.01, 1)) = 0.03
	[NoScaleOffset] _FoamGradient ("Foam Gradient ", 2D) = "black" {}

} 

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 204
	
	Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZTest LEqual
			ZWrite On
			Cull [_CullAbove]
			Fog { Mode off }

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex vert_FO
			#pragma fragment frag_FO

			#pragma multi_compile_fog

			#pragma exclude_renderers gles
			
			//#pragma glsl
			
			#pragma fragmentoption ARB_precision_hint_fastest			
			#pragma multi_compile __ FO_PROJECTED_ON
			#pragma multi_compile __ FO_GERSTNER_ON	
			#pragma multi_compile __ FO_FOAM_ON
			#pragma multi_compile __ FO_PHONG_ON FO_BLINNPHONG_ON
			#pragma multi_compile __ FO_TRAIL_ON FO_TRAILSCREEN_ON
			#pragma multi_compile __ FO_DEPTHBLEND_ON	

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 203
	
	Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZTest LEqual
			ZWrite On
			Cull Back
			Fog { Mode off }

			CGPROGRAM

			#pragma target 2.0

			#pragma vertex vert_FO
			#pragma fragment frag_FO

			#pragma multi_compile_fog

			#pragma only_renderers gles gles3 d3d11
			
			//#pragma glsl
			
			#pragma fragmentoption ARB_precision_hint_fastest			
			#pragma multi_compile __ FO_PROJECTED_ON
			#pragma multi_compile __ FO_FOAM_ON
			#pragma multi_compile __ FO_PHONG_ON FO_BLINNPHONG_ON
			#pragma multi_compile __ FO_TRAIL_ON FO_TRAILSCREEN_ON
			
			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"	

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 202
	
	Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZTest LEqual
			ZWrite On
			Cull [_CullUnder]
			Fog { Mode off }
			
			CGPROGRAM

			#pragma target 3.0

			//To Show Skirt FOR Debug
			//#define FO_SKIRT

			#pragma vertex vert_UFO
			#pragma fragment frag_UFO

			#pragma exclude_renderers gles
			
			#pragma fragmentoption ARB_precision_hint_fastest			
			#pragma multi_compile __ FO_PROJECTED_ON	
			#pragma multi_compile __ FO_GERSTNER_ON	
			#pragma multi_compile __ FO_TRAIL_ON FO_TRAILSCREEN_ON
			#pragma multi_compile __ FO_DEPTHBLEND_ON
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 201
	
	Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZTest LEqual
			ZWrite On
			Cull Front
			Fog { Mode off }
			
			CGPROGRAM

			#pragma target 2.0

			#pragma vertex vert_UFO
			#pragma fragment frag_UFO

			#pragma only_renderers gles gles3 d3d11
			
			#pragma fragmentoption ARB_precision_hint_fastest			
			#pragma multi_compile __ FO_PROJECTED_ON	
			#pragma multi_compile __ FO_TRAIL_ON FO_TRAILSCREEN_ON
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

// for editor
Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 200
	
	Pass {
			Blend [_SrcBlend] [_DstBlend]
			ZTest LEqual
			ZWrite On
			Cull Off
			Fog { Mode off }

			CGPROGRAM
			
			#pragma target 3.0

			#pragma vertex vert_FO
			#pragma fragment frag_FO
			
			#pragma multi_compile_fog

			#pragma only_renderers d3d9 d3d11 glcore d3d11_9x

			//#pragma glsl
			
			#pragma fragmentoption ARB_precision_hint_fastest			
			#pragma shader_feature FO_PROJECTED_ON
			#pragma shader_feature FO_GERSTNER_ON
			#pragma shader_feature FO_FOAM_ON
			#pragma shader_feature FO_PHONG_ON FO_BLINNPHONG_ON
			#pragma shader_feature FO_EDITING
			#pragma shader_feature FO_DEPTHBLEND_ON	

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 199
	
	Pass {
			//ZTest LEqual Cull Back ZWrite On
			ZTest Off Cull Back ZWrite Off // Better Performance
			Fog { Mode off }  

			CGPROGRAM
			
			#pragma target 3.0

			#pragma exclude_renderers gles gles3

			#pragma vertex vert_FO
			#pragma fragment frag_OCEAN_MAP
			
			#pragma fragmentoption ARB_precision_hint_fastest				
			#pragma multi_compile __ FO_PROJECTED_ON
			#pragma multi_compile __ FO_GERSTNER_ON	

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 198
	
	Pass {
			ZTest LEqual Cull Off ZWrite On
			Fog { Mode off }  

			CGPROGRAM

			#pragma target 3.0
			
			#pragma vertex vert_UFO
			#pragma fragment frag_UOCEAN_MAP
			
			#pragma exclude_renderers gles gles3
			
			#pragma fragmentoption ARB_precision_hint_fastest				
			#pragma multi_compile __ FO_PROJECTED_ON
			#pragma multi_compile __ FO_GERSTNER_ON	

			#define FO_SKIRT

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 197
	
	Pass {
			ZTest LEqual Cull Front ZWrite On
			Fog { Mode off }  

			CGPROGRAM

			#pragma target 3.0
			
			#pragma vertex vert_UFO
			#pragma fragment frag_UOCEAN_MAP
			
			#pragma exclude_renderers gles gles3
			
			#pragma fragmentoption ARB_precision_hint_fastest				
			#pragma multi_compile __ FO_PROJECTED_ON
			#pragma multi_compile __ FO_GERSTNER_ON

			#define FO_SKIRT

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}

	Pass {
			ZTest LEqual Cull Back ZWrite On
			Fog { Mode off }  

			CGPROGRAM

			#pragma target 3.0
			
			#pragma vertex vert_FO
			#pragma fragment frag_UOCEAN_MAP2
			
			#pragma exclude_renderers gles gles3
			
			#pragma fragmentoption ARB_precision_hint_fastest				
			#pragma multi_compile __ FO_PROJECTED_ON
			#pragma multi_compile __ FO_GERSTNER_ON	

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Subshader 
{ 
	Tags {"RenderType"="Transparent" "Queue"="Transparent-1"}
	
	Lod 196
	
	Pass {
			ZTest Off Cull Back ZWrite Off
			Fog { Mode off }  

			CGPROGRAM

			#pragma target 3.0
			
			#pragma vertex vert_FO
			#pragma fragment frag_GLARE_MAP
			
			#pragma exclude_renderers gles gles3

			#pragma multi_compile_fog
			
			#pragma fragmentoption ARB_precision_hint_fastest				
			#pragma multi_compile __ FO_PROJECTED_ON	
			#pragma multi_compile __ FO_GERSTNER_ON
			#pragma multi_compile __ FO_PHONG_ON

			#include "UnityCG.cginc"
			#include "FOceanCore.cginc"
			#include "FOceanInclude.cginc"

			ENDCG
	}
}

Fallback off
}
