Shader "Hathos/Hidden/80sFilter/FilmGrain"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RealTime ("The actual real time of the system.", Float) = 0
		_Tint ("Color of Noise", Color) = (.2,.2,.2)
		_Weight ("Noise Strength", Range(0, 1)) = 0.2
		_Scale ("Noise Scale", Float) = 10
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			#include "Simplex.cginc"

            //// Variables
			sampler2D _MainTex;
			float _RealTime;
			fixed4 _Tint;
			float _Weight;
			float _Scale;
			
			//// Shader Functions
			fixed4 frag (v2f_img input) : SV_Target
			{
			    fixed4 color = tex2D(_MainTex, input.uv);
			
			    float2 movingSample = input.uv + _RealTime;
			    float rawNoise = snoise(movingSample * _Scale); 
				float noise = (rawNoise + 1) / 2;
				return lerp(color, _Tint, noise * _Weight);
			}
			ENDCG
		}
	}
}
