// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PaintCraft/Basic MaskedRegion" {	
	Properties {		
		_MainTex ("Swatch", 2D) = "white" {}	
		_ClippingMask ("ClippingMask (Alpha8)", 2D) = "white" {}			
		_UpperLimit ("Mask alpha upperLimit", Range (0, 1)) = 0.9
		_LowerLimit ("Mask alpha lowerLimit", Range (0, 1)) = 0.5

		_RegionTex("Regions Texture", 2D) = "white" {}		
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
	 sampler2D 	_RegionTex;

     fixed _UpperLimit;
     fixed _LowerLimit;
     
	

     struct v2f 
     {
	     float4  pos : SV_POSITION;
	     float2  uv0 : TEXCOORD0; // swatch texture
	     float2  uv1 : TEXCOORD1; // cliping mask
		 float2  uv2 : TEXCOORD2; // region texture
		 float2  uv3 : TEXCOORD3; // original click
	     fixed4  color : COLOR;
     };

     float4 _MainTex_ST;
     float4 _ClippingMask_ST;
	 float4 _RegionTex_ST;

     v2f vert (appdata_full v)
     {
	     v2f o;
	     o.pos = UnityObjectToClipPos (v.vertex); 
	     o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex); 
	     o.uv1 = TRANSFORM_TEX(v.texcoord1, _ClippingMask);
		 o.uv2 = TRANSFORM_TEX(v.texcoord1, _MainTex);
		 o.uv3 = TRANSFORM_TEX(v.texcoord3, _MainTex);
	     o.color =  v.color;		
	     return o;
     }
     
     
     
     bool isInRegion(float2 uv, float2 origin){
        fixed4 region = tex2D(_RegionTex, uv);
        fixed4 original = tex2D(_RegionTex, origin);
        if (original.a == 0){
            return 0;
        }
        fixed4 tmp = (1 - (region - original));
        return tmp.r + (tmp.g*10) + (tmp.b*100) == 111;
      }

     half4 frag (v2f i) : COLOR
     {
         fixed4 color = i.color;
         color.a *= tex2D (_MainTex, i.uv0).a *  smoothstep ( _LowerLimit, _UpperLimit, tex2D (_ClippingMask, i.uv1).a);		 
	 	 color *= isInRegion(i.uv2, i.uv3);
	 	 color.rgb *= color.a;	 	 
         return color;
     }

     ENDCG //Shader End
    }
   }
}

