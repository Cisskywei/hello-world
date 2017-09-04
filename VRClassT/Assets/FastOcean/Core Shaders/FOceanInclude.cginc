// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef FASTOCEAN_CG_INCLUDE
#define FASTOCEAN_CG_INCLUDE

// interpolator structs
	struct v2f_FO
	{
		float4 pos : SV_POSITION;
#if defined (FO_GERSTNER_ON)	
		float4 normalInterpolator : TEXCOORD0;
#endif	
		float4 viewInterpolator : TEXCOORD1; 	
		float4 bumpCoords : TEXCOORD2;
		float4 screenPos : TEXCOORD3;
#if defined (FO_GERSTNER_ON)	
		float4 tanInterpolator : TEXCOORD4;
		float4 binInterpolator : TEXCOORD5;
#else
	#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
			float4 tileCoords : TEXCOORD6;
	#endif	
#endif 

#if defined (FO_FOAM_ON)
		float4 foamBumpInterpolator : TEXCOORD7;
#endif 
	};	

	struct v2f_UFO
	{
		float4 pos : SV_POSITION;
#if defined (FO_GERSTNER_ON)	
		float4 normalInterpolator : TEXCOORD0;
#endif	
		float4 viewInterpolator : TEXCOORD1; 	
		float4 bumpCoords : TEXCOORD2;
		float4 screenPos : TEXCOORD3;
#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		float4 tileCoords : TEXCOORD4;
#endif	
	};
	
	// textures
	sampler2D _OceanNM;

	float4 _BumpTiling;// need float precision
	float4 _BumpDirection;// need float precision

	//
	v2f_FO vert_FO(appdata_base vert)
	{
		v2f_FO o;

		half3 localSpaceVertex = vert.vertex.xyz;
#if defined (FO_PROJECTED_ON)

		FoProjInterpolate(localSpaceVertex);

		//float3 worldSpaceVertex = mul(_Object2World, half4(localSpaceVertex,1)).xyz;
		float3 center = float3(_FoCenter.x, 0, _FoCenter.z);
		float3 worldSpaceVertex = localSpaceVertex + center;
#else
		//use localVertex to eliminate floating point error
		localSpaceVertex.y = 0;

		float3 worldSpaceVertex = mul(unity_ObjectToWorld, half4(localSpaceVertex,1)).xyz;
#endif

		float3 distance = _WorldSpaceCameraPos - worldSpaceVertex;
#if defined (FO_EDITING)
		float4 fadeAway = float4(distance,Fade(distance));
#else 
		float4 fadeAway = float4(normalize(distance),Fade(distance));
#endif

		half3 offsets;
#if defined (FO_GERSTNER_ON)
		half3 nrml;	
		half3 tan;
		half3 bin;
		Gerstner (
			offsets, nrml, tan, bin, worldSpaceVertex							// offsets, nrml will be written
		);
		
		offsets *= fadeAway.w;
		nrml = lerp(WORLD_UP, nrml, fadeAway.w);
#else
	    offsets = 0;
#endif

		worldSpaceVertex += offsets;
		localSpaceVertex += offsets;	
		
		o.pos = UnityObjectToClipPos(half4(localSpaceVertex,1));

		float2 tileableUv = worldSpaceVertex.xz;
		float2 tileableUvScale = tileableUv * _InvFoScale;
		
		o.bumpCoords.xyzw = tileableUvScale.xyxy * _BumpTiling.xxyy + _BumpDirection;

		o.viewInterpolator = fadeAway;

		o.screenPos = ComputeScreenPos(o.pos);

#if defined (FO_GERSTNER_ON)	
		o.normalInterpolator.xyz = nrml;
		o.normalInterpolator.w = offsets.y; // 		o.normalInterpolator.w = -tan.g;
		o.tanInterpolator.xyz = tan;
	#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		o.tanInterpolator.w = (tileableUv.x - _TrailOffset.x) * _TrailOffset.z;
	#else
		o.tanInterpolator.w = 0;
	#endif
		o.binInterpolator.xyz = bin;
	#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		o.binInterpolator.w = (tileableUv.y - _TrailOffset.y) * _TrailOffset.z;
	#else
		o.binInterpolator.w = 0;
	#endif
#else
	#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		o.tileCoords.xyzw = (tileableUv.xyxy - _TrailOffset.xyxy) * _TrailOffset.z;
	#endif	
#endif

#if defined (FO_FOAM_ON)
		o.foamBumpInterpolator = tileableUvScale.xyxy * _FoamTiling.xxyy + _FoamDirection;
#endif 

		FO_TRANSFER_FOG(o,o.pos);
		return o;
	}

	//UNDERWATER
	v2f_UFO vert_UFO(appdata_base vert)
	{
		v2f_UFO o;

		half3 localSpaceVertex = vert.vertex.xyz;
#if defined (FO_PROJECTED_ON)

		FoProjInterpolate(localSpaceVertex);

#if defined (FO_SKIRT)
		localSpaceVertex.y -= vert.vertex.y * _Skirt;
#endif

		//float3 worldSpaceVertex = mul(_Object2World, half4(localSpaceVertex,1)).xyz;
		float3 center = float3(_FoCenter.x, 0, _FoCenter.z);
		float3 worldSpaceVertex = localSpaceVertex + center;
#else

#if defined (FO_SKIRT)
		localSpaceVertex.y = -vert.vertex.y * _Skirt;
#else
		localSpaceVertex.y = 0;
#endif

		//use localVertex to eliminate floating point error
		float3 worldSpaceVertex = mul(unity_ObjectToWorld, half4(localSpaceVertex,1)).xyz;
#endif

		float3 distance = _WorldSpaceCameraPos - worldSpaceVertex;
#if defined (FO_EDITING)
		float4 fadeAway = float4(distance,Fade(distance));
#else 
		float4 fadeAway = float4(normalize(distance),Fade(distance));
#endif

		half3 offsets;
#if defined (FO_GERSTNER_ON)
		half3 nrml;	
		Gerstner (
			offsets, nrml, worldSpaceVertex							// offsets, nrml will be written
		);
		
		offsets *= fadeAway.w;
		nrml = lerp(WORLD_UP, nrml, fadeAway.w);
#else
	    offsets = 0;
#endif

		worldSpaceVertex += offsets;	
		localSpaceVertex += offsets;	
		
		o.pos = UnityObjectToClipPos(half4(localSpaceVertex,1));

		float2 tileableUv = worldSpaceVertex.xz;
		float2 tileableUvScale = tileableUv * _InvFoScale;
		
		o.bumpCoords.xyzw = tileableUvScale.xyxy * _BumpTiling.xxyy + _BumpDirection;

		o.viewInterpolator = fadeAway;

		o.screenPos = ComputeScreenPos(o.pos);

#if defined (FO_GERSTNER_ON)	
		nrml.y = -nrml.y;
		o.normalInterpolator.xyz = nrml;
		o.normalInterpolator.w = offsets.y;
#endif

#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		o.tileCoords.xyzw = (tileableUv.xyxy - _TrailOffset.xyxy) * _TrailOffset.z;
#endif	

		FO_TRANSFER_FOG(o, o.pos);
		return o;
	}

	fixed4 frag_FO( v2f_FO i ) : SV_Target
	{	
#if defined (FO_EDITING)
		half3 viewVector = normalize(i.viewInterpolator.xyz);
#else 
		half3 viewVector = i.viewInterpolator.xyz;
#endif
						
#if defined (FO_GERSTNER_ON)	
       FO_TANGENTSPACE

	    half3 worldNormal = PerPixelTangentSpaceParallax(_OceanNM, i.bumpCoords, m, viewVector, BUMP_POWER);
		half3 worldNormal2 = PerPixelTangentSpaceParallax(_OceanNM, i.bumpCoords, m, viewVector,BUMP_SHARPBIAS);
#else
	    half3 worldNormal = PerPixelNormal(_OceanNM, i.bumpCoords, BUMP_POWER);
		half3 worldNormal2 = PerPixelNormal(_OceanNM, i.bumpCoords, BUMP_SHARPBIAS);
#endif

#if defined (FO_GERSTNER_ON)	
		half top = i.normalInterpolator.w;
#endif

		half fade = i.viewInterpolator.w;

		worldNormal = lerp(WORLD_UP, worldNormal, fade);

#if defined (FO_FOAM_ON) || defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		half2 slope = worldNormal.xz;
#endif	

		worldNormal.xz *= _FresnelScale;

		half NDotV = abs(dot(viewVector, worldNormal));
		half refl2Refr = Fresnel(_FresnelLookUp, NDotV);
		half refl2RefrFade = refl2Refr * fade;
		
#if defined (FO_GERSTNER_ON)	
		fixed4 baseColor = ExtinctColor (_FoBaseColor, top, _ExtinctScale * refl2Refr);
#else
		fixed4 baseColor = _FoBaseColor;
#endif
			
#if defined (FO_DEPTHBLEND_ON)
		half depth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, i.screenPos));
		depth = LinearEyeDepth(depth) - i.screenPos.w;
