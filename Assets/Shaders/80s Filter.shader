Shader "Hathos/80s Filter"
{
	Properties
	{
		[PerRendererData] _SourceImage ("Texture", 2D) = "black" {}

		[Header(Barrel Distortion)]
		[Toggle(BD_ENABLED)] _BD_Enabled ("Enable Barrel Distortion", Float) = 0
		_BD_Horizontal ("Horizontal Distortion Parameter", Range(0, 1)) = 0.015
		_BD_Vertical ("Vertical Distortion Parameter", Range(0, 1)) = 0.015

		[Header(VHS Scroller)]
		[Toggle(VHSS_ENABLED)] _VHSS_Enabled ("Enable VHS Scroller", Float) = 0
		_VHSS_Width ("Scroller Width (in UV space)", Range(0, 1)) = 0.1
		_VHSS_Speed ("Rate of Scroll (UVs per second)", Float) = 0.1

		[Header(Simple Film Grain)]
		[Toggle(SFG_ENABLED)] _SFG_Enabled ("Enable Simple Film Grain", Float) = 0
		_SFG_Tint ("Color of Noise", Color) = (.2,.2,.2)
		_SFG_Weight ("Noise Strength", Range(0, 1)) = 0.2
		_SFG_Scale ("Noise Scale", Float) = 10

		[Header(Scanlines)]
		[Toggle(SL_ENABLED)] _SL_Enabled ("Enable Scanlines", Float) = 0
		_SL_Width ("Scanline Width (in UV space)", Range(0, 1)) = 0.05
		_SL_Frequency ("Frequency of Scanlines (in UV space)", Range(0, 1)) = 0.01
		_SL_Tint ("Color of Scanlines", Color) = (.2,.2,.2)
		_SL_Weight ("Weight of Scanlines", Range(0, 1)) = 0.2
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
			#pragma shader_feature BD_ENABLED
			#pragma shader_feature SFG_ENABLED
			#pragma shader_feature VHSS_ENABLED
			#pragma shader_feature SL_ENABLED
			
			#include "UnityCG.cginc"
			#include "Simplex.cginc"

			//// Variables
			sampler2D _SourceImage;
			float4 _SourceImage_TexelSize;

			float _BD_Enabled;
			float _BD_Horizontal;
			float _BD_Vertical;

			float _SFG_Enabled;
			fixed4 _SFG_Tint;
			float _SFG_Weight;
			float _SFG_Scale;

			float _VHSS_Enabled;
			float _VHSS_Width;
			float _VHSS_Speed;

			float _SL_Enabled;
			float _SL_Width;
			float _SL_Frequency;
			fixed4 _SL_Tint;
			float _SL_Weight;

			//// Helper Functions
			// Sourced from: http://marcodiiga.github.io/radial-lens-undistortion-filtering
			fixed4 applyBD(fixed4 color, v2f_img input) {
				// Normalize UVs
				float x = (2.0 * input.uv.x - 1.0) / 1.0;
				float y = (2.0 * input.uv.y - 1.0) / 1.0;

				// Calculate 12 norm
				float r = x*x + y*y;

				// Calculate defalted or inflated new coordinates
				float x3 = x / (1.0 - _BD_Horizontal * r);
				float y3 = y / (1.0 - _BD_Vertical * r);
				float x2 = x / (1.0 - _BD_Horizontal * (x3 * x3 + y3 * y3));
				float y2 = y / (1.0 - _BD_Vertical * (x3 * x3 + y3 * y3));

				// Denormalize to original range
				float u = (x2 + 1.0) * 1.0 / 2.0;
				float v = (y2 + 1.0) * 1.0 / 2.0;

				// Sample and return
				return tex2D(_SourceImage, float2(u, v));
			}

			fixed4 applySFG(fixed4 color, v2f_img input) {

				float noise = (snoise(input.uv * _SFG_Scale * _Time.w) + 1) / 2;
				return lerp(color, _SFG_Tint, noise * _SFG_Weight);
			}

			fixed4 applyVHSS(fixed4 color, v2f_img input) {
				float total = 1 + _VHSS_Width;
				float bar = (total - _Time.y * _VHSS_Speed) % total;
				float target = ((bar) + (bar - _VHSS_Width)) / 2;
				float v = input.uv.y;
				float cmp = v;

				#if UNITY_UV_STARTS_AT_TOP
					cmp = 1 - cmp;
					target = 1 - target;
				#endif

				if (cmp <= bar && cmp > bar - _VHSS_Width) {
					return tex2D(_SourceImage, float2(input.uv.x, target));
				}
				else {
					return color;
				}
			}

			fixed4 applySL(fixed4 color, v2f_img input) {
				float total = _SL_Width + _SL_Frequency;
				float v = input.uv.y;
				float cmp = v % total;

				if (cmp <= _SL_Width) {
					return lerp(color, _SL_Tint, _SL_Weight);
				}
				else {
					return color;
				}
			}
			
			//// Shader
			fixed4 frag (v2f_img input) : SV_Target
			{
				fixed4 color = tex2D(_SourceImage, input.uv);

				#ifdef BD_ENABLED
				color = applyBD(color, input);
				#endif

				#ifdef VHSS_ENABLED
				color = applyVHSS(color, input);
				#endif

				#ifdef SFG_ENABLED
				color = applySFG(color, input);
				#endif

				#ifdef SL_ENABLED
				color = applySL(color, input);
				#endif

				return color;
			}
			ENDCG
		}
	}
}
