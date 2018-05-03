Shader "Hathos/Hidden/80sFilter/BarrelDistortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Distortion ("Distortion Parameter", Range(0, 1)) = 0.015
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
			float _Distortion;
			
			//// Shader Functions
			fixed4 frag (v2f_img input) : SV_Target
			{
			    fixed4 color = tex2D(_MainTex, input.uv);
			
				// Normalize UVs
				float2 uvs = (2.0 * input.uv - 1.0) / 1.0;
				
				// Calculate 12 Norm
				float r = dot(uvs, uvs);
				
				// Calculate deflated or inflated new coordinates
				float2 coord3 = uvs / (1.0 - _Distortion * r);
				float2 coord2 = uvs / (1.0 - _Distortion * dot(coord3, coord3));
				
				// Denormalize to original range
				uvs = (coord2 + 1.0) * 1.0 / 2.0;
				
				// Sample and return
				if (uvs.x >= 0 && uvs.x <= 1 && uvs.y >= 0 && uvs.y <= 1) {
				    return tex2D(_MainTex, uvs);
				} else {
				    return fixed4(0, 0, 0, 1);
				}
			}
			ENDCG
		}
	}
}