#endif	

		fixed4 shallowColor = _FoShallowColor;

#if defined (FO_FOAM_ON)
	half4 distortUV = slope.xyxy * _FoamTime.zzww;
	half4 foamUV = i.foamBumpInterpolator + distortUV;

	fixed4 foam = Foam(foamUV) * _FoSunColor;

	#if defined (FO_GERSTNER_ON)
		FoWaveFoam(top, refl2RefrFade * foam, foamUV.zw, baseColor);
	#else
		FoWaveFoamSimple(refl2RefrFade * foam, foamUV.zw, baseColor);
	#endif

	#if defined (FO_DEPTHBLEND_ON)	
		FoShallowFoam(foam, depth, distortUV.xy, shallowColor);
	#endif
#endif	

#if defined (FO_DEPTHBLEND_ON)	
		baseColor = lerp(shallowColor, baseColor, saturate(_ShallowEdge * depth));
#endif

		baseColor = lerp(baseColor, _FoDeepColor, pow(NDotV, _FoDeepColor.a));
				
		half4 screenPosproj = half4(i.screenPos.xy + worldNormal.xz * REALTIME_DISTORTION * i.screenPos.w, 0, i.screenPos.w);
		
		fixed4 rtReflections = tex2Dproj(_ReflectionTex, screenPosproj);
		fixed4 reflectionColor = lerp (UNITY_LIGHTMODEL_AMBIENT,rtReflections, _FoReflAmb2Scene);

		baseColor = lerp (baseColor, reflectionColor, refl2Refr);

