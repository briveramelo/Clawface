// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water Projectile Ground Trail"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (0,0.3379312,1,0)
		_DetailColor("Detail Color", Color) = (0.7426471,0.8509127,1,0)
		_OpacityMask("Opacity Mask", 2D) = "white" {}
		_ScrollSpeed("Scroll Speed", Range( 0 , 5)) = 0.5
		_DetailMask("Detail Mask", 2D) = "white" {}
		_DetailMovementDirection("Detail Movement Direction", Float) = -0.5
		_DetailSpeed("Detail Speed", Range( 0 , 3)) = 0.5
		_DetailTiling("Detail Tiling", Range( 1 , 10)) = 1
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

		uniform float4 _DetailColor;
		uniform float4 _BaseColor;
		uniform sampler2D _DetailMask;
		uniform float _DetailTiling;
		uniform float _DetailSpeed;
		uniform float _DetailMovementDirection;
		uniform float _ScrollSpeed;
		uniform sampler2D _OpacityMask;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_DetailTiling).xx;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
			o.texcoord_1.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime77 = _Time.y * _DetailSpeed;
			float2 temp_cast_1 = (_DetailMovementDirection).xx;
			float2 panner74 = ( float2( 0,0 ) + mulTime77 * temp_cast_1);
			float4 appendResult20 = (float4(-1.0 , 0.0 , 0.0 , 0.0));
			float mulTime22 = _Time.y * _ScrollSpeed;
			float4 temp_output_24_0 = ( float4( i.texcoord_1, 0.0 , 0.0 ) + ( appendResult20 * mulTime22 ) );
			float4 lerpResult70 = lerp( _DetailColor , _BaseColor , tex2D( _DetailMask, ( float4( i.texcoord_0, 0.0 , 0.0 ) + ( float4( panner74, 0.0 , 0.0 ) + temp_output_24_0 ) ).xy ));
			o.Emission = lerpResult70.rgb;
			o.Alpha = tex2D( _OpacityMask, temp_output_24_0.xy ).r;
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
1999;84;1767;1004;2314.412;829.1628;2.13578;True;True
Node;AmplifyShaderEditor.RangedFloatNode;25;-1380.215,456.3392;Float;False;Property;_ScrollSpeed;Scroll Speed;3;0;0.5;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-1250.874,247.9335;Float;False;Constant;_XRotation;X Rotation;3;0;-1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;19;-1249.057,330.9086;Float;False;Constant;_YRotation;Y Rotation;3;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;78;-1514.722,-34.1521;Float;False;Property;_DetailSpeed;Detail Speed;5;0;0.5;0;3;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;22;-1035.613,476.187;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1040.816,277.2872;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-767.8168,363.9874;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;76;-1339.652,-195.8245;Float;False;Property;_DetailMovementDirection;Detail Movement Direction;5;0;-0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-1029.215,110.0871;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;77;-1221.069,-82.45458;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;74;-950.8035,-191.4097;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-623.6008,237.8801;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;81;-1299.171,-366.4768;Float;False;Property;_DetailTiling;Detail Tiling;6;0;1;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-757.9135,-107.8938;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.TextureCoordinatesNode;80;-919.8339,-365.1735;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;79;-582.2142,-99.24828;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;71;-527.5919,-346.7204;Float;False;Property;_BaseColor;Base Color;0;0;0,0.3379312,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;66;-531.4255,-520.3295;Float;False;Property;_DetailColor;Detail Color;1;0;0.7426471,0.8509127,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;69;-452.1928,-115.3197;Float;True;Property;_DetailMask;Detail Mask;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;70;-150.5929,-211.5198;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;2;-377.8447,178.0645;Float;True;Property;_OpacityMask;Opacity Mask;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;507.8069,-106.0429;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Water Projectile Ground Trail;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Translucent;0.5;True;True;0;False;Opaque;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;18;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;True;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;25;0
WireConnection;20;0;12;0
WireConnection;20;1;19;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;77;0;78;0
WireConnection;74;2;76;0
WireConnection;74;1;77;0
WireConnection;24;0;23;0
WireConnection;24;1;21;0
WireConnection;75;0;74;0
WireConnection;75;1;24;0
WireConnection;80;0;81;0
WireConnection;79;0;80;0
WireConnection;79;1;75;0
WireConnection;69;1;79;0
WireConnection;70;0;66;0
WireConnection;70;1;71;0
WireConnection;70;2;69;0
WireConnection;2;1;24;0
WireConnection;0;2;70;0
WireConnection;0;9;2;0
ASEEND*/
//CHKSM=BE1C230D02A182E687D2F7801251BA006CD6A565