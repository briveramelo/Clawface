// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PBR Outline Blood"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.01
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,0)
		_Normal("Normal", 2D) = "bump" {}
		_MetallicSmoothnessEmissiveAO("MetallicSmoothnessEmissiveAO", 2D) = "white" {}
		_EmissiveColor("Emissive Color", Color) = (1,1,1,0)
		_EmissiveStrength("Emissive Strength", Range( 0 , 5)) = 0
		_TextureTiling("Texture Tiling", Float) = 0
		[PerRendererData]_SplatMap("Splat Map", 2D) = "black" {}
		[PerRendererData]_RenderMask("Render Mask", 2D) = "black" {}
		_Blood2("Blood 2", Color) = (1,0.2720588,0.2720588,0)
		_Blood1("Blood 1", Color) = (0.75,0,0,0)
		_BloodColorationMask("Blood Coloration Mask", 2D) = "white" {}
		_BloodColorationScale("Blood Coloration Scale", Range( 0 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _RenderMask;
		uniform float4 _RenderMask_ST;
		uniform sampler2D _SplatMap;
		uniform float4 _SplatMap_ST;
		uniform sampler2D _Normal;
		uniform float _TextureTiling;
		uniform sampler2D _Albedo;
		uniform float4 _AlbedoTint;
		uniform float4 _Blood1;
		uniform float4 _Blood2;
		uniform sampler2D _BloodColorationMask;
		uniform float _BloodColorationScale;
		uniform sampler2D _MetallicSmoothnessEmissiveAO;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveStrength;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_RenderMask = i.uv_texcoord * _RenderMask_ST.xy + _RenderMask_ST.zw;
			float4 tex2DNode66 = tex2D( _RenderMask, uv_RenderMask );
			float2 uv_SplatMap = i.uv_texcoord * _SplatMap_ST.xy + _SplatMap_ST.zw;
			float4 tex2DNode56 = tex2D( _SplatMap, uv_SplatMap );
			float4 appendResult67 = (float4(tex2DNode56.r , tex2DNode56.g , tex2DNode56.b , 0.0));
			float2 temp_cast_0 = (_TextureTiling).xx;
			float2 uv_TexCoord11 = i.uv_texcoord * temp_cast_0 + float2( 0,0 );
			float3 tex2DNode2 = UnpackNormal( tex2D( _Normal, uv_TexCoord11 ) );
			float4 normalizeResult73 = normalize( ( appendResult67 + float4( tex2DNode2 , 0.0 ) ) );
			float4 ifLocalVar71 = 0;
			if( tex2DNode66.r <= 0.0 )
				ifLocalVar71 = float4( tex2DNode2 , 0.0 );
			else
				ifLocalVar71 = normalizeResult73;
			o.Normal = ifLocalVar71.xyz;
			float2 temp_cast_5 = (_BloodColorationScale).xx;
			float2 uv_TexCoord64 = i.uv_texcoord * temp_cast_5 + float2( 0,0 );
			float4 lerpResult61 = lerp( _Blood1 , _Blood2 , tex2D( _BloodColorationMask, uv_TexCoord64 ).r);
			float ifLocalVar68 = 0;
			if( tex2DNode66.r <= 0.0 )
				ifLocalVar68 = 0.0;
			else
				ifLocalVar68 = tex2DNode56.a;
			float4 lerpResult60 = lerp( ( tex2D( _Albedo, uv_TexCoord11 ) * _AlbedoTint ) , lerpResult61 , ifLocalVar68);
			o.Albedo = lerpResult60.rgb;
			o.Emission = ( ( tex2D( _MetallicSmoothnessEmissiveAO, uv_TexCoord11 ).b * _EmissiveColor ) * _EmissiveStrength ).rgb;
			o.Metallic = tex2D( _MetallicSmoothnessEmissiveAO, uv_TexCoord11 ).r;
			o.Smoothness = tex2D( _MetallicSmoothnessEmissiveAO, uv_TexCoord11 ).g;
			o.Occlusion = tex2D( _MetallicSmoothnessEmissiveAO, uv_TexCoord11 ).a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13903
1927;29;1906;1044;1144.15;1835.342;1.573811;True;True
Node;AmplifyShaderEditor.RangedFloatNode;12;-1277.9,-468.5;Float;False;Property;_TextureTiling;Texture Tiling;6;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;65;-1421.073,-1156.688;Float;False;Property;_BloodColorationScale;Blood Coloration Scale;12;0;0;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1026.7,-500.3002;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;56;-178.8419,-1533.328;Float;True;Property;_SplatMap;Splat Map;7;1;[PerRendererData];None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-1053.606,-1134.504;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;67;154.8472,-874.6045;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;2;-324.7001,-395.1002;Float;True;Property;_Normal;Normal;2;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;54;-766.9285,-182.4014;Float;True;Property;_MetallicSmoothnessEmissiveAO;MetallicSmoothnessEmissiveAO;3;0;Assets/Textures/Environment/MILO/player_texture_MetallicSmoothnessEmissiveAO.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;29;-346.6052,-783.7257;Float;False;Property;_AlbedoTint;Albedo Tint;1;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;52;-79.13554,38.39901;Float;False;Property;_EmissiveColor;Emissive Color;4;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;72;327.5752,-811.8777;Float;False;2;2;0;FLOAT4;0.0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;1;-657.6342,-866.6882;Float;True;Property;_Albedo;Albedo;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;59;-576.5565,-1539.865;Float;False;Property;_Blood1;Blood 1;10;0;0.75,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;66;-168.9255,-1744.09;Float;True;Property;_RenderMask;Render Mask;8;1;[PerRendererData];None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;70;115.2348,-1559.54;Float;False;Constant;_Float1;Float 1;13;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;63;-704.6082,-1106.17;Float;True;Property;_BloodColorationMask;Blood Coloration Mask;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;55;-312.0294,-179.2018;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;62;-569.7834,-1366.941;Float;False;Property;_Blood2;Blood 2;9;0;1,0.2720588,0.2720588,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;61;-127.9149,-1238.161;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.NormalizeNode;73;462.3983,-790.5101;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-101.557,-882.7745;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;218.4646,-60.20028;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.ConditionalIfNode;68;353.8668,-1626.6;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;51;169.2645,113.7988;Float;False;Property;_EmissiveStrength;Emissive Strength;5;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;466.7248,-153.1865;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ConditionalIfNode;71;653.978,-819.3946;Float;False;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT4;0.0;False;3;FLOAT3;0.0;False;4;FLOAT3;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;60;675.2321,-1304.073;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1038.377,-775.6289;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PBR Outline Blood;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;12;0
WireConnection;64;0;65;0
WireConnection;67;0;56;1
WireConnection;67;1;56;2
WireConnection;67;2;56;3
WireConnection;2;1;11;0
WireConnection;54;1;11;0
WireConnection;72;0;67;0
WireConnection;72;1;2;0
WireConnection;1;1;11;0
WireConnection;63;1;64;0
WireConnection;55;0;54;0
WireConnection;61;0;59;0
WireConnection;61;1;62;0
WireConnection;61;2;63;0
WireConnection;73;0;72;0
WireConnection;30;0;1;0
WireConnection;30;1;29;0
WireConnection;50;0;55;2
WireConnection;50;1;52;0
WireConnection;68;0;66;1
WireConnection;68;1;70;0
WireConnection;68;2;56;4
WireConnection;68;3;70;0
WireConnection;68;4;70;0
WireConnection;53;0;50;0
WireConnection;53;1;51;0
WireConnection;71;0;66;1
WireConnection;71;1;70;0
WireConnection;71;2;73;0
WireConnection;71;3;2;0
WireConnection;71;4;2;0
WireConnection;60;0;30;0
WireConnection;60;1;61;0
WireConnection;60;2;68;0
WireConnection;0;0;60;0
WireConnection;0;1;71;0
WireConnection;0;2;53;0
WireConnection;0;3;55;0
WireConnection;0;4;55;1
WireConnection;0;5;55;3
ASEEND*/
//CHKSM=DEA11FC0B7AD58D571052465CB5D53F600D4F001