#if defined (FO_PHONG_ON)
		half spec = PhongSpecular(viewVector, worldNormal2);
#elif defined (FO_BLINNPHONG_ON)
		half spec = BlinnPhongSpecular(viewVector, worldNormal2);
#else
		half spec = 0;
#endif	

#if defined (FO_TRAIL_ON)
	#if defined (FO_GERSTNER_ON)
		half4 tileCoords = half4(i.tanInterpolator.w, i.binInterpolator.w, slope.xy * PER_TRAILS_DISTORT);
	#else
		half4 tileCoords = half4(i.tileCoords.xy, slope.xy * PER_TRAILS_DISTORT);
	#endif
	#if defined (FO_FOAM_ON)
		baseColor += FoTrailsFoam(tileCoords, _FoSunColor * spec, foam * fade);
	#else 
		baseColor += FoTrailsMask(tileCoords, spec, fade) * _FoSunColor;
	#endif	
#elif defined (FO_TRAILSCREEN_ON)
	#if defined (FO_GERSTNER_ON)
		half4 tileCoords = half4(i.tanInterpolator.w, i.binInterpolator.w, slope.xy * PER_TRAILS_DISTORT);
	#else
		half4 tileCoords = half4(i.tileCoords.xy, slope.xy * PER_TRAILS_DISTORT);
	#endif
	#if defined (FO_FOAM_ON)
		baseColor += FoTrailsFoamScreen(tileCoords, i.screenPos, _FoSunColor * spec, foam * fade);
	#else 
		baseColor += FoTrailsMaskScreen(tileCoords, i.screenPos, spec, fade) * _FoSunColor;
	#endif	
#else
	#if defined (FO_PHONG_ON) || defined (FO_BLINNPHONG_ON)
		baseColor += spec;
	#endif
#endif

		FO_APPLY_FOG(i.screenPos, baseColor);

#if defined (FO_DEPTHBLEND_ON)		
		baseColor.a = saturate(_AboveDepth * depth * (1 - refl2RefrFade));
#else
		baseColor.a = saturate(1 - refl2RefrFade * (1 - _FoShallowColor.a));
#endif	
		baseColor.a *= _FoBaseColor.a;
		return baseColor;
	}

	//UNDERWATER
	fixed4 frag_UFO( v2f_UFO i ) : SV_Target
	{				
#if defined (FO_GERSTNER_ON)	
	    half3 worldNormal = PerPixelNormalBump(_OceanNM, i.bumpCoords, VERTEX_WORLD_NORMAL, BUMP_POWER);
		half3 worldNormal2 = PerPixelNormalBump(_OceanNM, i.bumpCoords, VERTEX_WORLD_NORMAL, BUMP_SHARPBIAS);
#else
	    half3 worldNormal = -PerPixelNormal(_OceanNM, i.bumpCoords, -BUMP_POWER);
		half3 worldNormal2 = -PerPixelNormal(_OceanNM, i.bumpCoords, -BUMP_SHARPBIAS);
#endif

#if defined (FO_EDITING)
		half3 viewVector = normalize(i.viewInterpolator.xyz);
#else 
		half3 viewVector = i.viewInterpolator.xyz;
#endif

		half fade = i.viewInterpolator.w;

#if defined (FO_GERSTNER_ON)	
		half top = i.normalInterpolator.w;
#endif

		worldNormal = lerp(-WORLD_UP, worldNormal, fade);
#if defined (FO_TRAIL_ON) || defined (FO_TRAILSCREEN_ON)
		half2 slope = worldNormal.xz;
#endif	
		worldNormal.xz *= _FresnelScale;

		half NDotV = abs(dot(viewVector, worldNormal));
		half refl2Refr = 1 - Fresnel(_FresnelLookUp, NDotV);
		half refl2RefrFade = refl2Refr * fade;

#if defined (FO_DEPTHBLEND_ON)					
		half depth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, i.screenPos));
		depth = LinearEyeDepth(depth) - i.screenPos.w;
