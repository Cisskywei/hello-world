// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaUnlitColor" {	
	Properties {		
		_MainTex ("Base (Alpha8)", 2D) = "white" {}				
	}

   SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off
	Lighting Off
	ZWrite Off
    Blend One OneMinusSrcAlpha
    Pass {
     CGPROGRAM //Shader Start, Vertex Shader named vert, Fragment shader named frag
     #pragma vertex vert
     #pragma fragment frag
     #include "UnityCG.cginc"
     //Link properties to the shader
     
     sampler2D _MainTex;
     
     struct v2f 
     {
	     float4  pos : SV_POSITION;
	     float2  uv : TEXCOORD0;
	     fixed4  color : COLOR;
     };

     float4 _MainTex_ST;

     v2f vert (appdata_full v)
     {
	     v2f o;
	     o.pos = UnityObjectToClipPos (v.vertex); //Transform the vertex position
	     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex); //Prepare the vertex uv
	     o.color =  v.color;
	     return o;
     }

     half4 frag (v2f i) : COLOR
     {
         fixed4 result = tex2D (_MainTex, i.uv) * i.color; //base texture
         result.rgb *= result.a;
        
         return result;
     }

     ENDCG //Shader End
    }
   }
}

