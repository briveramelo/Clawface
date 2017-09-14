// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/scrolling text"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		_ScrollingText("Scrolling Text", 2D) = "white" {}
		_ScrollSpeed("Scroll Speed", Range( 0 , 2)) = 1
		_EmissiveColor("Emissive Color", Color) = (1,0,0,0)
		_EmissionIntensity("Emission Intensity", Range( 0 , 5)) = 0
		_texttransparency("text transparency", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Front
		Blend SrcAlpha OneMinusSrcAlpha , One One
		BlendOp Add
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha  noshadow novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
		};

		uniform sampler2D _ScrollingText;
		uniform float _ScrollSpeed;
		uniform float4 _EmissiveColor;
		uniform float _EmissionIntensity;
		uniform float _texttransparency;
		uniform float _MaskClipValue = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult80 = float4( _ScrollSpeed , 0.0 , 0 , 0 );
			float4 tex2DNode52 = tex2D( _ScrollingText,( float4( i.texcoord_0, 0.0 , 0.0 ) + ( appendResult80 * _Time.x ) ).xy);
			o.Albedo = ( tex2DNode52 * _EmissiveColor ).rgb;
			o.Emission = ( _EmissiveColor * _EmissionIntensity ).rgb;
			o.Alpha = _texttransparency;
			clip( tex2DNode52 - ( _MaskClipValue ).xxxx );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1929;147;1904;868;1620.161;1455.474;1.6;True;True
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;579.7549,-750.8749;Float;False;True;6;Float;ASEMaterialInspector;Standard;turing/scrolling text;False;False;False;False;False;True;True;True;True;True;True;True;Front;0;0;False;0;0;Custom;0.5;True;False;0;True;TransparentCutout;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;1;SrcAlpha;OneMinusSrcAlpha;4;One;One;Add;OFF;0;False;0.0001;0,0,0,0;VertexScale;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.SamplerNode;52;-120.3193,-1087.264;Float;True;Property;_ScrollingText;Scrolling Text;0;0;Assets/Textures/remember-roots.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-322.6964,-1074.337;Float;False;0;FLOAT2;0.0;False;1;FLOAT4;0,0;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;78;-603.552,-1128.993;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-503.2979,-997.7363;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False
Node;AmplifyShaderEditor.TimeNode;81;-819.1959,-972.5385;Float;False
Node;AmplifyShaderEditor.AppendNode;80;-800.6953,-1116.538;Float;False;FLOAT4;0;0;0;0;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;77;-1018.496,-1044.938;Float;False;Constant;_Float4;Float 4;18;0;0;0;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-1016.496,-1124.938;Float;False;Property;_ScrollSpeed;Scroll Speed;1;0;1;0;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;224.6389,-1013.874;Float;False;0;FLOAT4;0.0;False;1;COLOR;0,0,0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;63;-143.2781,-544.9697;Float;False;Property;_EmissionIntensity;Emission Intensity;3;0;0;0;5
Node;AmplifyShaderEditor.ColorNode;82;-114.561,-727.4741;Float;False;Property;_EmissiveColor;Emissive Color;2;0;1,0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;201.8218,-691.969;Float;False;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;84;226.2389,-473.074;Float;False;Property;_texttransparency;text transparency;4;0;0;0;1
WireConnection;0;0;83;0
WireConnection;0;2;62;0
WireConnection;0;9;84;0
WireConnection;0;10;52;0
WireConnection;52;1;75;0
WireConnection;75;0;78;0
WireConnection;75;1;79;0
WireConnection;79;0;80;0
WireConnection;79;1;81;1
WireConnection;80;0;76;0
WireConnection;80;1;77;0
WireConnection;83;0;52;0
WireConnection;83;1;82;0
WireConnection;62;0;82;0
WireConnection;62;1;63;0
ASEEND*/
//CHKSM=7F4962843DE142E55A3D3E5A72987FAF86B211AD