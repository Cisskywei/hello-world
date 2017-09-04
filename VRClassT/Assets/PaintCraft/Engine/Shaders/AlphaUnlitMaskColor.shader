// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaUnlitMaskColor" {	
	Properties {		
		_MainTex ("Base", 2D) = "white" {}	
		_MaskTex ("Mask (Alpha8)", 2D) = "white" {}		
		_OriginX ("First click uv.x", Range(0,1)) = 0.5
		_OriginY ("First click uv.y", Range(0,1)) = 0.5		
	}

   SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Cull Off
	Lighting Off
	ZWrite Off
    Blend One OneMinusSrcAlpha
    Pass {
     CGPROGRAM //Shader Start, Vertex Shader named vert, Fragment shader named frag
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members originMaskColor)
#pragma exclude_renderers d3d11 xbox360


     #pragma vertex vert
     #pragma fragment frag
     #include "UnityCG.cginc"
     //Link properties to the shader
     
     sampler2D 	_MainTex;
     sampler2D 	_MaskTex;
     float		_OriginX;
     float 		_OriginY;
     
     struct v2f 
     {
	     float4  pos : SV_POSITION;
	     float2  uv : TEXCOORD0;
	     float2  uv2 : TEXCOORD1;
	     fixed4  color : COLOR;
	     float4  originMaskColor : TANGENT;
     };

     float4 _MainTex_ST;
     float4 _MaskTex_ST;

     v2f vert (appdata_full v)
     {
	     v2f o;
	     o.pos = UnityObjectToClipPos (v.vertex); //Transform the vertex position
	     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex); //Prepare the vertex uv
	     o.uv2 = TRANSFORM_TEX (v.texcoord1, _MaskTex);
	     
	     o.color =  v.color;
	     o.originMaskColor = tex2D(_MaskTex, float2(_OriginX, _OriginY));
	     return o;
     }

     half4 frag (v2f i) : COLOR
     {
         fixed4 result = tex2D (_MainTex, i.uv) * i.color; //base texture
         result.rgb *= result.a;
         fixed4 mask = tex2D(_MaskTex, i.uv);
         result *=  (i.originMaskColor.a == mask.a);        
         return result;
     }

     ENDCG //Shader End
    }
   }
}

