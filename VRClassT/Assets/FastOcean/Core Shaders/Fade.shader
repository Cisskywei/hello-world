Shader "Hidden/FastOcean/Fade" {
	SubShader {
		Pass {

		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM
		
		#include "UnityCG.cginc"
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma target 2.0
		#pragma fragmentoption ARB_precision_hint_fastest

        sampler2D u_Input;
		float u_FoFade;	
		//float up;
		//float down;
					
		fixed4 frag (v2f_img i) : SV_Target {
			
			float2 offset = i.uv - 0.5;
			float a = exp2(-dot(offset, offset) * u_FoFade);
			fixed4 c = tex2D(u_Input,i.uv) * saturate(a);

			//if(i.uv.x>=up || i.uv.x<=down || i.uv.y>=up || i.uv.y<=down)
			//   c = 0;

			return Luminance(c);
		}
		ENDCG
		}
	} 
	Fallback off
}
