Shader "Unlit/Ring"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RingWidth("Ring Width", Range(0.1, 100)) = 1.0
		_Radius("Radius", Range (1, 100)) = 10
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

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
			float4 _RingCenter;
			float _RingWidth;
			float _Radius;
			
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

				float distFromCenter = distance(i.worldVertex.xz, circleCenter);
				if (distFromCenter < _Radius) return fixed4(0.0, 0.0, 0.0, 0.0);
				if (distFromCenter > _Radius + _RingWidth) return fixed4(0.0, 0.0, 0.0, 0.0);

				float lookupXCoord = (distFromCenter - _Radius) / _RingWidth;

				float4 c = tex2D(_MainTex, lookupXCoord * _MainTex_ST.xy);
				return c;
			}
			ENDCG
		}
	}
}
