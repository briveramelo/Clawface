// Upgrade NOTE: upgraded instancing buffer 'turingblood' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/blood"
{
	Properties
	{
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
		_Color1("Color 1", Color) = (1,0,0,0)
		_Color2("Color 2", Color) = (0.3970588,0.01751729,0.01751729,0)
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_BloodMask("Blood Mask", 2D) = "white" {}
		_DetailMask("Detail Mask", 2D) = "white" {}
		_BloodSmoothness1("Blood Smoothness 1", Range( 0 , 2)) = 0.5
		_BloodSmoothness2("Blood Smoothness 2", Range( 0 , 2)) = 0
		_DetailMaskTiling("Detail Mask Tiling", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha  exclude_path:deferred nolightmap  nofog vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
		};

		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform sampler2D _TextureSample1;
		uniform sampler2D _BloodMask;
		uniform float4 _BloodMask_ST;
		uniform float _MaskClipValue = 0.5;

		UNITY_INSTANCING_BUFFER_START(turingblood)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color1)
#define _Color1_arr turingblood
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color2)
#define _Color2_arr turingblood
			UNITY_DEFINE_INSTANCED_PROP(float, _BloodSmoothness1)
#define _BloodSmoothness1_arr turingblood
			UNITY_DEFINE_INSTANCED_PROP(float, _BloodSmoothness2)
#define _BloodSmoothness2_arr turingblood
			UNITY_DEFINE_INSTANCED_PROP(float, _DetailMaskTiling)
#define _DetailMaskTiling_arr turingblood
		UNITY_INSTANCING_BUFFER_END(turingblood)

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = UNITY_ACCESS_INSTANCED_PROP(_DetailMaskTiling_arr, _DetailMaskTiling);
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			o.Albedo = lerp( UNITY_ACCESS_INSTANCED_PROP(_Color1_arr, _Color1) , UNITY_ACCESS_INSTANCED_PROP(_Color2_arr, _Color2) , tex2D( _DetailMask,uv_DetailMask).x ).rgb;
			o.Smoothness = lerp( UNITY_ACCESS_INSTANCED_PROP(_BloodSmoothness1_arr, _BloodSmoothness1) , UNITY_ACCESS_INSTANCED_PROP(_BloodSmoothness2_arr, _BloodSmoothness2) , tex2D( _TextureSample1,i.texcoord_0).x );
			o.Alpha = 1;
			float2 uv_BloodMask = i.uv_texcoord * _BloodMask_ST.xy + _BloodMask_ST.zw;
			clip( tex2D( _BloodMask,uv_BloodMask) - ( _MaskClipValue ).xxxx );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1928;165;1904;868;1653.117;1054.803;2.114428;True;True
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;261,-148;Float;False;True;2;Float;ASEMaterialInspector;Standard;turing/blood;False;False;False;False;False;False;True;False;False;True;False;False;Back;0;0;False;0;0;Custom;0.5;True;False;0;True;TransparentCutout;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.LerpOp;19;-2.419144,85.91479;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.ColorNode;8;-374.215,-799.5001;Float;False;InstancedProperty;_Color2;Color 2;1;0;0.3970588,0.01751729,0.01751729,0
Node;AmplifyShaderEditor.ColorNode;7;-342.9822,-975.1615;Float;False;InstancedProperty;_Color1;Color 1;0;0;1,0,0,0
Node;AmplifyShaderEditor.LerpOp;9;-13.80003,-668.2997;Float;False;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;20;-319.3185,25.06482;Float;False;InstancedProperty;_BloodSmoothness1;Blood Smoothness 1;5;0;0.5;0;2
Node;AmplifyShaderEditor.RangedFloatNode;21;-318.6182,104.5648;Float;False;InstancedProperty;_BloodSmoothness2;Blood Smoothness 2;6;0;0;0;2
Node;AmplifyShaderEditor.SamplerNode;28;-401.5243,193.0408;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Assets/Textures/blood/detail_mask_1.png;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.RangedFloatNode;30;-995.0659,141.9552;Float;False;InstancedProperty;_DetailMaskTiling;Detail Mask Tiling;6;0;0;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-692.9664,187.5552;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.SamplerNode;26;-701.3987,-671.8498;Float;True;Property;_DetailMask;Detail Mask;3;0;Assets/Textures/blood/detail_mask_1.png;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SamplerNode;27;-103.1503,359.969;Float;True;Property;_BloodMask;Blood Mask;2;0;Assets/Textures/blood/Blood12_mask.png;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
WireConnection;0;0;9;0
WireConnection;0;4;19;0
WireConnection;0;10;27;0
WireConnection;19;0;20;0
WireConnection;19;1;21;0
WireConnection;19;2;28;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;9;2;26;0
WireConnection;28;1;29;0
WireConnection;29;0;30;0
ASEEND*/
//CHKSM=F1A1CBACB7622E3AE3AA3EF7E2BB052F9DC6A5FC