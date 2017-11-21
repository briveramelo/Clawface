// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water Projectile"
{
	Properties
	{
		[Header(Refraction)]
		_InnerOuterColorStrength("Inner/Outer Color Strength", Range( 1 , 10)) = 2.726571
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_WaterInner("Water Inner", Color) = (0.4392157,0.7294118,1,1)
		_WaterOuter("Water Outer", Color) = (0,0.2980392,0.6980392,1)
		_Refaction("Refaction", Range( 0 , 2)) = 2
		_Transparency("Transparency", Range( 0 , 1)) = 0.5
		_DetailTexture("Detail Texture", 2D) = "white" {}
		_DetailStrength("Detail Strength", Float) = 0
		_DetailSpeed("Detail Speed", Float) = 0.23
		_WaterNormal("Water Normal", 2D) = "white" {}
		_WaterNormalTiling("Water Normal Tiling", Float) = 1
		_NormalSpeed("Normal Speed", Float) = 0.5
		_DetailTiling("Detail Tiling", Float) = 1
		_NormalStrength("Normal Strength", Range( 0 , 10)) = 1
		_OffsetTexture("Offset Texture", 2D) = "white" {}
		_OffsetMask("Offset Mask", 2D) = "white" {}
		_OffsetTiling("Offset Tiling", Float) = 1
		_OffestSpeed("Offest Speed", Range( 0 , 2)) = 0.23
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard keepalpha finalcolor:RefractionF noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
			float2 texcoord_2;
		};

		uniform sampler2D _WaterNormal;
		uniform float _WaterNormalTiling;
		uniform float _NormalSpeed;
		uniform float _NormalStrength;
		uniform sampler2D _DetailTexture;
		uniform float _DetailTiling;
		uniform float _DetailSpeed;
		uniform float _DetailStrength;
		uniform float4 _WaterInner;
		uniform float4 _WaterOuter;
		uniform float _InnerOuterColorStrength;
		uniform float _Transparency;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float _Refaction;
		uniform sampler2D _OffsetTexture;
		uniform float _OffsetTiling;
		uniform float _OffestSpeed;
		uniform sampler2D _OffsetMask;
		uniform float4 _OffsetMask_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_WaterNormalTiling).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
			float2 temp_cast_1 = (_DetailTiling).xx;
			o.texcoord_1.xy = v.texcoord.xy * temp_cast_1 + float2( 0,0 );
			float2 temp_cast_2 = (_OffsetTiling).xx;
			o.texcoord_2.xy = v.texcoord.xy * temp_cast_2 + float2( 0,0 );
			float4 appendResult90 = (float4(-1.0 , 0.0 , 0.0 , 0.0));
			float mulTime91 = _Time.y * _OffestSpeed;
			float4 temp_cast_5 = (tex2Dlod( _OffsetTexture, float4( ( float4( o.texcoord_2, 0.0 , 0.0 ) + ( appendResult90 * mulTime91 ) ).xy, 0, 0.0) ).b).xxxx;
			float2 uv_OffsetMask = v.texcoord * _OffsetMask_ST.xy + _OffsetMask_ST.zw;
			float4 appendResult101 = (float4(0.0 , ( temp_cast_5 - tex2Dlod( _OffsetMask, float4( uv_OffsetMask, 0, 0.0) ) ).r , 0.0 , 0.0));
			v.vertex.xyz += appendResult101.xyz;
			v.normal = appendResult101.xyz;
		}

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
				color.rgb = color.rgb + Refraction( i, o, _Refaction, _ChromaticAberration ) * ( 1 - color.a );
				color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult39 = (float4(-1.0 , 0.0 , 0.0 , 0.0));
			float mulTime35 = _Time.y * _NormalSpeed;
			o.Normal = ( tex2D( _WaterNormal, ( float4( i.texcoord_0, 0.0 , 0.0 ) + ( appendResult39 * mulTime35 ) ).xy ) * _NormalStrength ).rgb;
			float4 appendResult44 = (float4(-1.0 , 0.0 , 0.0 , 0.0));
			float mulTime45 = _Time.y * _DetailSpeed;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNDotV4 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode4 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNDotV4, _InnerOuterColorStrength ) );
			float4 lerpResult11 = lerp( _WaterInner , _WaterOuter , fresnelNode4);
			o.Emission = ( ( tex2D( _DetailTexture, ( float4( i.texcoord_1, 0.0 , 0.0 ) + ( appendResult44 * mulTime45 ) ).xy ) / _DetailStrength ) + lerpResult11 ).rgb;
			o.Alpha = _Transparency;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13401
