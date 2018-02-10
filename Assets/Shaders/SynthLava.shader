Shader "Unlit/SynthLava"
{
	Properties
	{
		_ColorMask("Color Mask", 2D) = "white" {}
		_ColorTex("Color", 2D) = "white" {}
		_ColorScrollSpeed("Color Scroll Speed", Range(0.0, 100.0)) = 1.0
		_ColorScrollDirection ("Color Scroll Direction", Range(0.0, 6.28)) = 0.0
		_ColorVibrance("Color Vibrance", Range(0.0, 1.0)) = 1.0
		_NoiseStrength("Noise Strength", Range(0.0, 100.0)) = 1.0
		_NoiseScrollSpeed("Noise Scroll Speed", Range(0.01, 100.0)) = 1.0
		_NoiseTex("Noise", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

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
				float2 colorUV : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _ColorMask;
			float4 _ColorMask_ST;
			float _ColorScrollSpeed;
			float _ColorVibrance;
			float _ColorScrollDirection;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _ColorTex;
			float4 _ColorTex_ST;
			float _NoiseScrollSpeed;
			float _NoiseStrength;
			
			v2f vert (appdata v)
			{
				v2f o;
				fixed noiseSample = tex2Dlod(_NoiseTex, float4(TRANSFORM_TEX(v.uv.xy, _NoiseTex) * _Time.y * _NoiseScrollSpeed, 0, 0));
				o.vertex = UnityObjectToClipPos(v.vertex) + fixed4(0.0, noiseSample * _NoiseStrength, 0.0, 0.0);
				o.uv = TRANSFORM_TEX(v.uv, _ColorMask);
				float x = cos(_ColorScrollDirection);
				float y = sin(_ColorScrollDirection);
				o.colorUV = TRANSFORM_TEX(o.uv, _ColorTex) + float2(x, y) * _Time.y * _ColorScrollSpeed;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed maskVal = tex2D(_ColorMask, i.uv).r * _ColorVibrance;
				fixed4 c = tex2D(_ColorTex, i.colorUV);
				float cameraDist = dist(_WorldSpace)
				c.a = 1.0;
				c.xyz *= maskVal;
				return c;
			}
			ENDCG
		}
	}
}
