// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlinedStandardBasic" {
	Properties{
		_Albedo("Albedo", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_Normal("Normal", 2D) = "normal" {}
		_Emission("Emission", 2D) = "black" {}
		_Metallic("Metallic", 2D) = "black" {}
		_Occlusion("Occlusion", 2D) = "black" {}
		_Outline("Outline", Range(0,0.1)) = 0
		_OutlineColor("Color", Color) = (1, 1, 1, 1)
	}
	SubShader{
		Pass{
			Tags{ "RenderType" = "Opaque" }
			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
			};

			float _Outline;
			float4 _OutlineColor;

			float4 vert(appdata_base v) : SV_POSITION{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 normal = mul((float3x3) UNITY_MATRIX_MV, v.normal);
				normal.x *= UNITY_MATRIX_P[0][0];
				normal.y *= UNITY_MATRIX_P[1][1];
				o.pos.xy += normal.xy * _Outline;
				return o.pos;
			}

			half4 frag(v2f i) : COLOR{
				return _OutlineColor;
			}

				ENDCG
		}

			CGPROGRAM
//#pragma surface surf Lambert
#pragma surface surf Standard

			sampler2D _Albedo;
			sampler2D _Normal;
			fixed4 _Tint;

	struct Input {
		float2 uv_Albedo;
	};

	void surf(Input IN, inout SurfaceOutputStandard o) {
		o.Albedo = tex2D(_Albedo, IN.uv_Albedo) * _Tint;
		o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Albedo));
	}

	ENDCG
	}
		FallBack "Diffuse"
}