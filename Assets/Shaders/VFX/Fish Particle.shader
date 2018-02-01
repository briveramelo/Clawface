// Upgrade NOTE: upgraded instancing buffer 'FishParticle' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fish Particle"
{
	Properties
	{
		_Fish_Smoothness("Fish_Smoothness", 2D) = "white" {}
		_Fish_Albedo("Fish_Albedo", 2D) = "white" {}
		_Smoothness("Smoothness", Float) = 1.2
		_FloppingRate("Flopping Rate", Float) = 1.68
		_FloppingDistance("Flopping Distance", Float) = 0.76
		_FloppingSpeed("Flopping Speed", Float) = 16.03
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Fish_Albedo;
		uniform float4 _Fish_Albedo_ST;
		uniform sampler2D _Fish_Smoothness;
		uniform float4 _Fish_Smoothness_ST;

		UNITY_INSTANCING_BUFFER_START(FishParticle)
			UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
#define _Smoothness_arr FishParticle
			UNITY_DEFINE_INSTANCED_PROP(float, _FloppingSpeed)
#define _FloppingSpeed_arr FishParticle
			UNITY_DEFINE_INSTANCED_PROP(float, _FloppingRate)
#define _FloppingRate_arr FishParticle
			UNITY_DEFINE_INSTANCED_PROP(float, _FloppingDistance)
#define _FloppingDistance_arr FishParticle
		UNITY_INSTANCING_BUFFER_END(FishParticle)

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float _FloppingSpeed_Instance = UNITY_ACCESS_INSTANCED_PROP(_FloppingSpeed_arr, _FloppingSpeed);
			float3 ase_vertex3Pos = v.vertex.xyz;
			float _FloppingRate_Instance = UNITY_ACCESS_INSTANCED_PROP(_FloppingRate_arr, _FloppingRate);
			float4 appendResult20 = (float4(0.0 , 0.0 , sin( ( ( _Time.y * _FloppingSpeed_Instance ) + ( ase_vertex3Pos.x * _FloppingRate_Instance ) ) ) , 0.0));
			float _FloppingDistance_Instance = UNITY_ACCESS_INSTANCED_PROP(_FloppingDistance_arr, _FloppingDistance);
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			v.vertex.xyz += ( appendResult20 * _FloppingDistance_Instance * float4( ase_objectScale , 0.0 ) ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Fish_Albedo = i.uv_texcoord * _Fish_Albedo_ST.xy + _Fish_Albedo_ST.zw;
			o.Albedo = tex2D( _Fish_Albedo, uv_Fish_Albedo ).rgb;
			float2 uv_Fish_Smoothness = i.uv_texcoord * _Fish_Smoothness_ST.xy + _Fish_Smoothness_ST.zw;
			float _Smoothness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Smoothness_arr, _Smoothness);
			o.Smoothness = ( tex2D( _Fish_Smoothness, uv_Fish_Smoothness ) * _Smoothness_Instance ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13903
2065;96;1768;977;1647.29;344.1818;1.3;True;True
Node;AmplifyShaderEditor.RangedFloatNode;25;-1129.347,369.6139;Float;False;InstancedProperty;_FloppingSpeed;Flopping Speed;5;0;16.03;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;29;-1227.616,597.4086;Float;False;InstancedProperty;_FloppingRate;Flopping Rate;3;0;1.68;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleTimeNode;26;-1105.16,279.8528;Float;False;1;0;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;24;-1243.945,454.1527;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-999.7451,511.2539;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-923.4017,309.5213;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-765.8493,384.8689;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SinOpNode;23;-604.0825,333.9424;Float;True;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-635,-63.5;Float;True;Property;_Fish_Smoothness;Fish_Smoothness;0;0;Assets/Textures/VFX/Fish_Smoothness.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;2;-385.985,133.2001;Float;False;InstancedProperty;_Smoothness;Smoothness;2;0;1.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;20;-399.6539,257.5772;Float;True;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.ObjectScaleNode;33;-378.8747,613.6856;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;22;-413.8867,529.3311;Float;False;InstancedProperty;_FloppingDistance;Flopping Distance;4;0;0.76;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-169,18.5;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-164.5878,252.4314;Float;True;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT3;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;1;-328.1001,-261.4;Float;True;Property;_Fish_Albedo;Fish_Albedo;1;0;Assets/Textures/VFX/Fish_Albedo.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;167,-68;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Fish Particle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;24;1
WireConnection;27;1;29;0
WireConnection;31;0;26;0
WireConnection;31;1;25;0
WireConnection;30;0;31;0
WireConnection;30;1;27;0
WireConnection;23;0;30;0
WireConnection;20;2;23;0
WireConnection;4;0;3;0
WireConnection;4;1;2;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;21;2;33;0
WireConnection;0;0;1;0
WireConnection;0;4;4;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=E767BCD0CC2CF65839D575C1F8EB8DE653D4829B