// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/geyser base"
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
		_FoamTiling("Foam Tiling", Range( 1 , 10)) = 0
		_WaveOffsetMask("Wave Offset Mask", 2D) = "white" {}
		_WaveHeight("Wave Height", Range( 0 , 4)) = 0
		_WaveSpeed("Wave Speed", Range( 0 , 20)) = 0
		_crackmask("crack mask", 2D) = "white" {}
		_crack("crack", 2D) = "white" {}
		_crack_NRM("crack_NRM", 2D) = "white" {}
		_crack_DISP("crack_DISP", 2D) = "white" {}
		_crack_OCC("crack_OCC", 2D) = "white" {}
		_DebrisTint("Debris Tint", Color) = (0,0,0,0)
		_crackwatermask("crack water mask", 2D) = "white" {}
		_RubbleHeight("Rubble Height", Range( 0 , 5)) = 0
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

		uniform sampler2D _crack_NRM;
		uniform float4 _crack_NRM_ST;
		uniform sampler2D _crack;
		uniform float4 _crack_ST;
		uniform float4 _DebrisTint;
		uniform float4 _WaterColorTint;
		uniform sampler2D _FoamTexture;
		uniform float _FoamTiling;
		uniform sampler2D _crackmask;
		uniform float4 _crackmask_ST;
		uniform sampler2D _WaterTexture;
		uniform float _WaterTiling;
		uniform sampler2D _crackwatermask;
		uniform float4 _crackwatermask_ST;
		uniform sampler2D _crack_OCC;
		uniform float4 _crack_OCC_ST;
		uniform float _Opacity;
		uniform sampler2D _ShapeMask;
		uniform float4 _ShapeMask_ST;
		uniform sampler2D _crack_DISP;
		uniform float4 _crack_DISP_ST;
		uniform float _RubbleHeight;
		uniform sampler2D _WaveOffsetMask;
		uniform float _WaveSpeed;
		uniform float _WaveHeight;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = _FoamTiling;
			o.texcoord_0.xy = v.texcoord.xy * temp_cast_0 + float2( 0,0 );
			float2 temp_cast_1 = _WaterTiling;
			o.texcoord_1.xy = v.texcoord.xy * temp_cast_1 + float2( 0,0 );
			o.texcoord_2.xy = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
			float4 uv_crack_DISP = float4(v.texcoord * _crack_DISP_ST.xy + _crack_DISP_ST.zw, 0 ,0);
			float temp_output_26_0 = ( _Time.w / _WaveSpeed );
			float4 uv_crackwatermask = float4(v.texcoord * _crackwatermask_ST.xy + _crackwatermask_ST.zw, 0 ,0);
			float4 tex2DNode98 = tex2Dlod( _crackwatermask,uv_crackwatermask);
			v.vertex.xyz += lerp( ( tex2Dlod( _crack_DISP,uv_crack_DISP) * _RubbleHeight ) , ( ( ( tex2Dlod( _WaveOffsetMask,float4( (abs( o.texcoord_2+temp_output_26_0 * float2(-0.03,0 ))), 0.0 , 0.0 )) * float4( v.normal , 0.0 ) ) * ( tex2Dlod( _WaveOffsetMask,float4( (abs( o.texcoord_2+temp_output_26_0 * float2(0.04,0.04 ))), 0.0 , 0.0 )) * float4( v.normal , 0.0 ) ) ) * _WaveHeight ) , tex2DNode98.x ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_crack_NRM = i.uv_texcoord * _crack_NRM_ST.xy + _crack_NRM_ST.zw;
			o.Normal = tex2D( _crack_NRM,uv_crack_NRM).xyz;
			float2 uv_crack = i.uv_texcoord * _crack_ST.xy + _crack_ST.zw;
			float4 tex2DNode57 = tex2D( _FoamTexture,(abs( i.texcoord_0+_Time.y * float2(-0.03,0 ))));
			float2 uv_crackmask = i.uv_texcoord * _crackmask_ST.xy + _crackmask_ST.zw;
			float4 tex2DNode76 = tex2D( _crackmask,uv_crackmask);
			float2 uv_crackwatermask = i.uv_texcoord * _crackwatermask_ST.xy + _crackwatermask_ST.zw;
			float4 tex2DNode98 = tex2D( _crackwatermask,uv_crackwatermask);
			o.Albedo = lerp( lerp( ( tex2D( _crack,uv_crack) * _DebrisTint ) , lerp( _WaterColorTint , tex2DNode57 , tex2DNode57.x ) , tex2DNode76.x ) , ( tex2DNode76 * ( tex2D( _WaterTexture,(abs( i.texcoord_1+( _Time.y / 5.0 ) * float2(-0.03,0 )))) * _WaterColorTint ) ) , tex2DNode98.x ).rgb;
			float2 uv_crack_OCC = i.uv_texcoord * _crack_OCC_ST.xy + _crack_OCC_ST.zw;
			o.Occlusion = tex2D( _crack_OCC,uv_crack_OCC).x;
			float2 uv_ShapeMask = i.uv_texcoord * _ShapeMask_ST.xy + _ShapeMask_ST.zw;
			o.Alpha = lerp( 0.0 , _Opacity , tex2D( _ShapeMask,uv_ShapeMask).x );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1927;90;1904;868;2114.188;1441.05;2.531343;True;True
Node;AmplifyShaderEditor.CommentaryNode;16;-1599.219,423.9948;Float;False;1637.442;763.1202;Wave Tessellation;15;31;30;29;28;27;26;25;24;23;22;21;20;19;18;17;
Node;AmplifyShaderEditor.TimeNode;17;-1549.033,817.1689;Float;False
Node;AmplifyShaderEditor.NormalVertexDataNode;18;-756.8416,1030.338;Float;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-547.8412,923.7372;Float;False;0;FLOAT4;0.0,0,0;False;1;FLOAT3;0.0,0,0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-438.3421,700.4696;Float;False;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-591.8459,498.0706;Float;False;0;FLOAT4;0.0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False
Node;AmplifyShaderEditor.NormalVertexDataNode;22;-778.7454,670.7707;Float;False
Node;AmplifyShaderEditor.PannerNode;24;-1130.745,744.6957;Float;False;0.04;0.04;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-1300.238,683.9684;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.PannerNode;23;-1134.645,596.9957;Float;False;-0.03;0;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-1539.257,617.9134;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.SamplerNode;30;-930.1393,825.4703;Float;True;Property;_TextureSample3;Texture Sample 3;18;0;None;True;0;False;white;Auto;False;Instance;28;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.RangedFloatNode;27;-1540.032,991.7695;Float;False;Property;_WaveSpeed;Wave Speed;10;0;0;0;20
Node;AmplifyShaderEditor.RangedFloatNode;43;-722.4077,1.58627;Float;False;Property;_Opacity;Opacity;1;0;1;0;1
Node;AmplifyShaderEditor.RangedFloatNode;29;-417.5451,581.1699;Float;False;Property;_WaveHeight;Wave Height;9;0;0;0;4
Node;AmplifyShaderEditor.SamplerNode;28;-928.5453,470.1699;Float;True;Property;_WaveOffsetMask;Wave Offset Mask;8;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Sand/Sand_height.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-181.5442,460.2695;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0.0,0,0,0;False
Node;AmplifyShaderEditor.SamplerNode;57;148.3309,-1427.788;Float;True;Property;_FoamTexture;Foam Texture;5;0;Assets/Textures/SimpleFoam.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.PannerNode;69;-89.64926,-1444.081;Float;False;-0.03;0;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.TimeNode;70;-558.7911,-1512.89;Float;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-339.3841,-1720.193;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;59;-710.5298,-1720.193;Float;False;Property;_FoamTiling;Foam Tiling;7;0;0;1;10
Node;AmplifyShaderEditor.LerpOp;64;429.4866,-1237.657;Float;False;0;COLOR;0.0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.LerpOp;55;824.4776,-810.0955;Float;False;0;FLOAT4;0,0,0,0;False;1;COLOR;0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SamplerNode;79;196.0644,-851.2407;Float;True;Property;_crack;crack;14;0;Assets/Textures/crack.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.LerpOp;85;675.2642,-1060.541;Float;False;0;COLOR;0,0,0,0;False;1;FLOAT4;0.0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;522.8667,-794.9395;Float;False;0;FLOAT4;0,0,0,0;False;1;COLOR;0.0,0,0,0;False
Node;AmplifyShaderEditor.ColorNode;88;255.3667,-644.9395;Float;False;Property;_DebrisTint;Debris Tint;18;0;0,0,0,0
Node;AmplifyShaderEditor.SamplerNode;56;130.6856,-473.6625;Float;True;Property;_FoamMask;Foam Mask;6;0;Assets/Textures/geyser target mask inverse.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SamplerNode;33;-1069.587,-287.4576;Float;True;Property;_ShapeMask;Shape Mask;0;0;Assets/Textures/geyser target mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.RangedFloatNode;42;-726.8077,-84.21374;Float;False;Constant;_Float0;Float 0;7;0;0;0;1
Node;AmplifyShaderEditor.SamplerNode;98;135.2684,489.0643;Float;True;Property;_crackwatermask;crack water mask;19;0;Assets/Textures/crack water mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SamplerNode;82;-141.8351,55.05964;Float;True;Property;_crack_DISP;crack_DISP;16;0;Assets/Textures/crack_DISP.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.RangedFloatNode;100;-117.5316,247.4641;Float;False;Property;_RubbleHeight;Rubble Height;20;0;0;0;5
Node;AmplifyShaderEditor.SamplerNode;84;734.3645,543.4599;Float;True;Property;_crack_OCC;crack_OCC;17;0;Assets/Textures/crack_OCC.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SamplerNode;80;461.0648,186.1602;Float;True;Property;_crack_NRM;crack_NRM;15;0;Assets/Textures/crack_NRM.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;7.5;False
Node;AmplifyShaderEditor.RangedFloatNode;74;-269.342,-277.0721;Float;False;Property;_WaterSmoothness;Water Smoothness;11;0;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;75;-269.3421,-195.7189;Float;False;Property;_FoamSmoothness;Foam Smoothness;11;0;0;0;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-835.5345,-962.0955;Float;False;0;-1;2;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False
Node;AmplifyShaderEditor.PannerNode;45;-563.9805,-741.222;Float;False;-0.03;0;0;FLOAT2;0,0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.TimeNode;48;-1033.126,-810.0309;Float;False
Node;AmplifyShaderEditor.SimpleDivideOpNode;66;-787.6376,-755.8201;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;67;-985.7208,-603.6098;Float;False;Constant;_Float1;Float 1;11;0;5;0;0
Node;AmplifyShaderEditor.SamplerNode;44;-358.8202,-805.6227;Float;True;Property;_WaterTexture;Water Texture;3;0;Assets/Textures/water texture.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.ColorNode;49;-386.7792,-590.4078;Float;False;Property;_WaterColorTint;Water Color Tint;2;0;0.6176471,0.620284,1,0
Node;AmplifyShaderEditor.RangedFloatNode;68;-1128.73,-1099.837;Float;False;Property;_WaterTiling;Water Tiling;4;0;0;0.1;10
Node;AmplifyShaderEditor.SamplerNode;76;-415.5811,-1102.056;Float;True;Property;_crackmask;crack mask;13;0;Assets/Textures/crack mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;56.77713,-934.785;Float;False;0;FLOAT4;0,0,0,0;False;1;COLOR;0.0,0,0,0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;327.0291,-1017.783;Float;False;0;FLOAT4;0.0,0,0,0;False;1;COLOR;0,0,0,0;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;962.5742,-383.621;Float;False;True;2;Float;ASEMaterialInspector;Standard;turing/geyser base;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;6;False;0;0,0,0,0;VertexOffset;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.LerpOp;73;515.728,-319.4268;Float;False;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.LerpOp;40;-39.90068,-110.766;Float;False;0;FLOAT;0.0;False;1;FLOAT;0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;216.8685,26.6639;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False
Node;AmplifyShaderEditor.LerpOp;83;456.7652,17.85968;Float;False;0;FLOAT4;0.0;False;1;FLOAT4;0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.TimeNode;102;-1319.346,-405.7308;Float;False
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
WireConnection;23;0;25;0
WireConnection;23;1;26;0
WireConnection;30;1;24;0
WireConnection;28;1;23;0
WireConnection;31;0;20;0
WireConnection;31;1;29;0
WireConnection;57;1;69;0
WireConnection;69;0;58;0
WireConnection;69;1;70;2
WireConnection;58;0;59;0
WireConnection;64;0;49;0
WireConnection;64;1;57;0
WireConnection;64;2;57;0
WireConnection;55;0;85;0
WireConnection;55;1;34;0
WireConnection;55;2;98;0
WireConnection;85;0;87;0
WireConnection;85;1;64;0
WireConnection;85;2;76;0
WireConnection;87;0;79;0
WireConnection;87;1;88;0
WireConnection;46;0;68;0
WireConnection;45;0;46;0
WireConnection;45;1;66;0
WireConnection;66;0;48;2
WireConnection;66;1;67;0
WireConnection;44;1;45;0
WireConnection;50;0;44;0
WireConnection;50;1;49;0
WireConnection;34;0;76;0
WireConnection;34;1;50;0
WireConnection;0;0;55;0
WireConnection;0;1;80;0
WireConnection;0;5;84;0
WireConnection;0;9;40;0
WireConnection;0;11;83;0
WireConnection;73;0;74;0
WireConnection;73;1;75;0
WireConnection;73;2;56;0
WireConnection;40;0;42;0
WireConnection;40;1;43;0
WireConnection;40;2;33;0
WireConnection;99;0;82;0
WireConnection;99;1;100;0
WireConnection;83;0;99;0
WireConnection;83;1;31;0
WireConnection;83;2;98;0
ASEEND*/
//CHKSM=931F0888D2321DBAC7767F98FA361C0716C28E8E