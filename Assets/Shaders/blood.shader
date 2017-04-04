// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "turing/blood"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		_Color1("Color 1", Color) = (1,0,0,0)
		_Color2("Color 2", Color) = (0.3970588,0.01751729,0.01751729,0)
		_BloodMask("Blood Mask", 2D) = "white" {}
		_DetailMask("Detail Mask", 2D) = "white" {}
		_BloodSmoothness1("Blood Smoothness 1", Range( 0 , 2)) = 0.5
		_BloodSmoothness2("Blood Smoothness 2", Range( 0 , 2)) = 0
		_BloodNormal("Blood Normal", 2D) = "white" {}
		_NormalStrength("Normal Strength", Range( 0 , 4)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _BloodNormal;
		uniform float4 _BloodNormal_ST;
		uniform float4 _DetailMask_ST;
		uniform float _NormalStrength;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform sampler2D _DetailMask;
		uniform float _BloodSmoothness1;
		uniform float _BloodSmoothness2;
		uniform sampler2D _BloodMask;
		uniform float4 _BloodMask_ST;
		uniform float _MaskClipValue = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BloodNormal = i.uv_texcoord * _BloodNormal_ST.xy + _BloodNormal_ST.zw;
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			o.Normal = ( ( tex2D( _BloodNormal,uv_BloodNormal) * tex2D( _DetailMask,uv_DetailMask) ) * _NormalStrength ).xyz;
			o.Albedo = lerp( _Color1 , _Color2 , tex2D( _DetailMask,uv_DetailMask).x ).rgb;
			o.Smoothness = lerp( _BloodSmoothness1 , _BloodSmoothness2 , tex2D( _DetailMask,uv_DetailMask).x );
			o.Alpha = 1;
			float2 uv_BloodMask = i.uv_texcoord * _BloodMask_ST.xy + _BloodMask_ST.zw;
			clip( tex2D( _BloodMask,uv_BloodMask) - ( _MaskClipValue ).xxxx );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5105
1929;147;1904;868;2429.734;1156.41;2.238806;True;True
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;261,-148;Float;False;True;2;Float;ASEMaterialInspector;Standard;turing/blood;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;False;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
Node;AmplifyShaderEditor.LerpOp;9;-13.80003,-668.2997;Float;False;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SamplerNode;27;-183.4985,488.9494;Float;True;Property;_BloodMask;Blood Mask;2;0;Assets/Textures/blood/Blood12_mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SamplerNode;26;-701.3987,-671.8498;Float;True;Property;_DetailMask;Detail Mask;3;0;Assets/Textures/blood/detail_mask_1.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.ColorNode;8;-374.215,-799.5001;Float;False;Property;_Color2;Color 2;1;0;0.3970588,0.01751729,0.01751729,0
Node;AmplifyShaderEditor.SamplerNode;18;-681.1275,-391.2266;Float;True;Property;_BloodNormal;Blood Normal;7;0;Assets/Textures/blood/Blood12_NRM.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SamplerNode;29;-670.2271,-170.3768;Float;True;Property;_TextureSample3;Texture Sample 3;1;0;Assets/Textures/blood/detail_mask_1.png;True;0;False;white;Auto;False;Instance;26;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-292.247,-207.6365;Float;False;0;FLOAT4;0.0,0,0,0;False;1;FLOAT4;0.0;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;5.639135,-207.156;Float;False;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False
Node;AmplifyShaderEditor.RangedFloatNode;32;-287.6442,-97.4544;Float;False;Property;_NormalStrength;Normal Strength;8;0;0;0;4
Node;AmplifyShaderEditor.ColorNode;7;-342.9822,-975.1615;Float;False;Property;_Color1;Color 1;0;0;1,0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;20;-482.6529,92.00513;Float;False;Property;_BloodSmoothness1;Blood Smoothness 1;5;0;0.5;0;2
Node;AmplifyShaderEditor.LerpOp;19;-165.7535,152.8551;Float;False;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT4;0.0;False
Node;AmplifyShaderEditor.RangedFloatNode;21;-481.9525,171.5051;Float;False;Property;_BloodSmoothness2;Blood Smoothness 2;6;0;0;0;2
Node;AmplifyShaderEditor.SamplerNode;28;-625.1049,330.9378;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Assets/Textures/blood/detail_mask_1.png;True;0;False;white;Auto;False;Instance;26;Auto;Texture2D;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False
WireConnection;0;0;9;0
WireConnection;0;1;31;0
WireConnection;0;4;19;0
WireConnection;0;10;27;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;9;2;26;0
WireConnection;30;0;18;0
WireConnection;30;1;29;0
WireConnection;31;0;30;0
WireConnection;31;1;32;0
WireConnection;19;0;20;0
WireConnection;19;1;21;0
WireConnection;19;2;28;0
ASEEND*/
//CHKSM=4A6DF6B717C749C616B9A379F541736059B823A1