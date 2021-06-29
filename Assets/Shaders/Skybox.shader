Shader "Skybox/Space"
{
    Properties
    {
        _NebulaRamp ("Nebula Color Ramp", 2D) = "" {}
        _NoiseScale ("Noise Scale", Float) = 1
        _NoiseCutoff ("Noise Cutoff", Range(0, 1)) = 0.5
        _OffsetScale ("Offset Scale", Float) = 1
        _OffsetStrength ("Offset Strength", Float) = 0.5
        _Dithering ("Dithering", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./Noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _NebulaRamp;
            float _NoiseScale;
            float _NoiseCutoff;
            float _OffsetScale;
            float _OffsetStrength;
            float _Dithering;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(v.vertex);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 spherePosition = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);
                float noiseOffset = noise(spherePosition * _OffsetScale);

                float nebula = noise(spherePosition * _NoiseScale + noiseOffset * _OffsetStrength);
                nebula += noise(spherePosition * _NoiseScale * 2) * 0.5;
                nebula += noise(spherePosition * _NoiseScale * 4) * 0.25;
                nebula += noise(spherePosition * _NoiseScale * 8) * 0.125;
                nebula += noise(spherePosition * _NoiseScale * 32) * 0.25;

                nebula = (nebula - _NoiseCutoff) / (1 - _NoiseCutoff);

                // dithering
                // float2 pixel = floor(i.screenPos * _ScreenParams / i.screenPos.w);
                // nebula += ((pixel.x + pixel.y) % 2) * _Dithering;
                // nebula *= (1 - _Dithering);

                return tex2D(_NebulaRamp, float2(nebula, 0));
            }
            ENDCG
        }
    }
}
