Shader "Planets/Gas Giant"
{
    Properties
    {
        _LightPosition ("Light Position", Vector) = (0.0, 0.0, 0.0, 1.0)
        _NoiseScale ("Noise Scale", Float) = 3.0
        _TurbulenceScale ("Turbulence Scale", Float) = 3.0
        _TurbulenceSpeed ("Turbulence Speed", Float) = 1.0
        _TurbulenceStrength ("Turbulence Strength", Float) = 1.0
        _TurbulenceRotation ("Turbulence Rotation Speed", Float) = 1.0
        _Stretch ("Stretch Noise Horizontally", Float) = 5.0
        _RampTexture ("Ramp Texture", 2D) = "red"
        _RimPower ("Rim Lighting", Float) = 3.0
        _DitheringSize ("Dither Size", Float) = 0.1
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./Shared.cginc"
            #include "./Noise.cginc"

            float3 _LightPosition;
            float _NoiseScale;
            float _TurbulenceScale;
            float _TurbulenceSpeed;
            float _TurbulenceStrength;
            float _TurbulenceRotation;
            float _Stretch;
            float _RimPower;
            float _DitheringSize;
            sampler2D _RampTexture;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 rayDir : TEXCOORD0;
                float3 rayOrigin : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // billboard quad towards camera
                float3 worldPos = billboard(v.vertex, 0.5);

                // construct an object-space ray from the camera to this vertex
                float3 worldRayDir = worldPos - _WorldSpaceCameraPos.xyz;
                o.rayDir = mul(unity_WorldToObject, float4(worldRayDir, 0.0));
                o.rayOrigin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1.0));

                // clip and screen position
                o.pos = UnityWorldToClipPos(worldPos);
                o.screenPos = ComputeScreenPos(o.pos);

                return o;
            }

            fixed4 frag(v2f i, out float outDepth : SV_Depth) : SV_Target
            {
                // clip sphere
                float3 rayDir = normalize(i.rayDir);
                float rayHit = sphereIntersect(i.rayOrigin, rayDir, float4(0, 0, 0, 0.5));
                clip(rayHit);

                // compute position on sphere in objectr and world space
                float3 objectSpacePos = rayDir * rayHit + i.rayOrigin;
                float3 worldSpacePos = mul(unity_ObjectToWorld, float4(objectSpacePos, 1.0));

                // computer normal in object and world space
                float3 objectSpaceNormal = normalize(objectSpacePos);
                float3 worldSpaceNormal = UnityObjectToWorldNormal(objectSpaceNormal);

                // set outDepth to allow for intersections with other objects
                float4 clipPos = UnityWorldToClipPos(worldSpacePos);
                outDepth = clipPos.z / clipPos.w;

                // calculate lighting
                float3 worldLightDir = normalize(_LightPosition - worldSpacePos);
                float lighting = dot(worldSpaceNormal, worldLightDir);

                // turbulence
                float3 turbulenceNoisePosition = objectSpacePos * _TurbulenceScale;
                turbulenceNoisePosition *= float3(1, _Stretch, 1);
                turbulenceNoisePosition = rotateXZ(turbulenceNoisePosition, _Time.x * _TurbulenceRotation);
                turbulenceNoisePosition += _Time.x * _TurbulenceSpeed;
                float turbulence = noise(turbulenceNoisePosition);

                // add noise
                float3 noisePosition = objectSpacePos * float3(1, 1, 1) * _NoiseScale;
                noisePosition += turbulence * _TurbulenceStrength;
                float n = noise(noisePosition) * 0.5 + 0.5;
                lighting *= n;

                // rim lighting
                float rimLighting = 1 + dot(rayDir, objectSpaceNormal);
                rimLighting = pow(rimLighting, _RimPower);
                lighting += rimLighting;

                // dithering
                float2 pixel = floor(i.screenPos * _ScreenParams / i.screenPos.w);
                lighting += ((pixel.x + pixel.y) % 2) * _DitheringSize;
                lighting *= (1 - _DitheringSize);

                float4 color = tex2D(_RampTexture, float2(lighting, 0));

                return color;
            }
            ENDCG
        }
    }
}
