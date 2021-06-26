Shader "Planets/Garden World"
{
    Properties
    {
        _LightPosition ("Light Position", Vector) = (0.0, 0.0, 0.0, 1.0)
        _FadeMin ("Fade Start Distance", Float) = 100.0
        _FadeMax ("Maximum Distance", Float) = 300.0
        _FadeRamp ("Fade Ramp Texture", 2D) = "" {}
        _DitheringSize ("Dither Size", Float) = 0.1

        [Header(Water)]
        _WaterNoiseScale ("Noise Scale", Float) = 0.1
        _WaterRimPower ("Rim Lighting", Float) = 3.0
        _WaterRampTexture ("Ramp Texture", 2D) = ""

        [Header(Land)]
        _LandmassScale ("Noise Scale", Float) = 1.0
        _LandCoverage ("Coverage", Float) = 0.5
        _LandRimPower ("Rim Lighting", Float) = 3.0
        _LandRampTexture ("Ramp Texture", 2D) = ""

        [Header(Clouds)]
        _CloudScale ("Cloud Noise Scale", Float) = 1.0
        _CloudCoverage ("Cloud Coverage", Float) = 0.5
        _TurbulenceScale ("Turbulence Scale", Float) = 1.0
        _TurbulenceSpeed ("Turbulence Speed", Float) = 1.0
        _TurbulenceStrength ("Turbulence Strength", Float) = 1.0
        _TurbulenceRotation ("Turbulence Rotation", Float) = 1.0
        _Stretch ("Turbulence Stretch", Float) = 1.0
        // _CloudColor ("Cloud Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Pass
        {
            // water pass

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "./Shared.cginc"
            #include "./Noise.cginc"

            float3 _LightPosition;
            float _FadeMin;
            float _FadeMax;
            sampler2D _FadeRamp;
            float _DitheringSize;
            float _WaterNoiseScale;
            float _WaterRimPower;
            sampler2D _WaterRampTexture;

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

                // ignore pass if we're past the fade distance
                float distance = length(i.rayOrigin);
                clip(_FadeMin - distance);

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

                // add noise
                float n = noise(objectSpacePos * _WaterNoiseScale);
                lighting += n * 0.25;

                // rim lighting
                float rimLighting = 1 + dot(rayDir, objectSpaceNormal);
                rimLighting = pow(rimLighting, _WaterRimPower);
                lighting += rimLighting;

                // dithering
                float2 pixel = floor(i.screenPos * _ScreenParams / i.screenPos.w);
                lighting += ((pixel.x + pixel.y) % 2) * _DitheringSize;
                lighting *= (1 - _DitheringSize);

                float4 color = tex2D(_WaterRampTexture, float2(lighting, 0));

                return color;
            }
            ENDCG
        }

        Pass
        {
            // land pass

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "./Shared.cginc"
            #include "./Noise.cginc"

            float3 _LightPosition;
            float _FadeMin;
            float _FadeMax;
            sampler2D _FadeRamp;
            float _DitheringSize;
            float _LandmassScale;
            float _LandCoverage;
            float _LandRimPower;
            sampler2D _LandRampTexture;

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
                    float b = 1 - saturate((distance - _FadeMin) / (_FadeMax - _FadeMin));
                    return tex2D(_LandRampTexture, float2(b, 0));
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

                // three-octave noise
                float landmassNoise = noise(objectSpacePos * _LandmassScale);
                landmassNoise += noise(objectSpacePos * _LandmassScale * 2) * 0.5;
                landmassNoise += noise(objectSpacePos * _LandmassScale * 4) * 0.25;
                landmassNoise = landmassNoise;

                // clip if on water
                clip(_LandCoverage - (landmassNoise * 0.5 + 0.5));

                // calculate lighting
                float3 worldLightDir = normalize(_LightPosition - worldSpacePos);
                float lighting = dot(worldSpaceNormal, worldLightDir);

                // apply noise
                lighting += landmassNoise * 0.40;

                // rim lighting
                float rimLighting = 1 + dot(rayDir, objectSpaceNormal);
                rimLighting = pow(rimLighting, _LandRimPower);
                lighting += rimLighting;

                // dithering
                float2 pixel = floor(i.screenPos * _ScreenParams / i.screenPos.w);
                lighting += ((pixel.x + pixel.y) % 2) * _DitheringSize;
                lighting *= (1 - _DitheringSize);

                float4 color = tex2D(_LandRampTexture, float2(lighting, 0));

                return color;
            }
            ENDCG
        }

        Pass
        {
            // cloud pass

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "./Shared.cginc"
            #include "./Noise.cginc"

            float3 _LightPosition;
            float _FadeMin;
            float _FadeMax;
            sampler2D _FadeRamp;
            float _DitheringSize;
            float _CloudScale;
            float _CloudCoverage;
            float _TurbulenceScale;
            float _TurbulenceSpeed;
            float _TurbulenceStrength;
            float _TurbulenceRotation;
            float _Stretch;
            float4 _CloudColor;

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

                // ignore pass if we're past the fade distance
                float distance = length(i.rayOrigin);
                clip(_FadeMin - distance);

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

                // turbulence
                float3 turbulenceNoisePosition = objectSpacePos * _TurbulenceScale;
                turbulenceNoisePosition *= float3(1, _Stretch, 1);
                turbulenceNoisePosition = rotateXZ(turbulenceNoisePosition, _Time.x * _TurbulenceRotation);
                turbulenceNoisePosition += _Time.x * _TurbulenceSpeed;
                float turbulence = noise(turbulenceNoisePosition);

                // add noise
                float3 noisePosition = objectSpacePos * _CloudScale + _Time.x * _TurbulenceSpeed;
                noisePosition += turbulence * _TurbulenceStrength;
                float n = noise(noisePosition) * 0.5 + 0.5;

                // dithering
                float2 pixel = floor(i.screenPos * _ScreenParams / i.screenPos.w);
                n += ((pixel.x + pixel.y) % 2) * _DitheringSize;
                n *= (1 - _DitheringSize);

                clip(_CloudCoverage - n);

                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}
