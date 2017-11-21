// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Enemy Projectile"
{
	Properties
	{
		_ColorFront("Color Front", Color) = (1,0.7655172,0,0)
		_ColorRear("Color Rear", Color) = (1,0,0,0)
		_ColorMask("Color Mask", 2D) = "white" {}
		_OpacityMask("Opacity Mask", 2D) = "white" {}
		_FrontGlowStrength("Front Glow Strength", Range( 0 , 10)) = 1
		_RearGlowStrength("Rear Glow Strength", Range( 0 , 10)) = 1
		_RotationSpeed("Rotation Speed", Float) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		AlphaToMask On
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 texcoord_0;
			float2 texcoord_1;
		};

		uniform float4 _ColorRear;
		uniform float4 _ColorFront;
		uniform sampler2D _ColorMask;
		uniform float _RearGlowStrength;
		uniform float _FrontGlowStrength;
		uniform sampler2D _OpacityMask;
		uniform float _RotationSpeed;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 appendResult60 = (float4(1.0 , 0.0 , 0.0 , 0.0));
			o.texcoord_0.xy = v.texcoord.xy * float2( 1,1 ) + appendResult60.xy;
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult39 = (float4(0.0 , 0.0 , 0.0 , 0.0));
			float mulTime40 = _Time.y * 0.5;
			float4 tex2DNode7 = tex2D( _ColorMask, ( float4( i.texcoord_0, 0.0 , 0.0 ) + ( appendResult39 * mulTime40 ) ).xy );
			float4 lerpResult8 = lerp( _ColorRear , _ColorFront , tex2DNode7.r);
			float lerpResult63 = lerp( _RearGlowStrength , _FrontGlowStrength , tex2DNode7.r);
			o.Emission = ( lerpResult8 * lerpResult63 ).rgb;
			float4 appendResult20 = (float4(1.0 , 0.0 , 0.0 , 0.0));
			float mulTime22 = _Time.y * _RotationSpeed;
			o.Alpha = tex2D( _OpacityMask, ( float4( i.texcoord_1, 0.0 , 0.0 ) + ( appendResult20 * mulTime22 ) ).xy ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13401
1998;29;1767;1004;2618.184;1205.66;2.110529;True;True
Node;AmplifyShaderEditor.RangedFloatNode;38;-1894.589,-256.0709;Float;False;Constant;_Float2;Float 2;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;61;-1801.262,-358.6262;Float;False;Constant;_Float7;Float 7;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;37;-1896.672,-173.0959;Float;False;Constant;_Float1;Float 1;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;62;-1799.178,-441.6016;Float;False;Constant;_Float8;Float 8;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;36;-1917.185,-30.02647;Float;False;Constant;_Float0;Float 0;6;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1688.43,-226.7173;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;60;-1610.22,-387.1138;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleTimeNode;40;-1683.228,-27.81773;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1415.431,-140.0172;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;19;-1249.057,330.9086;Float;False;Constant;_YRotation;Y Rotation;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-1269.57,473.9782;Float;False;Property;_RotationSpeed;Rotation Speed;7;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-1246.974,247.9335;Float;False;Constant;_XRotation;X Rotation;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-1378.33,-378.9174;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;22;-1035.613,476.187;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-1121.568,-121.2311;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1040.816,277.2872;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;35;-610.1265,-111.9971;Float;False;Property;_RearGlowStrength;Rear Glow Strength;5;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-810.7157,178.0872;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;1;-880.1661,-386.5349;Float;False;Property;_ColorFront;Color Front;0;0;1,0.7655172,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;6;-880.845,-569.8295;Float;False;Property;_ColorRear;Color Rear;1;0;1,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;64;-607.9304,-11.07605;Float;False;Property;_FrontGlowStrength;Front Glow Strength;4;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-767.8168,363.9874;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;7;-926.011,-180.0596;Float;True;Property;_ColorMask;Color Mask;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-540.3163,252.1872;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;63;-319.9304,-78.07605;Float;False;3;0;FLOAT;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;8;-419.9946,-302.0819;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;2;-383.0454,150.7644;Float;True;Property;_OpacityMask;Opacity Mask;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-127.0566,-160.8805;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;81.53719,-80.13137;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Enemy Projectile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Translucent;0.5;True;True;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;18;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;True;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;39;0;38;0
WireConnection;39;1;37;0
WireConnection;60;0;62;0
WireConnection;60;1;61;0
WireConnection;40;0;36;0
WireConnection;41;0;39;0
WireConnection;41;1;40;0
WireConnection;42;1;60;0
WireConnection;22;0;25;0
WireConnection;43;0;42;0
WireConnection;43;1;41;0
WireConnection;20;0;12;0
WireConnection;20;1;19;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;7;1;43;0
WireConnection;24;0;23;0
WireConnection;24;1;21;0
WireConnection;63;0;35;0
WireConnection;63;1;64;0
WireConnection;63;2;7;0
WireConnection;8;0;6;0
WireConnection;8;1;1;0
WireConnection;8;2;7;0
WireConnection;2;1;24;0
WireConnection;34;0;8;0
WireConnection;34;1;63;0
WireConnection;0;2;34;0
WireConnection;0;9;2;0
ASEEND*/
//CHKSM=01AB79DFD0FD027F37BA9E3F2F401948005AEAEA