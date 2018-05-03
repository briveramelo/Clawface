Shader "Hathos/Hidden/80sFilter/Scanlines"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Width ("Scanline Width (in UV space)", Range(0, 1)) = 0.05
		_Frequency ("Frequency of Scanlines (in UV space)", Range(0, 1)) = 0.01
		_Tint ("Color of Scanlines", Color) = (.2,.2,.2)
		_Weight ("Weight of Scanlines", Range(0, 1)) = 0.2
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

            //// Variables
			sampler2D _MainTex;
			float _Width;
			float _Frequency;
			fixed4 _Tint;
			float _Weight;
			
			//// Shader Functions
			fixed4 frag (v2f_img input) : SV_Target
			{
			    fixed4 color = tex2D(_MainTex, input.uv);
			
				float total = _Width + _Frequency;
				float v = input.uv.y;
				float cmp = v % total;
				
				if (cmp <= _Width) {
				    return lerp(color, _Tint, _Weight);
				} else {
				    return color;
				}
			}
			ENDCG
		}
	}
}
