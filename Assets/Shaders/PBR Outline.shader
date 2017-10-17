// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PBR Outline"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ASEOutlineColor( "Outline Color", Color ) = (1,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.01
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,0)
		_Normal("Normal", 2D) = "white" {}
		_MetallicSmoothnessEmissiveAO("MetallicSmoothnessEmissiveAO", 2D) = "white" {}
		_EmissiveColor("Emissive Color", Color) = (1,1,1,0)
		_EmissiveStrength("Emissive Strength", Range( 0 , 5)) = 0
		_TextureTiling("Texture Tiling", Float) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:outlineVertexDataFunc
		struct Input
		{
			fixed filler;
		};
		uniform fixed4 _ASEOutlineColor;
		uniform fixed _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline fixed4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return fixed4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o ) { o.Emission = _ASEOutlineColor.rgb; o.Alpha = 1; }
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
		};

		uniform sampler2D _Normal;
		uniform float _TextureTiling;
		uniform sampler2D _Albedo;
		uniform float4 _AlbedoTint;
		uniform sampler2D _MetallicSmoothnessEmissiveAO;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveStrength;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_TextureTiling).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = UnpackNormal( tex2D( _Normal, i.texcoord_0 ) );
			o.Albedo = ( tex2D( _Albedo, i.texcoord_0 ) * _AlbedoTint ).xyz;
			o.Emission = ( ( tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).z * _EmissiveColor ) * _EmissiveStrength ).rgb;
			o.Metallic = tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).x;
			o.Smoothness = tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).y;
			o.Occlusion = tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).w;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=11001
2016;45;1767;988;2169.028;1600.401;2.5;True;True
Node;AmplifyShaderEditor.RangedFloatNode;12;-1277.9,-468.5;Float;False;Property;_TextureTiling;Texture Tiling;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1026.7,-500.3002;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;54;-766.9285,-182.4014;Float;True;Property;_MetallicSmoothnessEmissiveAO;MetallicSmoothnessEmissiveAO;3;0;Assets/Textures/Environment/MILO/player_texture_MetallicSmoothnessEmissiveAO.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;55;-312.0294,-179.2018;Float;False;FLOAT4;1;0;FLOAT4;0.0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;52;-79.13554,38.39901;Float;False;Property;_EmissiveColor;Emissive Color;4;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;218.4646,-60.20028;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;29;-15.21395,-582.7505;Float;False;Property;_AlbedoTint;Albedo Tint;1;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-359.6,-626.5001;Float;True;Property;_Albedo;Albedo;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;169.2645,113.7988;Float;False;Property;_EmissiveStrength;Emissive Strength;5;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;402.5639,-304.4009;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;243.2757,-577.8205;Float;False;2;2;0;FLOAT4;0.0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;2;-324.7001,-395.1002;Float;True;Property;_Normal;Normal;2;0;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;676.4,-350.7001;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;PBR Outline;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Translucent;0.5;True;True;0;True;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0.01;1,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;2;-1;-1;-1;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;12;0
WireConnection;54;1;11;0
WireConnection;55;0;54;0
WireConnection;50;0;55;2
WireConnection;50;1;52;0
WireConnection;1;1;11;0
WireConnection;53;0;50;0
WireConnection;53;1;51;0
WireConnection;30;0;1;0
WireConnection;30;1;29;0
WireConnection;2;1;11;0
WireConnection;0;0;30;0
WireConnection;0;1;2;0
WireConnection;0;2;53;0
WireConnection;0;3;55;0
WireConnection;0;4;55;1
WireConnection;0;5;55;3
ASEEND*/
//CHKSM=795E99C1657A8FEE9EFBED851D1B28C1FD45674E