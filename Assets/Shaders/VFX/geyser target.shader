// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/geyser target"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_ShapeMask("Shape Mask", 2D) = "white" {}
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_WaterColorTint("Water Color Tint", Color) = (0.6176471,0.620284,1,0)
		_WaterTexture("Water Texture", 2D) = "white" {}
		_WaterTiling("Water Tiling", Range( 0.1 , 10)) = 0
		_FoamTexture("Foam Texture", 2D) = "white" {}
		_FoamMask("Foam Mask", 2D) = "white" {}
		_FoamTiling("Foam Tiling", Range( 1 , 10)) = 0
		_WaveOffsetMask("Wave Offset Mask", 2D) = "white" {}
		_WaveHeight("Wave Height", Range( 0 , 4)) = 0
		_WaveSpeed("Wave Speed", Range( 0 , 20)) = 0
		_FoamSmoothness("Foam Smoothness", Range( 0 , 1)) = 0
		_WaterSmoothness("Water Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha  vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float2 texcoord_0;
			float2 texcoord_1;
			float2 texcoord_2;
		};

		uniform sampler2D _ShapeMask;
		uniform float4 _ShapeMask_ST;
		uniform sampler2D _WaterTexture;
		uniform float _WaterTiling;
		uniform float4 _WaterColorTint;
		uniform sampler2D _FoamTexture;
		uniform float _FoamTiling;
		uniform sampler2D _FoamMask;
		uniform float4 _FoamMask_ST;
		uniform float _WaterSmoothness;
		uniform float _FoamSmoothness;
		uniform float _Opacity;
		uniform sampler2D _WaveOffsetMask;
		uniform float _WaveSpeed;
		uniform float _WaveHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = _WaterTiling;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
			float2 temp_cast_1 = _FoamTiling;
			o.texcoord_1.xy = v.texcoord.xy * temp_cast_1 + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float temp_output_26_0 = ( _Time.w / _WaveSpeed );
			v.vertex.xyz += ( ( ( tex2Dlod( _WaveOffsetMask,float4( (abs( o.texcoord_2+temp_output_26_0 * float2(-0.03,0 ))), 0.0 , 0.0 )) * float4( v.normal , 0.0 ) ) * ( tex2Dlod( _WaveOffsetMask,float4( (abs( o.texcoord_2+temp_output_26_0 * float2(0.04,0.04 ))), 0.0 , 0.0 )) * float4( v.normal , 0.0 ) ) ) * _WaveHeight ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ShapeMask = i.uv_texcoord * _ShapeMask_ST.xy + _ShapeMask_ST.zw;
			float4 tex2DNode33 = tex2D( _ShapeMask,uv_ShapeMask);
			float4 tex2DNode57 = tex2D( _FoamTexture,(abs( i.texcoord_1+_Time.y * float2(-0.03,0 ))));
			float2 uv_FoamMask = i.uv_texcoord * _FoamMask_ST.xy + _FoamMask_ST.zw;
			float4 tex2DNode56 = tex2D( _FoamMask,uv_FoamMask);
			o.Albedo = lerp( ( tex2DNode33 * ( tex2D( _WaterTexture,(abs( i.texcoord_0+( _Time.y / 5.0 ) * float2(-0.03,0 )))) * _WaterColorTint ) ) , lerp( _WaterColorTint , tex2DNode57 , tex2DNode57.x ) , tex2DNode56.x ).xyz;
			o.Smoothness = lerp( _WaterSmoothness , _FoamSmoothness , tex2DNode56.x );
			o.Alpha = lerp( 0.0 , _Opacity , tex2DNode33.x );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1927;167;1904;868;2823.134;1803.544;3.1;True;True
Node;AmplifyShaderEditor.CommentaryNode;16;-1416.192,164.9366;Float;False;1637.442;763.1202;Wave Tessellation;15;31;30;29;28;27;26;25;24;23;22;21;20;19;18;17;
Node;AmplifyShaderEditor.TimeNode;17;-1366.007,558.1102;Float;False
Node;AmplifyShaderEditor.NormalVertexDataNode;18;-573.8145,771.2786;Float;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-364.8144,664.6786;Float;False;0;FLOAT4;0.0,0,0;False;1;FLOAT3;0.0,0,0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-255.3152,441.4111;Float;False;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-408.819,239.0121;Float;False;0;FLOAT4;0.0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False
Node;AmplifyShaderEditor.NormalVertexDataNode;22;-595.7183,411.7122;Float;False
Node;AmplifyShaderEditor.PannerNode;24;-947.7177,485.6371;Float;False;0.04;0.04;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-1117.211,424.9099;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;1.482665,201.2112;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False
Node;AmplifyShaderEditor.PannerNode;23;-951.6177,337.9373;Float;False;-0.03;0;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-1356.231,358.8549;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;739.8663,-315.2255;Float;False;True;2;Float;ASEMaterialInspector;Standard;turing/geyser target;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.SamplerNode;30;-747.1122,566.4117;Float;True;Property;_TextureSample3;Texture Sample 3;18;0;None;True;0;False;white;Auto;False;Instance;28;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.RangedFloatNode;27;-1357.005,732.7109;Float;False;Property;_WaveSpeed;Wave Speed;10;0;0;0;20
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-122.7347,-462.7633;Float;False;0;FLOAT4;0,0,0,0;False;1;COLOR;0.0,0,0,0;False
Node;AmplifyShaderEditor.LerpOp;40;-194.4083,-64.41371;Float;False;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;42;-726.8077,-84.21374;Float;False;Constant;_Float0;Float 0;7;0;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;43;-722.4077,1.58627;Float;False;Property;_Opacity;Opacity;1;0;1;0;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1.321523,-645.6619;Float;False;0;FLOAT4;0.0;False;1;COLOR;0,0,0,0;False
Node;AmplifyShaderEditor.SamplerNode;33;-636.287,-808.0551;Float;True;Property;_ShapeMask;Shape Mask;0;0;Assets/Textures/geyser target mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.LerpOp;64;-35.71304,-911.0562;Float;False;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SamplerNode;57;-363.0692,-1129.988;Float;True;Property;_FoamTexture;Foam Texture;5;0;Assets/Textures/SimpleFoam.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.ColorNode;49;-541.8344,-402.2962;Float;False;Property;_WaterColorTint;Water Color Tint;2;0;0.6176471,0.620284,1,0
Node;AmplifyShaderEditor.SamplerNode;44;-618.9752,-597.0112;Float;True;Property;_WaterTexture;Water Texture;3;0;Assets/Textures/water texture.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-1176.19,-900.4843;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;68;-1469.684,-1028.127;Float;False;Property;_WaterTiling;Water Tiling;4;0;0;0.1;10
Node;AmplifyShaderEditor.PannerNode;45;-904.6363,-679.6104;Float;False;-0.03;0;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.TimeNode;48;-1373.782,-748.4193;Float;False
Node;AmplifyShaderEditor.SimpleDivideOpNode;66;-1128.293,-694.2085;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;67;-1326.377,-541.9982;Float;False;Constant;_Float1;Float 1;11;0;5;0;0
Node;AmplifyShaderEditor.PannerNode;69;-616.7494,-1250.381;Float;False;-0.03;0;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.TimeNode;70;-1085.891,-1319.19;Float;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-866.4841,-1526.493;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;59;-1237.63,-1526.493;Float;False;Property;_FoamTiling;Foam Tiling;7;0;0;1;10
Node;AmplifyShaderEditor.LerpOp;55;429.3776,-787.9956;Float;False;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SamplerNode;56;76.28568,-488.6622;Float;True;Property;_FoamMask;Foam Mask;6;0;Assets/Textures/geyser target mask inverse.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.LerpOp;73;470.128,-276.3102;Float;False;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;74;108.1059,-270.2094;Float;False;Property;_WaterSmoothness;Water Smoothness;11;0;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;75;108.1058,-188.8562;Float;False;Property;_FoamSmoothness;Foam Smoothness;11;0;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;29;-234.5183,322.1115;Float;False;Property;_WaveHeight;Wave Height;9;0;0;0;4
Node;AmplifyShaderEditor.SamplerNode;28;-745.5182,211.1116;Float;True;Property;_WaveOffsetMask;Wave Offset Mask;8;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Sand/Sand_height.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
WireConnection;19;0;30;0
WireConnection;19;1;18;0
WireConnection;20;0;21;0
WireConnection;20;1;19;0
WireConnection;21;0;28;0
WireConnection;21;1;22;0
WireConnection;24;0;25;0
WireConnection;24;1;26;0
WireConnection;26;0;17;4
WireConnection;26;1;27;0
WireConnection;31;0;20;0
WireConnection;31;1;29;0
WireConnection;23;0;25;0
WireConnection;23;1;26;0
WireConnection;0;0;55;0
WireConnection;0;4;73;0
WireConnection;0;9;40;0
WireConnection;0;11;31;0
WireConnection;30;1;24;0
WireConnection;50;0;44;0
WireConnection;50;1;49;0
WireConnection;40;0;42;0
WireConnection;40;1;43;0
WireConnection;40;2;33;0
WireConnection;34;0;33;0
WireConnection;34;1;50;0
WireConnection;64;0;49;0
WireConnection;64;1;57;0
WireConnection;64;2;57;0
WireConnection;57;1;69;0
WireConnection;44;1;45;0
WireConnection;46;0;68;0
WireConnection;45;0;46;0
WireConnection;45;1;66;0
WireConnection;66;0;48;2
WireConnection;66;1;67;0
WireConnection;69;0;58;0
WireConnection;69;1;70;2
WireConnection;58;0;59;0
WireConnection;55;0;34;0
WireConnection;55;1;64;0
WireConnection;55;2;56;0
WireConnection;73;0;74;0
WireConnection;73;1;75;0
WireConnection;73;2;56;0
WireConnection;28;1;23;0
ASEEND*/
//CHKSM=C36A56E953D23F67E6CA023320DE385A50580D67