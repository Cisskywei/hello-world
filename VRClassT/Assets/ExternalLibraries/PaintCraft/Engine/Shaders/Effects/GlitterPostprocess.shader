// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GlitterPostprocess"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Off
		Lighting Off
		
		Blend One Zero
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			uniform half4 _MainTex_TexelSize;
			sampler2D _MainTex;
			
			
			static const float wt0 = 1.0;
			static const float wt1 = 0.8;
			static const float wt2 = 0.6;
			static const float wt3 = 0.4;
			static const float wt4 = 0.2;
			static const float wtNorm = (wt0 + 2.0 * (wt1 + wt2 + wt3 + wt4));
			static const half  hRad = 1.5708;
			static const half  rotationSpeed = 0.1;
			
			struct v2f
			{
				fixed2 uv : TEXCOORD0;				
				float4 vertex : SV_POSITION;
				half4 offs[8] : TEXCOORD1;
			};

			v2f vert (appdata_img v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv = v.texcoord;
				
				half s = sin(_Time.y * rotationSpeed);
				half c = cos(_Time.y * rotationSpeed);
				
				half s2 = sin(_Time.y * rotationSpeed + hRad);
				half c2 = cos(_Time.y * rotationSpeed + hRad);
			
				//NorthEast
				half2 netFilterWidth = _MainTex_TexelSize.xy * half2(s,c); 
				half4 coords = netFilterWidth.xyxy;
				
				o.offs[0] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				coords += netFilterWidth.xyxy;
				o.offs[1] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				coords += netFilterWidth.xyxy;
				o.offs[2] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				coords += netFilterWidth.xyxy;
				o.offs[3] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				
				//NorthWest
				netFilterWidth = _MainTex_TexelSize.xy * half2(s2,c2);
				coords = netFilterWidth.xyxy; 
				
				o.offs[4] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				coords += netFilterWidth.xyxy;
				o.offs[5] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				coords += netFilterWidth.xyxy;
				o.offs[6] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				coords += netFilterWidth.xyxy;
				o.offs[7] = v.texcoord.xyxy + coords * half4(1.0h,1.0h,-1.0h,-1.0h);
				
				return o;
			}
			
			




			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				half a = col.a;
				a+= tex2D(_MainTex, i.offs[0].xy).a * wt1;
				a+= tex2D(_MainTex, i.offs[0].zw).a * wt1;
				
				a+= tex2D(_MainTex, i.offs[1].xy).a * wt2;
				a+= tex2D(_MainTex, i.offs[1].zw).a * wt2;
				
				a+= tex2D(_MainTex, i.offs[2].xy).a * wt3;
				a+= tex2D(_MainTex, i.offs[2].zw).a * wt3;
				
				a+= tex2D(_MainTex, i.offs[3].xy).a * wt4;
				a+= tex2D(_MainTex, i.offs[3].zw).a * wt4;
				
				a+= tex2D(_MainTex, i.offs[4].xy).a * wt1;
				a+= tex2D(_MainTex, i.offs[4].zw).a * wt1;
				
				a+= tex2D(_MainTex, i.offs[5].xy).a * wt2;
				a+= tex2D(_MainTex, i.offs[5].zw).a * wt2;
				
				a+= tex2D(_MainTex, i.offs[6].xy).a * wt3;
				a+= tex2D(_MainTex, i.offs[6].zw).a * wt3;
				
				a+= tex2D(_MainTex, i.offs[7].xy).a * wt4;
				a+= tex2D(_MainTex, i.offs[7].zw).a * wt4;				
				
				a /= wtNorm;
				half mask = smoothstep (0, 1.8, a); // 1.8 = 9/5 = sum all uvNE-NW in case of all alpha = 1			
				//col.rgb += fixed3(4,4,4) * (1-mask);
				 
				
				col.rgba = mask;
				return col;
			}
			ENDCG
		}
	}
}
