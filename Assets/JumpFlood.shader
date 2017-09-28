Shader "Hidden/JumpFlood" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}

  CGINCLUDE
  #include "UnityCG.cginc"

  struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
  };

  struct v2f {
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
  };

  struct v2f_jump {
    float2 uv[9] : TEXCOORD0;
    float4 vertex : SV_POSITION;
  };

  float _Step;
  float2 _MainTex_TexelSize;
  sampler2D _MainTex;
  sampler2D _Source;

  v2f vert(appdata v) {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
  }

  v2f_jump vert_jump(appdata v) {
    v2f_jump o;
    o.vertex = UnityObjectToClipPos(v.vertex);

    o.uv[0] = v.uv;

    float2 dx = float2(_MainTex_TexelSize.x, 0) * _Step;
    float2 dy = float2(0, _MainTex_TexelSize.y) * _Step;
    o.uv[1] = v.uv + dx;
    o.uv[2] = v.uv - dx;
    o.uv[3] = v.uv + dy;
    o.uv[4] = v.uv - dy;

    o.uv[5] = v.uv + dx + dy;
    o.uv[6] = v.uv + dx - dy;
    o.uv[7] = v.uv - dx + dy;
    o.uv[8] = v.uv - dx - dy;

    return o;
  }

  float screenDist(float2 v) {
    float ratio = _MainTex_TexelSize.x / _MainTex_TexelSize.y;
    return dot(v, v * ratio);
  }

  fixed4 frag_init(v2f i) : SV_Target{
    float4 color = tex2D(_MainTex, i.uv);
    
    float4 result = 0;
    if (color.a < 0.5) {
      result.xy = float2(100, 100);
      result.w = screenDist(result.xy);
    }

    return result;
  }

  void compDist(float dist, float2 xy, inout float currDist, inout float2 currXY) {
    if (dist < currDist) {
      currDist = dist;
      currXY = xy;
    }
  }

  void checkBounds(inout float2 xy, float2 uv) {
    if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) {
      xy = float2(1000, 1000);
    }
  }

  fixed4 frag_jump(v2f_jump i) : SV_Target{
    float4 curr = tex2D(_MainTex, i.uv[0]);
    float currDist = sqrMag(curr.xy);

    for (uint j = 1; j <= 8; j++) {
      float2 n = tex2D(_MainTex, i.uv[j]).xy + (i.uv[j] - i.uv[0]);

      checkBounds(n, i.uv[j]);

      float dist = screenDist(n.xy);

      compDist(dist, n, currDist, curr.xy);
    }

    return float4(curr.xy, currDist, 1);
  }

  fixed4 frag_comp(v2f i) : SV_Target {
    float4 curr = tex2D(_MainTex, i.uv);
    float dist = smoothstep(0.9, 1, sin(100 * sqrt(curr.z)));
    return float4(0, dist, 0, 1);
  }
  ENDCG

	SubShader {
		Cull Off ZWrite Off ZTest Always
    Blend One Zero

    //Pass 0: Init
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_init
			ENDCG
		}

    //Pass 1: Jump
    Pass{
      CGPROGRAM
      #pragma vertex vert_jump
      #pragma fragment frag_jump
      ENDCG
    }

    //Pass 2: Composite
    Pass{
      Blend One One
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag_comp
      ENDCG
    }
	}
}
