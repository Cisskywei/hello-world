// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitVertexColor" {	
	Properties {						
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
     
     
     struct v2f 
     {
	     float4  pos : SV_POSITION;
	     fixed4  color : COLOR;
     };

     float4 _MainTex_ST;

     v2f vert (appdata_full v)
     {
	     v2f o;
	     o.pos = UnityObjectToClipPos (v.vertex); //Transform the vertex position
	     o.color =  v.color;
	     return o;
     }

     half4 frag (v2f i) : COLOR
     {
         return i.color;
     }

     ENDCG //Shader End
    }
   }
}

