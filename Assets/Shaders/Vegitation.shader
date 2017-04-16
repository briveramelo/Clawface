// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/vegitation"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (0.204892,0.5955882,0.01751732,0)
		_TintStrength("Tint Strength", Range( 0 , 2)) = 1
		_Normal("Normal", 2D) = "white" {}
		_WindIntensity("Wind Intensity", Range( 0 , 1)) = 0.001
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 5.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _AlbedoTint;
		uniform float _TintStrength;
		uniform float _WindIntensity;
		uniform float _MaskClipValue = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex);
			float temp_output_38_0 = ( sin( worldPosition.x ) * ( _WindIntensity * ( v.texcoord.y * cos( _Time.y ) ) ) );
			float4 appendResult40 = float4( temp_output_38_0 , 0.0 , temp_output_38_0 , 0 );
			v.vertex.xyz += ( v.color * appendResult40 ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal,uv_Normal).xyz;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode6 = tex2D( _Albedo,uv_Albedo);
			o.Albedo = ( tex2DNode6 * ( _AlbedoTint * _TintStrength ) ).rgb;
			o.Alpha = 1;
			clip( tex2DNode6.a - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1927;29;1906;1044;2025.801;967.2011;1.7;True;True
Node;AmplifyShaderEditor.SamplerNode;6;-493.9,-1053.399;Float;True;Property;_Albedo;Albedo;0;0;Assets/Models/Textures/fern1_D.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-293.0999,-829.1003;Float;False;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-80.09988,-882.1003;Float;False;0;FLOAT4;0,0,0,0;False;1;COLOR;0.0,0,0,0;False
Node;AmplifyShaderEditor.ColorNode;21;-558.0997,-851.1003;Float;False;Property;_AlbedoTint;Albedo Tint;1;0;0.204892,0.5955882,0.01751732,0
Node;AmplifyShaderEditor.SamplerNode;2;-336.7,-580.4999;Float;True;Property;_Normal;Normal;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;435.3001,-431.2;Float;False;True;7;Float;ASEMaterialInspector;Standard;turing/vegitation;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;1;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;24;-588.0997,-676.1002;Float;False;Property;_TintStrength;Tint Strength;2;0;1;0;2
Node;AmplifyShaderEditor.TimeNode;27;-1718.9,404.8999;Float;False
Node;AmplifyShaderEditor.CosOpNode;29;-1467.1,404.1;Float;False;0;FLOAT;0.0;False
Node;AmplifyShaderEditor.SinOpNode;37;-1046.2,-97.80001;Float;False;0;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-828.2,-23.8;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1286.6,247.6;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.AppendNode;40;-496.1999,24.2;Float;False;FLOAT4;0;0;0;0;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False
Node;AmplifyShaderEditor.TexCoordVertexDataNode;44;-1672.197,155.7003;Float;False;0
Node;AmplifyShaderEditor.VertexColorNode;45;-514.3984,-218.0999;Float;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-210.5984,-82.29987;Float;False;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1018.2,116.2;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.WorldPosInputsNode;36;-1286.2,-201.8;Float;False
Node;AmplifyShaderEditor.RangedFloatNode;35;-1531.401,3.199992;Float;False;Property;_WindIntensity;Wind Intensity;5;0;0.001;0;1
Node;AmplifyShaderEditor.RangedFloatNode;41;-742.2,150.2;Float;False;Constant;_Float0;Float 0;6;0;0;0;0
WireConnection;23;0;21;0
WireConnection;23;1;24;0
WireConnection;22;0;6;0
WireConnection;22;1;23;0
WireConnection;0;0;22;0
WireConnection;0;1;2;0
WireConnection;0;10;6;4
WireConnection;0;11;46;0
WireConnection;29;0;27;2
WireConnection;37;0;36;1
WireConnection;38;0;37;0
WireConnection;38;1;39;0
WireConnection;33;0;44;2
WireConnection;33;1;29;0
WireConnection;40;0;38;0
WireConnection;40;1;41;0
WireConnection;40;2;38;0
WireConnection;46;0;45;0
WireConnection;46;1;40;0
WireConnection;39;0;35;0
WireConnection;39;1;33;0
ASEEND*/
//CHKSM=FC43CAD21AADE3EF97652CC16B71C83AF11C670E