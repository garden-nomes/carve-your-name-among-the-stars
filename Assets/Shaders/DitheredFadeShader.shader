Shader "GUI/Dithered Fade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fade ("Fade", Range(0, 1)) = 0.5
        _Color ("Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off Lighting Off ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            static float ditherMatrix[64] = {
                0, 32,  8, 40,  2, 34, 10, 42,
                48, 16, 56, 24, 50, 18, 58, 26,
                12, 44,  4, 36, 14, 46,  6, 38,
                60, 28, 52, 20, 62, 30, 54, 22,
                3, 35, 11, 43,  1, 33,  9, 41,
                51, 19, 59, 27, 49, 17, 57, 25,
                15, 47,  7, 39, 13, 45,  5, 37,
                63, 31, 55, 23, 61, 29, 53, 21
            };

            float dither(float2 pixel, float value)
            {
                float index = fmod(pixel.y, 8.) * 8. + fmod(pixel.x, 8.);
                float ditherThreshold = ditherMatrix[index] / 64.;
                return step(1.0 - value, ditherThreshold);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _Fade;
            float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 toCenter = i.uv - 0.5;
                float dist = 1 - length(toCenter) / (sqrt(2) * 0.5);
                dist = pow(dist, 1.2);
                float fade = dist + _Fade * 2 - 1;

                float2 pixel = floor(i.uv * _ScreenParams);
                return lerp(_Color, float4(_Color.xyz, 0), dither(pixel, fade));
            }
            ENDCG
        }
    }
}
