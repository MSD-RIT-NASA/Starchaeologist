Shader "Custom/Triplanar"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _Sharpness ("Sharpness", float) = 0.5
         
        _FrontTexture   ("Front", 2D) = "white" {}
        _SideTexture    ("Side",  2D) = "white" {}
        _TopTexture     ("Top",   2D) = "white" {}

        _FrontTiling ("FrontTiling", Vector) = (1, 1, 0, 0)
        _FrontOffset ("FrontOffset", Vector) = (0, 0, 0, 0)
        _SideTiling ("SideTiling", Vector) = (1, 1, 0, 0)
        _SideOffset ("SideOffset", Vector) = (0, 0, 0, 0)
        _TopTiling ("TopTiling", Vector) = (1, 1, 0, 0)
        _TopOffset ("TopOffset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        sampler2D _MainTex;
        sampler2D _FrontTexture;
        sampler2D _SideTexture;
        sampler2D _TopTexture;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };


        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Sharpness;

        float2 _FrontTiling;
        float2 _FrontOffset;

        void TilingAndOffset(float2 uv, float2 tiling, float2 offset, out float2 Out)
        {
            Out = uv * tiling + offset;
        }

        float2 TilingAndOffset(float2 uv, float2 tiling, float2 offset)
        {
            return uv * tiling + offset;
        }

        float4 SampleTilAndOffset(sampler2D text, float2 uv, float2 tiling, float2 offset)
        {
            float2 nUV;
            TilingAndOffset(uv, tiling, offset, nUV);

            return tex2D(text, nUV);
        }

    float3 TriplanarTexture(
        float3 worldPos,
        float3 worldNormal,
        float sharpness,
        sampler2D frontTexture,
        sampler2D sideTexture,
        sampler2D topTexture)
    {
        // Get unique triplanar UVs based on normals
        //float2 frontUV  = TilingAndOffset(worldPos.xy,    frontTiling,   frontOffset);
        //float2 sideUV   = TilingAndOffset(worldPos.zy,    sideTiling,    sideOffset);
        //float2 topUV    = TilingAndOffset(worldPos.xz,    topTiling,     topOffset);
        
        // Sample from given textures  
        float3 front    = tex2D(frontTexture,   worldPos.xy);
        float3 side     = tex2D(sideTexture,    worldPos.zy);
        float3 top      = tex2D(topTexture,     worldPos.xz);
        
        // Normals used to determine weight of each texture
        // to interpolate from  
        float3 norm = pow(abs(worldNormal), sharpness);
        
        float base = norm.x + norm.y + norm.z;
        norm /= base;
        
        front   *= saturate(norm.z);
        side    *= saturate(norm.x);
        top     *= saturate(norm.y);
        
        return front + side + top;
    }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 5) - 0.5);
            float3 normal = o.Normal;
            
            //return TriplanarTexture(
            //        IN.WorldPos,
            //        o.Normal,
            //        10.0f,
            //        frontTexture,
            //        sideTexture,
            //        topTexture
            //    );

            
            float2 target;
            TilingAndOffset(
                IN.uv_MainTex,
                float2(0,0),
                float2(0,0),
                target);


            float3 normData = abs(normal);
            normData = pow(normData, _Sharpness);
            float XYZSum = normData.x + normData.y + normData.z;
            normData = normData / XYZSum;

            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 c = SampleTilAndOffset(
                _MainTex,       
                IN.uv_MainTex,  
                _FrontTiling,   
                _FrontOffset);  
            o.Albedo = normData; //c.rgb;


            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Albedo = IN.worldPos;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
