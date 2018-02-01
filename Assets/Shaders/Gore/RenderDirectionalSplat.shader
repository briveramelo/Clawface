Shader "Custom/Gore/RenderDirectionalSplat"
{
	Properties
	{
		[Header (Updated Per Splat)]
		[PerRendererData] _Previous ("Previous RenderTexture", 2D) = "black" {}
		[PerRendererData] _PaintMask ("Render Mask", 2D) = "white" {}
		[PerRendererData] _Masks ("Gore Masks (Where to render gore)", 2DArray) = "" {}
		[PerRendererData] _Normals ("Gore Normals (How bumpy it is)", 2DArray) = "" {}
		// _Anchors can't be defined here
		// _Rotations can't be defined here
		// _Positions can't be defined here
		[PerRendererData] _Count ("Count (Number of splats)", Int) = 0

		[Header (Decal Reference Dimensions)]
		_Width ("Decal Reference Width", Float) = 4
		_Height ("Decal Reference Height", Float) = 4
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
			#pragma target 3.5
			
			#include "UnityCG.cginc"
			#include "GoreLib.cginc"

			//// Constants
			static const int MAX_NUM_SPLATS = 10;

			//// Properties
			sampler2D _Previous;
			sampler2D _PaintMask;
			UNITY_DECLARE_TEX2DARRAY(_Masks);
			UNITY_DECLARE_TEX2DARRAY(_Normals);
			float4 _Anchors[MAX_NUM_SPLATS];
			float4 _Rotations[MAX_NUM_SPLATS];
			float4 _Positions[MAX_NUM_SPLATS];
			int _Count;
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

					// Iterate through splats and override color as appropriate
					for (int index = 0; index < _Count; index++) {
						// Project and acquire an unadjusted "uv" value for sampling splat textures
						float3 projected = PlanarProjection(i.worldPos, i.normal, _Positions[index].xyz);
						float3 offset = i.worldPos - projected;
						float2 uv = ConvertToUV(offset, i.tangent, i.bitangent);

						// Transform our uv into the splat uv (that is, do translation and rotation to account)
						// for orientation of our rotated splat
						uv = RotateByJUnitVector(uv, _Rotations[index].xy);
						uv += _Anchors[index].xy;
						uv = AdjustUVByScale(uv, _Width, _Height);

						// Finally, We can check if we can splat with this (and if so, where to sample)
						if (IsValidUV(uv)) {
							float3 compoundUV = float3(uv.x, uv.y, index);
							float4 maskSample = UNITY_SAMPLE_TEX2DARRAY(_Masks, compoundUV);
							float4 normalSample = UNITY_SAMPLE_TEX2DARRAY(_Normals, compoundUV);

							// Apply alpha for BLOOD, keep highest value
							color.a = max(color.a, maskSample.r);

							// Merge normals
							float3 unpacked = (normalSample * 2) - 1;
							color.rgb = MergeNormals(color.rgb, unpacked);
						}
					}
				}

				return color;
			}
			ENDCG
		}
	}
}
