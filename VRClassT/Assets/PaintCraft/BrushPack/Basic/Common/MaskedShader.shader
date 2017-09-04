Shader "PaintCraft/Basic Masked" {	
	Properties {		
		_MainTex ("Swatch", 2D) = "white" {}	
		_ClippingMask ("ClippingMask (Alpha8)", 2D) = "white" {}			
		_UpperLimit ("Mask alpha upperLimit", Range (0, 1)) = 0.9
		_LowerLimit ("Mask alpha lowerLimit", Range (0, 1)) = 0.5
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
     sampler2D _ClippingMask;
     fixed _UpperLimit;
     fixed _LowerLimit;
     
     struct v2f 
     {
	     float4  pos : SV_POSITION;
	     float2  uv : TEXCOORD0;
	     float2  uv2 : TEXCOORD1;
	     fixed4  color : COLOR;
     };

     float4 _MainTex_ST;
     float4 _ClippingMask_ST;

     v2f vert (appdata_full v)
     {
	     v2f o;
	     o.pos = UnityObjectToClipPos (v.vertex); 
	     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex); 
	     o.uv2 = TRANSFORM_TEX (v.texcoord1, _ClippingMask);
	     o.color =  v.color;
	     return o;
     }

     half4 frag (v2f i) : COLOR
     {
         fixed4 color = i.color;
         color.a *= tex2D (_MainTex, i.uv).a *  smoothstep ( _LowerLimit, _UpperLimit, tex2D (_ClippingMask, i.uv2).a);
         color.rgb *= color.a;
         return color;
     }

     ENDCG //Shader End
    }
   }
}

