// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/hologram"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MeshTransparency("Mesh Transparency", Range( 0 , 1)) = 0.8
		_ColorTint("Color Tint", Color) = (0.875,0.04503676,0.04503676,0)
		_Lines("Lines", 2D) = "white" {}
		_LineIntensity("Line Intensity", Range( 0 , 10)) = 0
		_LineSpeed("Line Speed", Range( 0 , 5)) = 0.1
		_LineSize("Line Size", Range( 0 , 2)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard alpha:fade keepalpha  noshadow novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
		};

		uniform float4 _ColorTint;
		uniform sampler2D _Lines;
		uniform float _LineSpeed;
		uniform float _LineSize;
		uniform float _LineIntensity;
		uniform float _MeshTransparency;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = _LineSize;
			float2 temp_cast_1 = 1.0;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + temp_cast_1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_1 = i.texcoord_0.x;
			float4 temp_cast_2 = _LineIntensity;
			o.Albedo = ( _ColorTint * ( tex2D( _Lines,(abs( temp_cast_1+( _Time.x * _LineSpeed ) * float2(1,1 )))) + temp_cast_2 ) ).xyz;
			float2 temp_cast_5 = i.texcoord_0.x;
			float4 temp_cast_6 = _LineIntensity;
			o.Emission = ( _ColorTint * ( tex2D( _Lines,(abs( temp_cast_5+( _Time.x * _LineSpeed ) * float2(1,1 )))) + temp_cast_6 ) ).xyz;
			o.Smoothness = 0.0;
			o.Alpha = _MeshTransparency;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1929;147;1904;868;1592.978;1339.77;1.6;True;True
Node;AmplifyShaderEditor.RangedFloatNode;13;140.6223,-411.5365;Float;False;Property;_MeshTransparency;Mesh Transparency;0;0;0.8;0;1
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;567.2549,-673.3749;Float;False;True;6;Float;ASEMaterialInspector;Standard;turing/hologram;False;False;False;False;False;True;True;True;True;True;True;True;Front;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;1;SrcAlpha;OneMinusSrcAlpha;4;One;One;Add;OFF;0;False;0.0001;0,0,0,0;VertexScale;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;23;-500.0877,-425.7473;Float;False;Property;_LineIntensity;Line Intensity;5;0;0;0;10
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1046.31,-691.8602;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;36;-1386.211,-877.16;Float;False;Property;_LineSize;Line Size;7;0;0;0;2
Node;AmplifyShaderEditor.TimeNode;33;-1206.11,-528.2599;Float;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-950.1092,-477.2599;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;30;-1270.71,-674.8602;Float;False;Constant;_Float0;Float 0;4;0;1;0;0
Node;AmplifyShaderEditor.PannerNode;27;-753.3079,-617.8601;Float;False;1;1;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;35;-1271.11,-333.9622;Float;False;Property;_LineSpeed;Line Speed;6;0;0.1;0;5
Node;AmplifyShaderEditor.SamplerNode;17;-520.4887,-640.5128;Float;True;Property;_Lines;Lines;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.ColorNode;18;-468.8642,-885.3837;Float;False;Property;_ColorTint;Color Tint;3;0;0.875,0.04503676,0.04503676,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;233.6241,-716.2379;Float;False;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-17.60269,-630.3243;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;333.3805,-962.764;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.ColorNode;56;56.22205,-858.6704;Float;False;Constant;_Color0;Color 0;7;0;1,0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;273.5806,-800.264;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;51;226.2275,-538.931;Float;False;Constant;_Float1;Float 1;8;0;0;0;0
Node;AmplifyShaderEditor.SamplerNode;52;3.180542,-1001.764;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
WireConnection;0;0;50;0
WireConnection;0;2;50;0
WireConnection;0;4;51;0
WireConnection;0;9;13;0
WireConnection;28;0;36;0
WireConnection;28;1;30;0
WireConnection;34;0;33;1
WireConnection;34;1;35;0
WireConnection;27;0;28;1
WireConnection;27;1;34;0
WireConnection;17;1;27;0
WireConnection;50;0;18;0
WireConnection;50;1;26;0
WireConnection;26;0;17;0
WireConnection;26;1;23;0
ASEEND*/
//CHKSM=405BABDF67673ED1C747072CD33CCFCAA363D57C