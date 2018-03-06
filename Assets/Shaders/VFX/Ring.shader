Shader "Unlit/Ring"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RingWidth("Ring Width", Range(0.1, 100)) = 1.0
		_Radius("Radius", Range (1, 100)) = 10
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

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
				float4 vertex : SV_POSITION;
				float4 worldVertex : POSITION1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _RingWidth;
			float4 _Color;
			//float4 _RingCenter;
			//float _Radius;
			UNITY_INSTANCING_CBUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _RingCenter)
				UNITY_DEFINE_INSTANCED_PROP(fixed, _Radius)
			UNITY_INSTANCING_CBUFFER_END
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(v.vertex, unity_ObjectToWorld);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 circleCenter = mul(float4(0.0, 0.0, 0.0, 0.0), unity_ObjectToWorld).xz;
				float r = UNITY_ACCESS_INSTANCED_PROP(_Radius);
				float4 color = UNITY_ACCESS_INSTANCED_PROP(_Color);
				float halfRingWidth = _RingWidth / 2.0;

				float distFromCenter = distance(i.worldVertex.xz, circleCenter);
				if (distFromCenter < r - halfRingWidth) return fixed4(0.0, 0.0, 0.0, 0.0);
				if (distFromCenter > r + halfRingWidth) return fixed4(0.0, 0.0, 0.0, 0.0);

				float lookupXCoord = (distFromCenter - (r - halfRingWidth)) / _RingWidth;

				float4 c = tex2D(_MainTex, lookupXCoord * _MainTex_ST.xy) * color;
				return c;
			}
			ENDCG
		}
	}
}
