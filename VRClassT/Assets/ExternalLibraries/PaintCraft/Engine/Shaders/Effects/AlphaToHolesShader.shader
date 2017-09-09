// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaToHoles"
{

//TODO: set several shader pass  and then use this https://machinesdontcare.wordpress.com/2009/05/26/nvidia-post-star-filter/
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}		
		_NoiseTex2 ("Noise Texture", 2D) = "white" {}
		
		_Min ("Min ", Float) = 2.5
		_Max ("Max", Float) = 2.51
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off
		Lighting Off
		ZWrite Off
	    Blend One Zero
//		Blend One OneMinusSrcAlpha //Use this to debug holes

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;	
				float2 uv2: TEXCOORD1;	
				float2 uv3: TEXCOORD2;	
				float4 vertex : SV_POSITION;
			};
			
			float _Min;
			float _Max;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
				
			sampler2D _NoiseTex2;
			float4 _NoiseTex2_ST;
									
																														
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv  = TRANSFORM_TEX(v.uv, _MainTex);	
				o.uv2 = TRANSFORM_TEX(v.uv, _NoiseTex) ;	
				o.uv3 = TRANSFORM_TEX(v.uv, _NoiseTex2);	
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				//animate
				float p1 = tex2D(_NoiseTex, i.uv2 + float2(_Time.y* 0.02, _Time.y*0.001)).a;
				float p2 = tex2D(_NoiseTex2, i.uv3 + float2(_Time.y * -0.01, _Time.y * -0.0012)).a;
				
				//static
//				float p1 = tex2D(_NoiseTex, i.uv2).a;
//				float p2 = tex2D(_NoiseTex, i.uv3).a;
				
				float n = smoothstep(_Min, _Max, p1+p2);
				
				col.a = max (col.a, 1 - n);
				return col;
			}
			ENDCG
		}
	}
}
