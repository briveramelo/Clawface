// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PBR Outline"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ASEOutlineWidth( "Outline Width", Float ) = 0.02
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_MainTex("MainTex", 2D) = "white" {}
		_AlbedoColor("Albedo Color", Color) = (1,1,1,0)
		_MetallicTexture("Metallic Texture", 2D) = "white" {}
		_SmoothnessTexture("Smoothness Texture", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_TextureTiling("Texture Tiling", Float) = 0
		_EmissiveStrength("Emissive Strength", Range( 0 , 5)) = 0
		_EmissiveColor("Emissive Color", Color) = (1,1,1,0)
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
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
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
		uniform sampler2D _MainTex;
		uniform float4 _AlbedoColor;
		uniform sampler2D _Emission;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveStrength;
		uniform sampler2D _MetallicTexture;
		uniform sampler2D _SmoothnessTexture;
		uniform sampler2D _AO;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_TextureTiling).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = UnpackNormal( tex2D( _Normal, i.texcoord_0 ) );
			o.Albedo = ( tex2D( _MainTex, i.texcoord_0 ) * _AlbedoColor ).xyz;
			o.Emission = ( ( tex2D( _Emission, i.texcoord_0 ) * _EmissiveColor ) * _EmissiveStrength ).rgb;
			o.Metallic = tex2D( _MetallicTexture, i.texcoord_0 ).r;
			o.Smoothness = tex2D( _SmoothnessTexture, i.texcoord_0 ).r;
			o.Occlusion = tex2D( _AO, i.texcoord_0 ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=11001
1927;44;1767;988;961.4291;750.1016;1.3;True;True
Node;AmplifyShaderEditor.RangedFloatNode;12;-1187.5,-307.4998;Float;False;Property;_TextureTiling;Texture Tiling;7;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-936.3004,-339.3;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-338.7997,-294.9003;Float;True;Property;_Emission;Emission;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;52;-38.83556,-268.7009;Float;False;Property;_EmissiveColor;Emissive Color;9;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;29;24.78605,-675.2505;Float;False;Property;_AlbedoColor;Albedo Color;1;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-334.7001,-687.4998;Float;True;Property;_MainTex;MainTex;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;232.8647,-340.2003;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;51;154.8645,-220.6011;Float;False;Property;_EmissiveStrength;Emissive Strength;8;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;4;-337.2003,-101.3005;Float;True;Property;_MetallicTexture;Metallic Texture;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-337.2001,-490.1002;Float;True;Property;_Normal;Normal;4;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;5;-334.0001,97.09951;Float;True;Property;_SmoothnessTexture;Smoothness Texture;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-337.2003,287.4995;Float;True;Property;_AO;AO;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;250.7757,-672.8205;Float;False;2;2;0;FLOAT4;0.0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;447.364,-301.2009;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;676.4,-350.7001;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;PBR Outline;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0.02;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;12;0
WireConnection;3;1;11;0
WireConnection;1;1;11;0
WireConnection;50;0;3;0
WireConnection;50;1;52;0
WireConnection;4;1;11;0
WireConnection;2;1;11;0
WireConnection;5;1;11;0
WireConnection;6;1;11;0
WireConnection;30;0;1;0
WireConnection;30;1;29;0
WireConnection;53;0;50;0
WireConnection;53;1;51;0
WireConnection;0;0;30;0
WireConnection;0;1;2;0
WireConnection;0;2;53;0
WireConnection;0;3;4;0
WireConnection;0;4;5;0
WireConnection;0;5;6;0
ASEEND*/
//CHKSM=886B37CCB1FBD96540937D0395C05DF15636E765