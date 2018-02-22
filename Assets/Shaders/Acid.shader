// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Acid"
{
	Properties
	{
		_AcidColor1("Acid Color 1", Color) = (0.0514706,0.2573529,0,0)
		_AcidColor2("Acid Color 2", Color) = (0.1911765,0.9558824,0,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_clouds("clouds", 2D) = "white" {}
		_MaskDistortion("Mask Distortion", Range( 0 , 5)) = 0.15
		_AcidMask("Acid Mask", 2D) = "white" {}
		_DistortionTexture("Distortion Texture", 2D) = "white" {}
		_EmissionStrength("Emission Strength", Range( 0 , 10)) = 1
		_CloudTextureTiling("Cloud Texture Tiling", Range( 1 , 10)) = 2
		_CloudsDistortion("Clouds Distortion", Range( 0 , 10)) = 3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _AcidColor1;
		uniform float4 _AcidColor2;
		uniform sampler2D _clouds;
		uniform float _CloudTextureTiling;
		uniform sampler2D _DistortionTexture;
		uniform float _CloudsDistortion;
		uniform float _EmissionStrength;
		uniform sampler2D _AcidMask;
		uniform float _MaskDistortion;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_CloudTextureTiling).xx;
			float2 uv_TexCoord19 = i.uv_texcoord * temp_cast_0 + float2( 0,0 );
			float2 uv_TexCoord4 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner9 = ( uv_TexCoord4 + 1.0 * _Time.y * float2( -0.085,0.0785 ));
			float2 panner7 = ( uv_TexCoord4 + 1.0 * _Time.y * float2( 0.0784,-0.0954 ));
			float4 appendResult10 = (float4(tex2D( _DistortionTexture, panner9 ).g , tex2D( _DistortionTexture, panner7 ).b , 0.0 , 0.0));
			float4 lerpResult17 = lerp( _AcidColor1 , _AcidColor2 , tex2D( _clouds, ( float4( uv_TexCoord19, 0.0 , 0.0 ) + ( appendResult10 * _CloudsDistortion ) ).xy ).r);
			o.Albedo = lerpResult17.rgb;
			o.Emission = ( lerpResult17 * _EmissionStrength ).rgb;
			o.Alpha = 1;
			float2 uv_TexCoord13 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner26 = ( float2( 0,10 ) + 1.0 * _Time.y * float2( 0.1,0 ));
			clip( tex2D( _AcidMask, ( ( float4( uv_TexCoord13, 0.0 , 0.0 ) + ( appendResult10 * _MaskDistortion ) ) + float4( panner26, 0.0 , 0.0 ) ).xy ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13903
1967;96;1768;977;1603.369;320.5872;1;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2331.714,-122.9419;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;9;-2046.191,-279.0788;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.085,0.0785;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;7;-2041.619,33.81573;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.0784,-0.0954;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SamplerNode;5;-1837.931,-307.8115;Float;True;Property;_DistortionTexture;Distortion Texture;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-1839.513,5.317534;Float;True;Property;_TextureSample0;Texture Sample 0;6;0;None;True;0;False;white;Auto;False;Instance;5;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1482.433,-90.84496;Float;True;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;24;-1433.529,-550.8542;Float;False;Property;_CloudsDistortion;Clouds Distortion;9;0;3;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-1809.045,-789.3501;Float;False;Property;_CloudTextureTiling;Cloud Texture Tiling;8;0;2;1;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1116.068,-566.9614;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;12;-1244.653,93.02402;Float;False;Property;_MaskDistortion;Mask Distortion;4;0;0.15;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-1436.819,-868.7103;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-1039.324,-329.0325;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1019.494,-58.68808;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-853.468,-617.0403;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;18;-508.6307,-662.6821;Float;False;Property;_AcidColor2;Acid Color 2;1;0;0.1911765,0.9558824,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PannerNode;26;-758.3605,165.6865;Float;False;3;0;FLOAT2;0,10;False;2;FLOAT2;0.1,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-713.4067,-119.8384;Float;False;2;2;0;FLOAT2;0,0,0,0;False;1;FLOAT4;0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.ColorNode;3;-515.2928,-829.4378;Float;False;Property;_AcidColor1;Acid Color 1;0;0;0.0514706,0.2573529,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-529.7933,-477.5383;Float;True;Property;_clouds;clouds;3;0;Assets/Textures/VFX/clouds.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;22;-33.96646,-310.3652;Float;False;Property;_EmissionStrength;Emission Strength;7;0;1;0;10;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;17;-2.530717,-698.5826;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-473.4958,-21.20842;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;2;-261.0136,-105.0919;Float;True;Property;_AcidMask;Acid Mask;5;0;Assets/Textures/VFX/acid-mask.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;239.5729,-320.6409;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;550.2138,-275.206;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Acid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;0;0;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;4;0
WireConnection;7;0;4;0
WireConnection;5;1;9;0
WireConnection;6;1;7;0
WireConnection;10;0;5;2
WireConnection;10;1;6;3
WireConnection;23;0;10;0
WireConnection;23;1;24;0
WireConnection;19;0;25;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;20;0;19;0
WireConnection;20;1;23;0
WireConnection;16;0;13;0
WireConnection;16;1;11;0
WireConnection;1;1;20;0
WireConnection;17;0;3;0
WireConnection;17;1;18;0
WireConnection;17;2;1;0
WireConnection;32;0;16;0
WireConnection;32;1;26;0
WireConnection;2;1;32;0
WireConnection;21;0;17;0
WireConnection;21;1;22;0
WireConnection;0;0;17;0
WireConnection;0;2;21;0
WireConnection;0;10;2;0
ASEEND*/
//CHKSM=ACBD7F10756495D1D2582A7B26E01892E88BC5C4