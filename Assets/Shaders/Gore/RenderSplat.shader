Shader "Custom/Gore/RenderSplat"
{
	Properties
	{
		[Header (Updated Per Splat)]
		[PerRendererData] _Previous ("Previous RenderTexture", 2D) = "black" {}
		[PerRendererData] _SplatLocation ("Splat Location (World Space)", Vector) = (0,0,0,0)
		[PerRendererData] _OriginalNormal ("Splat Location Normal (World Space)", Vector) = (0,0,0,0)
		[PerRendererData] _Decal("Blood Decal", 2D) = "black" {}

		[Header (Blood Style)]
		_Color ("Blood Color", Color) = (1,0,0,1)

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
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			//// Structs
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float2 uv : TEXCOORD;
				float4 vertex : SV_POSITION;
				float3 worldPos : POSITIONT1;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float3 bitangent : BINORMAL;
			};

			//// Properties
			sampler2D _Previous;
			float4 _SplatLocation;
			float4 _OriginalNormal;
			sampler2D _Decal;
			float4 _Color;
			float _Width;
			float _Height;

			//// Helper Functions
			float3 planarProjection(float3 normal, float3 planePoint, float3 projectionPoint) {
				float numerator = dot(normal, planePoint) - dot(normal, projectionPoint);
				float denominator = dot(normal, normal);
				float t = numerator / denominator;
				return projectionPoint + mul(normal, t);
			}

			float3 calculateTriangleLeg(float3 legA, float3 legC) {
				float3 legBSquared = mul(legC, legC) - mul(legA, legA);
				return sqrt(legBSquared);
			}
			
			//// Vertex / Fragment
			v2f vert (appdata v)
			{
				v2f o;
				// Map vertices to their positions in "UV space" so we can "render"
				// into a texture that has the same positions as the appropriate UV
				o.vertex = mul( UNITY_MATRIX_VP, float4(v.uv * 2.0F - 1.0F, 0.5, 1));
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.normal = v.normal;
				o.tangent = v.tangent;
				o.bitangent = cross(v.normal.xyz, v.tangent.xyz) * v.tangent.w;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				// First, project our splat location onto the plane this fragment lies in
				float3 projected = planarProjection(i.normal, i.worldPos, _SplatLocation);
				
				// Calculate the Offset from the decal center of the point being rendered
				float3 offsetVector = i.worldPos - projected;

				// Use the reference dimensions to select an a color
				float tangent = dot(offsetVector, i.tangent / length(i.tangent));
				float bitangent = dot(offsetVector, i.bitangent / length(i.bitangent));
				float4 color = tex2D(_Previous, i.uv);

				if ((tangent <= _Width / 2 && tangent >= -_Width / 2 &&
					bitangent <= _Height / 2 && bitangent >= -_Height / 2)) {
					float u = (tangent + _Width / 2) / _Width;
					float v = (bitangent + _Height / 2) / _Height;
					float4 decalSample = tex2D(_Decal, float2(u, v));
					if (decalSample.a != 0) {
						color = _Color;
					}
				}

				return color;
			}
			ENDCG
		}
	}
}
