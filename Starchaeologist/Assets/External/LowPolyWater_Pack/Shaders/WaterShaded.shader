// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "LowPolyWater/WaterShaded" {
Properties { 

	_BaseColor ("Base color", COLOR)  = ( .54, .95, .99, 0.5) 
	_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
    _Shininess ("Shininess", Float) = 10
	_ShoreTex ("Shore & Foam texture ", 2D) = "black" {} 
	 
	_InvFadeParemeter ("Auto blend parameter (Edge, Shore, Distance scale)", Vector) = (0.2 ,0.39, 0.5, 1.0)

	_BumpTiling ("Foam Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
	_BumpDirection ("Foam movement", Vector) = (1.0 ,1.0, -1.0, 1.0) 

	_Foam ("Foam (intensity, cutoff)", Vector) = (0.1, 0.375, 0.0, 0.0) 
	[MaterialToggle] _isInnerAlphaBlendOrColor("Fade inner to color or alpha?", Float) = 0 
}


CGINCLUDE 

	#include "UnityCG.cginc" 
	#include "UnityLightingCommon.cginc" // for _LightColor0


	sampler2D _ShoreTex;
	//Changed at the suggestion of http://answers.unity.com/answers/1560010/view.html; this is ultimately what 
	// fixed the pink missing texture effect
	//sampler2D_float _CameraDepthTexture;
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
  
	uniform float4 _BaseColor;  
    uniform float _Shininess;
	 
	uniform float4 _InvFadeParemeter;
    
	uniform float4 _BumpTiling;
	uniform float4 _BumpDirection;
 
	uniform float4 _Foam; 
  	float _isInnerAlphaBlendOrColor; 
	#define VERTEX_WORLD_NORMAL i.normalInterpolator.xyz 


	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		
		//VR compatibility addtion; see below
		//https://www.reddit.com/r/vive_vr/comments/p9jkyg/unity_a_gameobject_is_only_being_rendered_in_one/h9z7hbj/
		//https://docs.unity3d.com/Manual/SinglePassInstancing.html
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};
 
	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 normalInterpolator : TEXCOORD0;
		float4 viewInterpolator : TEXCOORD1;
		float4 bumpCoords : TEXCOORD2;
		float4 screenPos : TEXCOORD3;
		float4 grabPassPos : TEXCOORD4; 
		half3 worldRefl : TEXCOORD6;
		float4 posWorld : TEXCOORD7;
        float3 normalDir : TEXCOORD8;

		UNITY_FOG_COORDS(5)
		//VR compatibility addtion; see below
		//https://www.reddit.com/r/vive_vr/comments/p9jkyg/unity_a_gameobject_is_only_being_rendered_in_one/h9z7hbj/
		//https://docs.unity3d.com/Manual/SinglePassInstancing.html
		UNITY_VERTEX_OUTPUT_STEREO
	}; 
 
	inline half4 Foam(sampler2D shoreTex, half4 coords) 
	{
		half4 foam = (tex2D(shoreTex, coords.xy) * tex2D(shoreTex,coords.zw)) - 0.125;
		return foam;
	}

	v2f vert(appdata_full v)
	{
		/*v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);*/

		//VR compatibility addtion; see below
		//https://www.reddit.com/r/vive_vr/comments/p9jkyg/unity_a_gameobject_is_only_being_rendered_in_one/h9z7hbj/
		//https://docs.unity3d.com/Manual/SinglePassInstancing.html
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		
		half3 worldSpaceVertex = mul(unity_ObjectToWorld,(v.vertex)).xyz;
		half3 vtxForAni = (worldSpaceVertex).xzz;
 
		half3	offsets = half3(0,0,0);
		half3	nrml = half3(0,1,0);
		
		v.vertex.xyz += offsets;
		 
		half2 tileableUv = mul(unity_ObjectToWorld,(v.vertex)).xz;
		
		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

		o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.screenPos=ComputeScreenPos(o.pos); 
		o.normalInterpolator.xyz = nrml;
		o.viewInterpolator.w = saturate(offsets.y);
		o.normalInterpolator.w = 1; 
		
		UNITY_TRANSFER_FOG(o,o.pos);
 		half3 worldNormal = UnityObjectToWorldNormal(v.normal); 
   		float4x4 modelMatrix = unity_ObjectToWorld;
        float4x4 modelMatrixInverse = unity_WorldToObject; 
	 	o.posWorld = mul(modelMatrix, v.vertex);
        o.normalDir = normalize( mul(float4(v.normal, 0.0), modelMatrixInverse).xyz); 

        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos)); 
        o.worldRefl = reflect(-worldViewDir, worldNormal);

		return o;
	}
 
	 half4 calculateBaseColor(v2f input)  
         {
            float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float3 ambientLighting = 
               UNITY_LIGHTMODEL_AMBIENT.rgb * _BaseColor.rgb;
 
            float3 diffuseReflection = 
               attenuation * _LightColor0.rgb * _BaseColor.rgb
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else  
            {
               specularReflection = attenuation * _LightColor0.rgb  * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
            }

            return half4(ambientLighting + diffuseReflection  + specularReflection, 1.0);
         }

	half4 frag( v2f i ) : SV_Target
	{ 
 
		//VR compatibility addtion; see below
		//https://docs.unity3d.com/Manual/SinglePassInstancing.html
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
		
		#ifdef WATER_EDGEBLEND_ON
			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
			depth = LinearEyeDepth(depth);
			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.w));
			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
		#endif
		
 
        half4 baseColor = calculateBaseColor(i);
       
 
		half4 foam = Foam(_ShoreTex, i.bumpCoords * 2.0);
		baseColor.rgb += foam.rgb * _Foam.x * (edgeBlendFactors.y + saturate(i.viewInterpolator.w - _Foam.y));
		if( _isInnerAlphaBlendOrColor==0)
			baseColor.rgb += 1.0-edgeBlendFactors.x;
		if(  _isInnerAlphaBlendOrColor==1.0)
			baseColor.a  =  edgeBlendFactors.x;
		UNITY_APPLY_FOG(i.fogCoord, baseColor);
		return baseColor;
	}
	
ENDCG

Subshader
{
	Tags {"RenderType"="Transparent" "Queue"="Transparent"}
	
	Lod 500
	ColorMask RGB
	
	GrabPass { "_RefractionTex" }
	
	Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Cull Off
		
			CGPROGRAM
		
			#pragma target 3.0
		
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
		
			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF 
		
			ENDCG
	}
}


Fallback "Transparent/Diffuse"
}
