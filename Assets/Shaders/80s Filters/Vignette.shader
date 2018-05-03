Shader "Hathos/Hidden/80sFilter/Vignette"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint ("Color of Vignette", Color) = (0, 0, 0)
		_Distance ("Distance from Center to start Vignette", Range(0, 1)) = 0.8
		_Softness ("Softness of the Vignette", Range(0, 1)) = 0.45
		_Strength ("Strength of Vignette", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

        // Heavily inspired by / sourced from: https://github.com/mattdesl/lwjgl-basics/wiki/ShaderLesson3
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

            //// Variables
			sampler2D _MainTex;
			fixed4 _Tint;
			float _Distance;
			float _Softness;
			float _Strength;
			
			//// Shader Functions
			fixed4 frag (v2f_img input) : SV_Target
			{
			    fixed4 color = tex2D(_MainTex, input.uv);
			
				float adjusted = input.uv - float2(0.5, 0.5);
				float len = length(adjusted);
				float vignette = smoothstep(_Distance, _Distance - _Softness, len);
				
				
				return lerp(color, color * vignette, _Strength);
			}
			ENDCG
		}
	}
}
