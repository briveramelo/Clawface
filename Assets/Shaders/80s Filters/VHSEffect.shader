Shader "Hathos/Hidden/80sFilter/VHSEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RealTime ("The actual real time of the system.", Float) = 0
		_Width ("Scroller Width (in UV space)", Range(0, 1)) = 0.1
		_Speed ("Rate of Scroll (UVs per second)", Float) = 0.1
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
			float _RealTime;
			float _Width;
			float _Speed;
			
			//// Shader Functions
			fixed4 frag (v2f_img input) : SV_Target
			{			    
			    float total = 1 + _Width; 
			    float progress = (_RealTime * _Speed) % 1;
			    float bar = total - total * progress;
			    float target = ((bar) + (bar - _Width)) / 2;
			    float v = input.uv.y;
			    float cmp = v;
			    
			    #if UNITY_UV_STARTS_AT_TOP
			        cmp = 1 - cmp;
			        target = 1 - target;
			    #endif
			    
			    if (cmp <= bar && cmp > bar - _Width) {
			        return tex2D(_MainTex, float2(input.uv.x, target));
			    } else {
			        return tex2D(_MainTex, input.uv);
			    }
			}
			ENDCG
		}
	}
}
