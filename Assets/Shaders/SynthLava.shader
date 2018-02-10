Shader "Unlit/SynthLava"
{
	Properties
	{
		_ColorMask("Color Mask", 2D) = "white" {}
		_ColorMaskAdd("Color Mask Add", 2D) = "black" {}
		_ColorMaskAddStrength("Color Mask Add Strength", Range(0.0, 1.0)) = 1.0
		_ColorMaskAddScrollSpeed("Color Mask Add Scroll Speed", Range (0.0, 10.0)) = 1.0
		_ColorTex("Color", 2D) = "white" {}
		_ColorScrollSpeed("Color Scroll Speed", Range(0.0, 100.0)) = 1.0
		_ColorScrollDirection ("Color Scroll Direction", Range(0.0, 6.28)) = 0.0
		_ColorVibrance("Color Vibrance", Range(0.0, 1.0)) = 1.0
		_NoiseStrength("Noise Strength", Range(0.0, 100.0)) = 1.0
		_NoiseScrollSpeed("Noise Scroll Speed", Range(0.01, 100.0)) = 1.0
		_NoiseTex("Noise", 2D) = "black" {}
		_NoisePower("Noise Power", Range(1.0, 5.0)) = 1.0
		_FadeDist("FadeDist", Range(1, 1000)) = 100
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
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
				float2 addMaskUV : TEXCOORD2;
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _ColorMask;
			float4 _ColorMask_ST;
			sampler2D _ColorMaskAdd;
			float4 _ColorMaskAdd_ST;
			float _ColorMaskAddStrength;
			float _ColorMaskAddScrollSpeed;
			float _ColorScrollSpeed;
			float _ColorVibrance;
			float _ColorScrollDirection;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _ColorTex;
			float4 _ColorTex_ST;
			float _NoiseScrollSpeed;
			float _NoiseStrength;
			float _NoisePower;
			float _FadeDist;
			
			v2f vert (appdata v)
			{
				v2f o;
				fixed noiseSample = tex2Dlod(_NoiseTex, float4(TRANSFORM_TEX(v.uv.xy, _NoiseTex) * _Time.y * _NoiseScrollSpeed, 0, 0)).r;
				noiseSample = pow(noiseSample, _NoisePower);
				o.vertex = UnityObjectToClipPos(v.vertex) + fixed4(0.0, -noiseSample * _NoiseStrength, 0.0, 0.0);
				o.uv = TRANSFORM_TEX(v.uv, _ColorMask);
				float x = cos(_ColorScrollDirection);
				float y = sin(_ColorScrollDirection);
				o.colorUV = TRANSFORM_TEX(o.uv, _ColorTex) + float2(x, y) * _Time.y * _ColorScrollSpeed;
				float3 worldPos = mul(v.vertex, unity_ObjectToWorld).xyz;
				float d = distance(_WorldSpaceCameraPos, worldPos);
				o.color = (1.0 - clamp(d / _FadeDist, 0.0, 1.0)) * float4(1.0, 1.0, 1.0, 1.0);
				o.addMaskUV = TRANSFORM_TEX(v.uv, _ColorMaskAdd) + float2(x, y) * _Time.y * _ColorMaskAddScrollSpeed;
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed maskVal = (tex2D(_ColorMask, i.uv).r + _ColorMaskAddStrength * tex2D(_ColorMaskAdd, i.addMaskUV).r) * _ColorVibrance;
				fixed4 c = tex2D(_ColorTex, i.colorUV);
				c.a = clamp(pow (i.color.a + 0.001, 0.9), 0.0, 1.0);
				c.xyz *= maskVal;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG
		}
	}
}
