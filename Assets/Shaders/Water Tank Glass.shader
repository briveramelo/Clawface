// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water Tank Glass"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_InnerOuterColorStrength("Inner/Outer Color Strength", Range( 1 , 10)) = 2.726571
		_WaterOuter("Water Outer", Color) = (0,0.2980392,0.6980392,1)
		_WaterInner("Water Inner", Color) = (0.4392157,0.7294118,1,1)
		_Refaction("Refaction", Range( 0 , 2)) = 1.470094
		_Transparency("Transparency", Range( 0 , 1)) = 0.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#pragma target 4.6
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF noshadow exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		uniform float4 _WaterInner;
		uniform float4 _WaterOuter;
		uniform float _InnerOuterColorStrength;
		uniform float _Transparency;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;
		uniform float _Refaction;

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
			o.Normal = float3(0,0,1);
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelFinalVal4 = (0.0 + 1.0*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , _InnerOuterColorStrength));
			o.Emission = lerp( _WaterInner , _WaterOuter , fresnelFinalVal4 ).rgb;
			o.Alpha = _Transparency;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=11001
2016;45;1767;988;1782.699;454.2006;1.6;True;True
Node;AmplifyShaderEditor.RangedFloatNode;1;-1257.382,80.05685;Float;False;Property;_InnerOuterColorStrength;Inner/Outer Color Strength;2;0;2.726571;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-1002.086,-171.5687;Float;False;Property;_WaterOuter;Water Outer;3;0;0,0.2980392,0.6980392,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FresnelNode;4;-864.7542,11.12134;Float;True;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-989.3254,-369.3776;Float;False;Property;_WaterInner;Water Inner;4;0;0.4392157,0.7294118,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;7;-537.1241,236.1;Float;False;Property;_Refaction;Refaction;5;0;1.470094;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;11;-446.3551,-109.1188;Float;True;3;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;8;-527.5245,357.7005;Float;False;Property;_Transparency;Transparency;6;0;0.5;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;6;Float;ASEMaterialInspector;0;Standard;Water Tank Glass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;0;-1;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;3;1;0
WireConnection;11;0;3;0
WireConnection;11;1;2;0
WireConnection;11;2;4;0
WireConnection;0;2;11;0
WireConnection;0;8;7;0
WireConnection;0;9;8;0
ASEEND*/
//CHKSM=99E0822D9998CA5475594F8F68600CF288FD3F3C