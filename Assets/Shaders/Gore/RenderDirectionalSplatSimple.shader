Shader "Custom/Gore/RenderDirectionalSplatSimple"
{
	Properties
	{
		[Header (Updated Per Splat)]
		[PerRendererData] _Previous ("Previous RenderTexture", 2D) = "black" {}
		[PerRendererData] _PaintMask ("Render Mask", 2D) = "white" {}
		[PerRendererData] _Masks ("Gore Mask (Where to render gore)", 2D) = "black" {}
		[PerRendererData] _Normals ("Gore Normal (How bumpy it is)", 2D) = "black" {}
		[PerRendererData] _Anchor ("Splat Anchor", Vector) = (0,0,0,0)
		[PerRendererData] _Rotation ("Splat Rotation", Vector) = (0,0,0,0)
		[PerRendererData] _Position ("Splat Position", Vector) = (0,0,0,0)

		[Header (Decal Reference Dimensions)]
		_Width ("Decal Reference Width", Float) = 8
		_Height ("Decal Reference Height", Float) = 8
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex Vertex_Gore // Acquired from GoreLib.cginc
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "GoreLib.cginc"

			//// Properties
			sampler2D _Previous;
			sampler2D _PaintMask;
			sampler2D _Mask;
			sampler2D _Normal;
			float4 _Anchor;
			float4 _Rotation;
			float4 _Position;

			float _Width;
			float _Height;
			
			//// Shader Functions
			
			fixed4 frag (V2F_Gore i) : SV_Target
			{
				//// Acquire Color
				//// Color packs two values: rgb->normal, a->percentage splat (0 = no blood, 1 = full blood)
				// Use previous color
				float4 color = tex2D(_Previous, i.uv);

				// Only render new blood if paint mask is white.
				if (IsWhite(tex2D(_PaintMask, i.uv).rgb)) {
					// Project and acquire an unadjusted "uv" value for sampling splat textures
					float3 projected = PlanarProjection(i.worldPos, i.normal, _Position.xyz);
					float3 offset = i.worldPos - projected;
					float2 uv = ConvertToUV(offset, i.tangent, i.bitangent);

					// Transform our uv into the splat uv (that is, do translation and rotation to account)
					// for orientation of our rotated splat
					uv = RotateByJUnitVector(uv, _Rotation.xy);
					uv += _Anchor.xy;
					uv = AdjustUVByScale(uv, _Width, _Height);

					// Finally, We can check if we can splat with this (and if so, where to sample)
					if (IsValidUV(uv)) {
						float4 maskSample = tex2D(_Mask, uv);
						float4 normalSample = tex2D(_Normal, uv);

						// Apply alpha for BLOOD, keep highest value
						color.a = max(color.a, maskSample.r);

						// Merge normals
						float3 unpacked = UnpackNormal(normalSample).xyz;
						color.rgb = MergeNormals(color.rgb, unpacked);
					}
				}

				return color;
			}
			ENDCG
		}
	}
}
