// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PBR Outline Xray"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Header (Outline)]
		_ASEOutlineWidth( "Outline Width", Float ) = 0.04
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)

		[Header (Model)]
		_FaceTexture("Face Texture", 2D) = "white" {}
		_FaceColor("Face Color", Color) = (0,0,0,0)
		_FaceEmissiveStrength("Face Emissive Strength", Range( 0 , 5)) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoTint("Albedo Tint", Color) = (1,1,1,0)
		_Normal("Normal", 2D) = "white" {}
		_MetallicSmoothnessEmissiveAO("MetallicSmoothnessEmissiveAO", 2D) = "white" {}
		_EmissiveColor("Emissive Color", Color) = (1,1,1,0)
		_EmissiveStrength("Emissive Strength", Range( 0 , 5)) = 0
		_TextureTiling("Texture Tiling", Float) = 1
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}

		[Header Hit Feedback]
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

		// XRay
		Cull Back
		ZWrite Off
		ZTest Greater
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv2_texcoord2;
		};

		uniform float4 _XrayColor;
		uniform float _XrayColorStrength;
		uniform float4 _HitColor;
		uniform float _HitColorStrength;
		uniform sampler2D _FaceTexture;
		uniform float4 _FaceTexture_ST;
		uniform float4 _FaceColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 lerpResult77 = lerp( ( ( _XrayColor * _XrayColorStrength ) + float4( 0,0,0,0 ) ) , _HitColor , _HitColorStrength);
			float2 uv2_FaceTexture = i.uv2_texcoord2 * _FaceTexture_ST.xy + _FaceTexture_ST.zw;
			o.Albedo = ( lerpResult77 + ( tex2D( _FaceTexture, uv2_FaceTexture ) * _FaceColor ) ).rgb;
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
				struct Input
		{
			float2 texcoord_0;
			float2 uv2_texcoord2;
		};

		uniform sampler2D _Normal;
		uniform float _TextureTiling;
		uniform sampler2D _Albedo;
		uniform float4 _AlbedoTint;
		uniform float4 _HitColor;
		uniform float _HitColorStrength;
		uniform sampler2D _FaceTexture;
		uniform float4 _FaceTexture_ST;
		uniform float4 _FaceColor;
		uniform float _FaceEmissiveStrength;
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
			float4 lerpResult73 = lerp( ( tex2D( _Albedo, i.texcoord_0 ) * _AlbedoTint ) , _HitColor , _HitColorStrength);
			o.Albedo = lerpResult73.rgb;
			float2 uv2_FaceTexture = i.uv2_texcoord2 * _FaceTexture_ST.xy + _FaceTexture_ST.zw;
			o.Emission = ( ( ( tex2D( _FaceTexture, uv2_FaceTexture ) * _FaceColor ) * _FaceEmissiveStrength ) + ( ( tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).b * _EmissiveColor ) * _EmissiveStrength ) ).rgb;
			o.Metallic = tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).r;
			o.Smoothness = tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).g;
			o.Occlusion = tex2D( _MetallicSmoothnessEmissiveAO, i.texcoord_0 ).a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=11001
2015;45;1767;988;595.2139;492.0201;1.215005;True;True
Node;AmplifyShaderEditor.RangedFloatNode;12;-1665.4,-496;Float;False;Property;_TextureTiling;Texture Tiling;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1414.2,-527.8002;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;54;-653.9329,-194.8965;Float;True;Property;_MetallicSmoothnessEmissiveAO;MetallicSmoothnessEmissiveAO;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;62;173.9947,138.0779;Float;False;Property;_FaceColor;Face Color;1;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;61;96.5946,-49.72198;Float;True;Property;_FaceTexture;Face Texture;0;0;None;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;55;-315.0337,-194.6969;Float;False;FLOAT4;1;0;FLOAT4;0.0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;52;-65.59489,534.0291;Float;False;Property;_EmissiveColor;Emissive Color;7;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;65;408.2123,246.8085;Float;False;Property;_FaceEmissiveStrength;Face Emissive Strength;2;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;530.2245,61.46792;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;232.0061,435.4297;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;51;182.8059,609.4288;Float;False;Property;_EmissiveStrength;Emissive Strength;8;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;760.6339,57.94284;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;713.9058,311.6689;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;1;-570.6997,-742.2999;Float;True;Property;_Albedo;Albedo;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;29;-261.3137,-675.9499;Float;False;Property;_AlbedoTint;Albedo Tint;4;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;66;964.4725,50.5624;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-4.724298,-671.0198;Float;False;2;2;0;FLOAT4;0.0;False;1;COLOR;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;2;-324.7001,-395.1002;Float;True;Property;_Normal;Normal;5;0;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1198.9,-315.7001;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;PBR Outline Player;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;Back;0;0;False;0;0;Translucent;0.5;True;True;0;True;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;True;0.04;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;2;-1;-1;-1;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;12;0
WireConnection;54;1;11;0
WireConnection;55;0;54;0
WireConnection;63;0;61;0
WireConnection;63;1;62;0
WireConnection;50;0;55;2
WireConnection;50;1;52;0
WireConnection;64;0;63;0
WireConnection;64;1;65;0
WireConnection;53;0;50;0
WireConnection;53;1;51;0
WireConnection;1;1;11;0
WireConnection;66;0;64;0
WireConnection;66;1;53;0
WireConnection;30;0;1;0
WireConnection;30;1;29;0
WireConnection;2;1;11;0
WireConnection;0;0;30;0
WireConnection;0;1;2;0
WireConnection;0;2;66;0
WireConnection;0;3;55;0
WireConnection;0;4;55;1
WireConnection;0;5;55;3
ASEEND*/
//CHKSM=BD7193E8F1326FA2C23E86DEC3076FA8F3F435A9