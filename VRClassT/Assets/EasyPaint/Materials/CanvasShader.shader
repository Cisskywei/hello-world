// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CanvasShader" {
	Properties{
			initialValue ("initialValue", vector) = (0.0, 0.0, 0.0, 0.0)
	}
	SubShader {
		Pass{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			float4 initialValue;
						
			struct appdata{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f{
				float4 pos : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v){
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = float4( v.texcoord.xy, 0, 0 );
				return o;
			}

			float4 frag(v2f i) : COLOR{		
				return initialValue;
			}
			
			ENDCG
		}
	}
}
