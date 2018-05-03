Shader "Hathos/Hidden/80sFilter/Scaler"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale of Image", Float) = 1.0
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
			float _Scale;
			
			//// Shader Functions
			fixed4 frag (v2f_img input) : SV_Target
			{
			    float2 offcenter = input.uv - float2(0.5, 0.5);
			    float2 scaled = offcenter * _Scale;
			    float2 recentered = scaled + float2(0.5, 0.5);
			    
			    return tex2D(_MainTex, recentered);
			}
			ENDCG
		}
	}
}
