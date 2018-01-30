// Upgrade NOTE: upgraded instancing buffer 'PBROutlinePlayer' to new syntax.
// Upgrade NOTE: upgraded instancing buffer 'PBROutlinePlayer1stPassv2' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Diffuse Outline"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Header (Outline)]
		_ASEOutlineWidth( "Outline Width", Float ) = 0.04
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)

		[Header (Model)]
		_Diffuse("Diffuse", 2D) = "white" {}
		_DiffuseTint("Diffuse Tint", Color) = (1,1,1,0)
		_Normal("Normal", 2D) = "white" {}
		_Metallic("Metallic", Range(0.0, 1.0)) = 0.5
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
		_Emission("Emission", 2D) = "black" {}

		[Header (Hit Feedback)]
		_HitColor("Hit Color", Color) = (0,0,0,0)
		_HitColorStrength("Hit Color Strength", Range( 0 , 1)) = 0

		[Header (XRay)]
		_XrayColor("Xray Color", Color) = (0,0,0,0)
		_XrayColorStrength("Xray Color Strength", Range( 0 , 5)) = 1
	}

	SubShader
	{
		// OUTLINE
		Tags{}
		Cull Front
		ZWrite Off
		Ztest Always
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:outlineVertexDataFunc
		#pragma multi_compile_instancing
		struct Input
		{
			fixed filler;
		};
		UNITY_INSTANCING_BUFFER_START(PBROutlinePlayer)
			UNITY_DEFINE_INSTANCED_PROP( fixed4, _ASEOutlineColor )
#define _ASEOutlineColor_arr PBROutlinePlayer
			UNITY_DEFINE_INSTANCED_PROP(fixed, _ASEOutlineWidth)
#define _ASEOutlineWidth_arr PBROutlinePlayer
		UNITY_INSTANCING_BUFFER_END(PBROutlinePlayer)
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * UNITY_ACCESS_INSTANCED_PROP( _ASEOutlineWidth_arr, _ASEOutlineWidth ) );
		}
		inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = UNITY_ACCESS_INSTANCED_PROP( _ASEOutlineColor_arr, _ASEOutlineColor ).rgb; o.Alpha = 1; }
		ENDCG

		// XRay
		Cull Back
		ZWrite Off
		ZTest Greater
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv2_texcoord2;
		};

		uniform float4 _XrayColor;
		uniform float _XrayColorStrength;

		UNITY_INSTANCING_BUFFER_START(PBROutlinePlayer1stPassv2)
			UNITY_DEFINE_INSTANCED_PROP(float4, _HitColor)
#define _HitColor_arr PBROutlinePlayer1stPassv2
			UNITY_DEFINE_INSTANCED_PROP(float, _HitColorStrength)
#define _HitColorStrength_arr PBROutlinePlayer1stPassv2
		UNITY_INSTANCING_BUFFER_END(PBROutlinePlayer1stPassv2)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _HitColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_HitColor_arr, _HitColor);
			float _HitColorStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(_HitColorStrength_arr, _HitColorStrength);
			float4 lerpResult77 = lerp( ( ( _XrayColor * _XrayColorStrength ) ) , _HitColor_Instance , _HitColorStrength_Instance);
			o.Albedo = lerpResult77;
			o.Alpha = 1;
		}

		ENDCG
	
		// MODEL
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest Less
		CGPROGRAM
		
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows

		#include "UnityCG.cginc"

		struct Input
		{
			float2 uv_Diffuse;
		};

		uniform sampler2D _Normal;
		float4 _Normal_ST;
		uniform sampler2D _Diffuse;
		uniform float4 _DiffuseTint;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _Emission;
		uniform sampler2D _Emission_ST;

		UNITY_INSTANCING_BUFFER_START(PBROutlinePlayer)
			UNITY_DEFINE_INSTANCED_PROP(float4, _HitColor)
#define _HitColor_arr PBROutlinePlayer
			UNITY_DEFINE_INSTANCED_PROP(float, _HitColorStrength)
#define _HitColorStrength_arr PBROutlinePlayer
		UNITY_INSTANCING_BUFFER_END(PBROutlinePlayer)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = UnpackNormal( tex2D( _Normal, i.uv_Diffuse ) );
			float4 _HitColor_Instance = UNITY_ACCESS_INSTANCED_PROP(_HitColor_arr, _HitColor);
			float _HitColorStrength_Instance = UNITY_ACCESS_INSTANCED_PROP(_HitColorStrength_arr, _HitColorStrength);
			float4 color = lerp( ( tex2D( _Diffuse, i.uv_Diffuse) * _DiffuseTint ) , _HitColor_Instance , _HitColorStrength_Instance);
			o.Albedo = color.rgb;
			o.Emission = tex2D(_Emission, i.uv_Diffuse);
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Occlusion = 1.0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}