Shader "Unlit/CycleUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexScaleX("Texture Scale", Float) = 1
        _TexScaleY("Texture Scale", Float) = 1
        _ScrollSpeed("Scroll Speed", Float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TexScaleX;
            float _TexScaleY;
            float _ScrollSpeed;
             
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float offset = _ScrollSpeed * _Time;

                // sample the texture
                fixed4 col = tex2D(_MainTex, ( float2(i.uv.x * _TexScaleX, i.uv.y *_TexScaleY) + float2(offset, 0)));
                return col;
            }
            ENDCG
        }
    }
}
