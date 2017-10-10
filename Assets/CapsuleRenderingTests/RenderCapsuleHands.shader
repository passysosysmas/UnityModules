// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "LeapMotion/Test/RenderCapsuleHands" {
	Properties {

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM

      #include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
        float4 worldVertex : TEXCOORD1;
			};

      uniform float3 testCapsA;
      uniform float3 testCapsB;
      uniform float testCapsR;

#define NUM_CAPSULES 26

      uniform float4 CapsulePos0s[NUM_CAPSULES];
      uniform float4 CapsulePos1s[NUM_CAPSULES];
      uniform float  CapsuleRadii[NUM_CAPSULES];

      v2f vert(appdata v) {
        v2f o;
        o.uv = v.uv;
        o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
        o.vertex = UnityObjectToClipPos(v.vertex);
        return o;
      }

      // IQ sdCapsule
      float distanceToCapsule(half3 p, half4 a, half4 b, float r) {
        half3 ap = p - a, ab = b - a;
        float h = saturate(dot(ap, ab) / dot(ab, ab));
        return length(ap - (ab * h)) - r;
      }

      float getSceneDistance(half3 p) {
        float smallestDist = 10000;
        for (int i = 0; i < NUM_CAPSULES; i++) {
          float capsDist = distanceToCapsule(p, CapsulePos0s[i], CapsulePos1s[i], CapsuleRadii[i]);
          smallestDist = min(smallestDist, capsDist);
        }
        return smallestDist;
      }
      float distanceToSphere(half3 p, half r) {
        return length(p) - r;
      }

      float distanceToSphere(half3 p, half3 c, half r) {
        return length(c - p) - r;
      }
			
			fixed4 frag (v2f i) : SV_Target {
        float3 rayOrigin = _WorldSpaceCameraPos;
        float3 rayDir = normalize(i.worldVertex - rayOrigin);


        //return distanceToSphere(rayOrigin + rayDir, testCapsA, testCapsR) < 0;

        float3 rayPoint = rayOrigin;
        float step;
        float traveled = 0;
        float cutoff = 0.01;
        for (int j = 0; j < 8; j++) {
          step = getSceneDistance(rayPoint);
          rayPoint += rayDir * step;
          traveled += step;
          if (step < cutoff) return lerp(1, 0, traveled - 0.1) * 2;
        }

        return float4(0.3, 0, 0.3, 1);
			}

      #pragma vertex vert
      #pragma fragment frag

			ENDCG
		}
	}
}
