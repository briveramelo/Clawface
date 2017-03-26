// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/hologram-globe"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MeshTransparency("Mesh Transparency", Range( 0 , 1)) = 0.8
		_LandTransparency("Land Transparency", Range( 0 , 1)) = 0.6
		_MainTexture("Main Texture", 2D) = "white" {}
		_ColorTint("Color Tint", Color) = (0.875,0.04503676,0.04503676,0)
		_Lines("Lines", 2D) = "white" {}
		_LineIntensity("Line Intensity", Range( 0 , 10)) = 0
		_LineSpeed("Line Speed", Range( 0 , 5)) = 0.1
		_LineSize("Line Size", Range( 0 , 2)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
			float2 uv_texcoord;
			float2 texcoord_0;
		};

		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float4 _ColorTint;
		uniform sampler2D _Lines;
		uniform float _LineSpeed;
		uniform float _LineSize;
		uniform float _LineIntensity;
		uniform float _LandTransparency;
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
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode16 = tex2D( _MainTexture,uv_MainTexture);
			float4 temp_cast_1 = _LineIntensity;
			float4 temp_cast_2 = tex2DNode16.a;
			float4 temp_output_15_0 = ( tex2DNode16 * lerp( ( _ColorTint * ( tex2D( _Lines,(abs( i.texcoord_0+( _Time.x * _LineSpeed ) * float2(1,1 )))) + temp_cast_1 ) ) , temp_cast_2 , ( tex2DNode16.a * _LandTransparency ) ) );
			o.Albedo = temp_output_15_0.xyz;
			o.Emission = temp_output_15_0.xyz;
			o.Smoothness = 0.0;
			o.Alpha = _MeshTransparency;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1929;66;1904;887;3606.917;2816.552;3.6452;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1767.909,-597.26;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;36;-2107.81,-782.5598;Float;False;Property;_LineSize;Line Size;7;0;0;0;2
Node;AmplifyShaderEditor.TimeNode;33;-1927.709,-433.6598;Float;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1671.709,-382.6597;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;30;-1992.309,-580.26;Float;False;Constant;_Float0;Float 0;4;0;1;0;0
Node;AmplifyShaderEditor.PannerNode;27;-1474.908,-523.2599;Float;False;1;1;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;23;-1237.687,-139.3472;Float;False;Property;_LineIntensity;Line Intensity;5;0;0;0;10
Node;AmplifyShaderEditor.SamplerNode;17;-1258.088,-354.1129;Float;True;Property;_Lines;Lines;4;0;Assets/Textures/Environment/props/holo_lines 1.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.RangedFloatNode;35;-1992.709,-239.3621;Float;False;Property;_LineSpeed;Line Speed;6;0;0.1;0;5
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-755.2028,-343.9245;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;13;140.6223,-411.5365;Float;False;Property;_MeshTransparency;Mesh Transparency;0;0;0.8;0;1
Node;AmplifyShaderEditor.RangedFloatNode;51;226.2275,-538.931;Float;False;Constant;_Float1;Float 1;8;0;0;0;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;567.2549,-673.3749;Float;False;True;6;Float;ASEMaterialInspector;Standard;turing/hologram-globe;False;False;False;False;False;True;True;True;True;True;True;True;Front;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;1;SrcAlpha;OneMinusSrcAlpha;4;One;One;Add;OFF;0;False;0.0001;0,0,0,0;VertexScale;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.LerpOp;41;-74.7577,-626.4921;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;2;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;120.708,-709.0864;Float;False;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False
Node;AmplifyShaderEditor.ColorNode;18;-1160.964,-784.8842;Float;False;Property;_ColorTint;Color Tint;3;0;0.875,0.04503676,0.04503676,0
Node;AmplifyShaderEditor.RangedFloatNode;42;-619.0595,-537.7708;Float;False;Property;_LandTransparency;Land Transparency;1;0;0.6;0;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-297.7813,-607.9659;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-503.9759,-429.8381;Float;False;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False
Node;AmplifyShaderEditor.SamplerNode;16;-616.7106,-856.6377;Float;True;Property;_MainTexture;Main Texture;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
WireConnection;28;0;36;0
WireConnection;28;1;30;0
WireConnection;34;0;33;1
WireConnection;34;1;35;0
WireConnection;27;0;28;0
WireConnection;27;1;34;0
WireConnection;17;1;27;0
WireConnection;26;0;17;0
WireConnection;26;1;23;0
WireConnection;0;0;15;0
WireConnection;0;2;15;0
WireConnection;0;4;51;0
WireConnection;0;9;13;0
WireConnection;41;0;50;0
WireConnection;41;1;16;4
WireConnection;41;2;43;0
WireConnection;15;0;16;0
WireConnection;15;1;41;0
WireConnection;43;0;16;4
WireConnection;43;1;42;0
WireConnection;50;0;18;0
WireConnection;50;1;26;0
ASEEND*/
//CHKSM=96A8C8E72BF829AD037738FE180032A67CF5ACDA