1927;69;1767;1004;1944.623;759.826;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;41;-1315.257,-540.1578;Float;False;Constant;_Float4;Float 4;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;43;-1313.174,-623.1328;Float;False;Constant;_Float6;Float 6;3;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;42;-1345.69,-404.3436;Float;False;Property;_DetailSpeed;Detail Speed;9;0;0.23;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;89;-1861.518,962.6497;Float;False;Constant;_Float5;Float 5;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;87;-1891.952,1098.464;Float;False;Property;_OffestSpeed;Offest Speed;18;0;0.23;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;88;-1859.435,879.6749;Float;False;Constant;_Float3;Float 3;3;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-1984.733,425.7034;Float;False;Constant;_Float2;Float 2;3;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;-1038.364,-689.7866;Float;False;Property;_DetailTiling;Detail Tiling;13;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;90;-1665.045,921.3928;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleTimeNode;91;-1624.993,1090.753;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;33;-2007.329,651.7482;Float;False;Property;_NormalSpeed;Normal Speed;12;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;45;-1078.733,-412.0552;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;44;-1118.785,-581.415;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;32;-1986.816,508.6788;Float;False;Constant;_Float0;Float 0;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;92;-1584.624,813.0215;Float;False;Property;_OffsetTiling;Offset Tiling;17;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-1354.108,873.4694;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-803.0336,-400.4117;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-807.8482,-629.3385;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1785.074,417.3572;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleTimeNode;35;-1773.372,653.957;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;-1924.23,228.7919;Float;False;Property;_WaterNormalTiling;Water Normal Tiling;11;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-1349.293,1102.396;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-1133.554,1054.195;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;1;-1221.931,203.351;Float;False;Property;_InnerOuterColorStrength;Inner/Outer Color Strength;2;0;2.726571;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-1546.934,326.5823;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1505.576,541.7574;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-587.2949,-448.6126;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;96;-975.1249,859.3635;Float;True;Property;_OffsetTexture;Offset Texture;15;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1251.428,491.2466;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;97;-963.614,1072.858;Float;True;Property;_OffsetMask;Offset Mask;16;0;Assets/Textures/VFX/Geyser Offest Mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FresnelNode;4;-921.2106,181.8515;Float;True;World;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;56;-266.5009,-67.06032;Float;False;Property;_DetailStrength;Detail Strength;8;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-1091.155,-77.92237;Float;False;Property;_WaterOuter;Water Outer;4;0;0,0.2980392,0.6980392,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;49;-478.6067,-305.4339;Float;True;Property;_DetailTexture;Detail Texture;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-1078.394,-275.7315;Float;False;Property;_WaterInner;Water Inner;3;0;0.4392157,0.7294118,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;53;-293.0797,229.4679;Float;False;Property;_NormalStrength;Normal Strength;14;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;13;-996.3755,566.9175;Float;True;Property;_WaterNormal;Water Normal;10;0;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;54;-50.42981,-196.7029;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;11;-538.3892,-42.15533;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleSubtractOpNode;99;-401.7564,555.2712;Float;True;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.DynamicAppendNode;101;-107.7633,471.1435;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;8;-272.9961,346.7105;Float;False;Property;_Transparency;Transparency;6;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;18.19208,192.9935;Float;False;Property;_Refaction;Refaction;5;0;2;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;3.860638,76.33752;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;50;101.9509,-67.44887;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;307.7756,13.95884;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Water Projectile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Custom;0.92;True;False;0;True;TransparentCutout;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;90;0;88;0
WireConnection;90;1;89;0
WireConnection;91;0;87;0
WireConnection;45;0;42;0
WireConnection;44;0;43;0
WireConnection;44;1;41;0
WireConnection;94;0;92;0
WireConnection;46;0;44;0
WireConnection;46;1;45;0
WireConnection;47;0;51;0
WireConnection;39;0;34;0
WireConnection;39;1;32;0
WireConnection;35;0;33;0
WireConnection;93;0;90;0
WireConnection;93;1;91;0
WireConnection;95;0;94;0
WireConnection;95;1;93;0
WireConnection;36;0;40;0
WireConnection;37;0;39;0
WireConnection;37;1;35;0
WireConnection;48;0;47;0
WireConnection;48;1;46;0
WireConnection;96;1;95;0
WireConnection;38;0;36;0
WireConnection;38;1;37;0
WireConnection;4;3;1;0
WireConnection;49;1;48;0
WireConnection;13;1;38;0
WireConnection;54;0;49;0
WireConnection;54;1;56;0
WireConnection;11;0;3;0
WireConnection;11;1;2;0
WireConnection;11;2;4;0
WireConnection;99;0;96;3
WireConnection;99;1;97;0
WireConnection;101;1;99;0
WireConnection;52;0;13;0
WireConnection;52;1;53;0
WireConnection;50;0;54;0
WireConnection;50;1;11;0
WireConnection;0;1;52;0
WireConnection;0;2;50;0
WireConnection;0;8;7;0
WireConnection;0;9;8;0
WireConnection;0;11;101;0
WireConnection;0;12;101;0
ASEEND*/
//CHKSM=58EDA76CAFDE6C3DE7201A072BDF92C16C23D290