#endif

#if defined (FO_GERSTNER_ON)
		fixed4 baseColor = ExtinctColor (_FoBaseColor, top, _ExtinctScale * (1 - refl2Refr));
#else
		fixed4 baseColor = _FoBaseColor;
#endif

#if defined (FO_DEPTHBLEND_ON)
		baseColor = lerp(_FoShallowColor, baseColor, saturate(_ShallowEdge * depth));
#endif

		baseColor = lerp(baseColor, _FoDeepColor, pow(NDotV, _FoDeepColor.a));

		baseColor = lerp (baseColor, UNITY_LIGHTMODEL_AMBIENT, refl2Refr);

		half spec = SpecularU(viewVector, worldNormal2);

#if defined (FO_TRAIL_ON)
		half4 tileCoords = half4(i.tileCoords.xy, slope.xy * PER_TRAILS_DISTORT);
		baseColor += FoTrailsMask(tileCoords, spec, fade) * _FoSunColor;
#elif defined (FO_TRAILSCREEN_ON)
		half4 tileCoords = half4(i.tileCoords.xy, slope.xy * PER_TRAILS_DISTORT);
		baseColor += FoTrailsMaskScreen(tileCoords, i.screenPos, spec, fade) * _FoSunColor;
#else
		baseColor += spec;
#endif


		FO_APPLY_FOG(i.screenPos, baseColor);

#if defined (FO_DEPTHBLEND_ON)
		baseColor.a = saturate(_UnderDepth * depth * (1 - refl2RefrFade));
#else 
		baseColor.a = saturate(refl2RefrFade); //use refl2Refr as depth
#endif	

		baseColor.a *= _FoBaseColor.a;
		return baseColor;
	}

	half4 frag_OCEAN_MAP( v2f_FO i ) : SV_Target
	{				
		return half4(i.screenPos.w * _ProjectionParams.w, 0, 0, 1);
	}

	half4 frag_UOCEAN_MAP(v2f_UFO i) : SV_Target
	{
		return half4(i.screenPos.w * _ProjectionParams.w, 0, 0, 1);
	}

	half4 frag_UOCEAN_MAP2(v2f_FO i) : SV_Target
	{
		return half4(0, 0, 0, 1);
	}

	fixed4 frag_GLARE_MAP( v2f_FO i ) : SV_Target
	{				
#if defined (FO_EDITING)
		half3 viewVector = normalize(i.viewInterpolator.xyz);
#else 
		half3 viewVector = i.viewInterpolator.xyz;
#endif

#if defined (FO_GERSTNER_ON)	
		FO_TANGENTSPACE

		half3 worldNormal2 = PerPixelTangentSpaceParallax(_OceanNM, i.bumpCoords, m, viewVector,BUMP_SHARPBIAS);
#else
		half3 worldNormal2 = PerPixelNormal(_OceanNM, i.bumpCoords, BUMP_SHARPBIAS);
#endif
		
		half fade = i.viewInterpolator.w;

#if defined (FO_PHONG_ON)
		half spec = PhongSpecular(viewVector,worldNormal2);
#elif defined (FO_BLINNPHONG_ON)
		half spec = BlinnPhongSpecular(viewVector,worldNormal2);
#else 
		half spec = 0;
#endif	

#if defined (FO_TRAIL_ON)
		#if defined (FO_GERSTNER_ON)
			half4 tileCoords = half4(i.tanInterpolator.w, i.binInterpolator.w, 0, 0);
		#else
			half4 tileCoords = half4(i.tileCoords.xy, 0, 0);
		#endif
		spec = FoTrailsMask(tileCoords, spec, fade);
#elif defined (FO_TRAILSCREEN_ON)
		#if defined (FO_GERSTNER_ON)
				half4 tileCoords = half4(i.tanInterpolator.w, i.binInterpolator.w, 0, 0);
		#else
				half4 tileCoords = half4(i.tileCoords.xy, 0, 0);
		#endif
		spec = FoTrailsMaskScreen(tileCoords, i.screenPos, spec, fade);
#endif

		fixed4 c = fixed4(spec * fade * _FoBaseColor.a, 0, 0, 1);

		FO_APPLY_FOG_COLOR(i.screenPos,c,fixed4(0,0,0,0));
	    return fixed4(c.rgb, 1);
	}
			
#endif
