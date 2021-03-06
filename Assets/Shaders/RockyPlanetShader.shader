Shader "Planets/Rocky"
{
    Properties
    {
        _LightPosition ("Light Position", Vector) = (0.0, 0.0, 0.0, 1.0)
        _RotSpeed ("Rotation Speed", Float) = 2.0
        _FadeMin ("Fade Start Distance", Float) = 100.0
        _FadeMax ("Maximum Distance", Float) = 300.0
        _FadeRamp ("Fade Ramp Texture", 2D) = "" {}
        _NoiseScale ("Noise Scale", Float) = 3.0
        _NoiseDepth ("Noise Depth", Float) = 0.25
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
            #pragma multi_compile_instancing
            #pragma instancing_options forcemaxcount:128 nolodfade nolightprobe nolightmap

            #include "UnityCG.cginc"
            #include "./Shared.cginc"
            #include "./Noise.cginc"

            float3 _LightPosition;
            float _RotSpeed;
            float _FadeMin;
            float _FadeMax;
            sampler2D _FadeRamp;
            float _NoiseScale;
            float _NoiseDepth;
            float _RimPower;
            float _DitheringSize;
            sampler2D _RampTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 rayDir : TEXCOORD0;
                float3 rayOrigin : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;

                // allow Unity to do its GPU instancing magic
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                // don't let planet get smaller than a pixel
                float viewDepth = -UnityObjectToViewPos(v.vertex).z;
                float pixelToWorldScale = viewDepth * unity_CameraProjection._m11 / _ScreenParams.x;
                float minRadius = pixelToWorldScale * 0.75;

                // billboard quad towards camera
                float3 worldPos = billboard(v.vertex, max(0.5, minRadius));

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
                // more Unity GPU instancing boilerplate
                UNITY_SETUP_INSTANCE_ID(i);

                // if we're far enough away go to monocrome
                float distance = length(i.rayOrigin);
                if (distance > _FadeMin)
                {
                    // set outDepth to be the object pivot
                    float4 clipPos = UnityObjectToClipPos(float3(0, 0, 0));
                    outDepth = clipPos.z / clipPos.w;

                    float b = 1 - saturate((distance - _FadeMin) / (_FadeMax - _FadeMin));
                    return tex2D(_RampTexture, float2(b, 0));
                }

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

                // rim lighting
                float rimLighting = 1 + dot(rayDir, objectSpaceNormal);
                rimLighting = pow(rimLighting, _RimPower);
                lighting += rimLighting;

                // calculate rotated world position to use for noise
                float3x3 rotationMatrix = rotationMatrixAroundY(_Time.x * _RotSpeed);
                float3 noisePosition = mul(rotationMatrix, objectSpacePos);
                noisePosition = mul(unity_ObjectToWorld, float4(noisePosition, 1.0));

                // add noise
                lighting += noise(noisePosition * _NoiseScale) * _NoiseDepth;
                lighting += noise(noisePosition * _NoiseScale * 2.0) * 0.5 * _NoiseDepth;
                lighting += noise(noisePosition * _NoiseScale * 4.0) * 0.25 * _NoiseDepth;

                // subtle dithering effect
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
