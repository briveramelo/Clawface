/*
	This Library contains helper functions that will be used in shaders
	involved in rendering gore.  It makes a few assumptions about the gore
	in order to render things nicely.
*/

// Common Shader Logic
// ===================

/*
	Appdata_Gore: Structure outlining the required data to successfully splat
	gore.
*/
struct Appdata_Gore
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
};

/*
	V2F_Gore: Structure identifying information REQUIRED to splat blood correctly.
*/
struct V2F_Gore
{
	float2 uv : TEXCOORD;
	float4 vertex : SV_POSITION;
	float3 worldPos : POSITIONT1;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float3 bitangent : BINORMAL;
};

/*
	Vertex_Gore: A common vertex function for all shaders wishing to render gore to
	splat textures.
*/
V2F_Gore Vertex_Gore(Appdata_Gore v)
{
	V2F_Gore o;

	// Map vertices to their positions in "UV space" so we can "render"
	// into a texture that has the same positions as the appropriate UV
	o.vertex = mul(UNITY_MATRIX_VP, float4(v.uv * 2.0F - 1.0F, 0.5, 1));
	o.uv = v.uv;
	o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	o.normal = v.normal;
	o.tangent = v.tangent;
	o.bitangent = cross(v.normal.xyz, v.tangent.xyz) * v.tangent.w;
	return o;
}

// Geometry Helper Functions
// =========================

/*
	PlanarProjection: Projects a point onto a plane defined by a normal and a point on the plane.
		- planePoint      : a point lying in the desired plane
		- normal          : the normal of the desired plane
		- projectionPoint : the point to project into the plane
*/
inline float3 PlanarProjection(float3 planePoint, float3 normal, float3 projectionPoint)
{
	float numerator = dot(normal, planePoint) - dot(normal, projectionPoint);
	float denominator = dot(normal, normal);
	float t = numerator / denominator;
	return projectionPoint + mul(normal, t);
}

/*
	ConvertToUV: Converts a vector3 to a vector2 where:
		vec is the absolute vector
		tangent is a vector pointing along the 'u' axis
		bitangent is a vector pointing along the 'v' axis
		returns a float2 where retVal.x = 'u' and retVal.y = 'v'
*/
inline float2 ConvertToUV(float3 vec, float3 tangent, float3 bitangent)
{
	float u = dot(vec, normalize(tangent));
	float v = dot(vec, normalize(bitangent));
	return float2(u, v);
}

/*
	RotateByJUnitVector: Rotates a uv coordinate into the correct coordinate frame
	by using a transform of the form:
		{ y-component of j' , -x-component of j' }
		{ x-component of j' ,  y-component of j' }
	This formula was extracted from:
		http://www.continuummechanics.org/coordxforms.html (Transformation Matrix Formulation)
*/
inline float2 RotateByJUnitVector(float2 uv, float2 rotation)
{
	float2 normalized = normalize(rotation);
	float2x2 transform = {
		normalized.y, -normalized.x,
		normalized.x, normalized.y
	};
	return mul(transform, uv);
}

/*
	AdjustUVByScale: Adjusts a uv into the "fake" space we create for each splat decal.
	This fake space has the designated width and height below.  Basically, we're converting
	from a position in that space into an actual UV coordinate we can use to sample with.
	(So.. yea.. maybe a bit of a misnomer..)
*/
inline float2 AdjustUVByScale(float2 uv, float width, float height)
{
	float u = (uv.x + width / 2) / width;
	float v = (uv.y + height / 2) / height;
	return float2(u, v);
}

/*
	IsValidUV: Convenience method.  Simply checks that the UV coordinate samples inside the texture.
*/
inline bool IsValidUV(float2 uv)
{
	return uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1;
}

/*
	MergeNormals: Merges two normal values the simple way.
*/
inline float3 MergeNormals(float3 a, float3 b)
{
	return normalize(a + b);
}