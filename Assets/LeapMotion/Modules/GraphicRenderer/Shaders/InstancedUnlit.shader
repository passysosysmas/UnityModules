// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/InstancedUnlit" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
      #pragma multi_compile_instancing
			
			#include "UnityCG.cginc"
      #include "Assets/LeapMotion/Modules/GraphicRenderer/Resources/CylindricalSpace.cginc"

      UNITY_INSTANCING_CBUFFER_START(MyProperties)
      UNITY_DEFINE_INSTANCED_PROP(float4, _GraphicRendererCurved_GraphicParameters)
      UNITY_INSTANCING_CBUFFER_END

      float4x4 _GraphicRenderer_LocalToWorld;

			struct appdata {
				float4 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v) {
				v2f o;

        UNITY_SETUP_INSTANCE_ID(v);

        v.vertex = mul(unity_ObjectToWorld, v.vertex);
        float4 graphicParams = UNITY_ACCESS_INSTANCED_PROP(_GraphicRendererCurved_GraphicParameters);
        Cylindrical_LocalToWorld(v.vertex.xyz, graphicParams);

        v.vertex = mul(_GraphicRenderer_LocalToWorld, v.vertex);

        o.vertex = mul(UNITY_MATRIX_VP, v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				return 1;
			}
			ENDCG
		}
	}
}
