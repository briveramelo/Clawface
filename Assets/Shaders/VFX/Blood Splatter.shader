// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blood Splatter"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_BloodColor("Blood Color", Color) = (0.5955882,0.01751732,0.01751732,0)
		_BloodHighlightColor("Blood Highlight Color", Color) = (1,0.8602941,0.8602941,0)
		_BloodSplat1("Blood Splat 1", 2D) = "white" {}
		_BloodSplat1_Normal("Blood Splat 1_Normal", 2D) = "bump" {}
		//_PlaybackSpeed("Playback Speed", Float) = 24
		_BloodSmoothness("Blood Smoothness", Float) = 0.5
		_BloodHighlightSmoothness("Blood Highlight Smoothness", Float) = 0.8
		_fbcurrenttileindex6("Linear Tile Index", Float)=0.0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf Standard keepalpha 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _BloodSplat1_Normal;
		//uniform float _PlaybackSpeed;
		uniform float4 _BloodHighlightColor;
		uniform float4 _BloodColor;
		uniform sampler2D _BloodSplat1;
		uniform float _BloodHighlightSmoothness;
		uniform float _BloodSmoothness;
		uniform float _Cutoff = 0.5;
		uniform float _fbcurrenttileindex6;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord10 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			// *** BEGIN Flipbook UV Animation vars ***
			// Total tiles of Flipbook Texture
			float fbtotaltiles6 = 4.0 * 4.0;
			// Offsets for cols and rows of Flipbook Texture
			float fbcolsoffset6 = 1.0f / 4.0;
			float fbrowsoffset6 = 1.0f / 4.0;
			// Speed of animation
			//float fbspeed6 = _Time[ 1 ] * _PlaybackSpeed;
			// UV Tiling (col and row offset)
			float2 fbtiling6 = float2(fbcolsoffset6, fbrowsoffset6);
			// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
			// Calculate current tile linear index
			//float fbcurrenttileindex6 = round( fmod( fbspeed6 + 0.0, fbtotaltiles6) );
			//fbcurrenttileindex6 += ( fbcurrenttileindex6 < 0) ? fbtotaltiles6 : 0;


			// Obtain Offset X coordinate from current tile linear index
			float fblinearindextox6 = round ( fmod ( _fbcurrenttileindex6, 4.0 ) );
			// Multiply Offset X by coloffset
			float fboffsetx6 = fblinearindextox6 * fbcolsoffset6;
			// Obtain Offset Y coordinate from current tile linear index
			float fblinearindextoy6 = round( fmod( ( _fbcurrenttileindex6 - fblinearindextox6 ) / 4.0, 4.0 ) );
			// Reverse Y to get tiles from Top to Bottom
			fblinearindextoy6 = (int)(4.0-1) - fblinearindextoy6;
			// Multiply Offset Y by rowoffset
			float fboffsety6 = fblinearindextoy6 * fbrowsoffset6;
			// UV Offset
			float2 fboffset6 = float2(fboffsetx6, fboffsety6);
			// Flipbook UV
			half2 fbuv6 = uv_TexCoord10 * fbtiling6 + fboffset6;
			// *** END Flipbook UV Animation vars ***
			o.Normal = UnpackNormal( tex2D( _BloodSplat1_Normal, fbuv6 ) );
			float4 tex2DNode4 = tex2D( _BloodSplat1, fbuv6 );
			float4 lerpResult11 = lerp( _BloodHighlightColor , _BloodColor , tex2DNode4.r);
			o.Albedo = lerpResult11.rgb;
			float lerpResult22 = lerp( _BloodHighlightSmoothness , _BloodSmoothness , tex2DNode4.r);
			o.Smoothness = lerpResult22;
			o.Alpha = 1;
			clip( tex2DNode4.r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13903
1974;51;1768;977;1457.103;887.1586;1.6;True;True
Node;AmplifyShaderEditor.RangedFloatNode;7;-924.8297,-260.2206;Float;False;Constant;_Float0;Float 0;2;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-930.8017,-68.64389;Float;False;Property;_PlaybackSpeed;Playback Speed;2;0;24;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-931.9731,-169.2279;Float;False;Constant;_Float2;Float 2;2;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-1006.839,-377.5963;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;6;-524.7162,-189.2092;Float;True;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;12;141.0493,-400.8377;Float;False;Property;_BloodColor;Blood Color;1;0;0.5955882,0.01751732,0.01751732,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;149.1477,-574.8362;Float;False;Property;_BloodHighlightColor;Blood Highlight Color;1;0;1,0.8602941,0.8602941,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;21;355.8586,-133.91;Float;False;Property;_BloodHighlightSmoothness;Blood Highlight Smoothness;4;0;0.8;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;4;-58.52602,32.10043;Float;True;Property;_BloodSplat1;Blood Splat 1;1;0;Assets/Textures/VFX/Blood/Blood Splat 1.png;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;422.5743,-44.14059;Float;False;Property;_BloodSmoothness;Blood Smoothness;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;22;696.6552,-115.7487;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;14;-57.18466,-190.0903;Float;True;Property;_BloodSplat1_Normal;Blood Splat 1_Normal;2;0;Assets/Textures/VFX/Blood/Blood Splat 1_Normal.png;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;11;554.8237,-382.7014;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;866.4222,-205.9244;Float;False;True;3;Float;ASEMaterialInspector;0;0;Standard;Blood Splatter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;False;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;10;0
WireConnection;6;1;7;0
WireConnection;6;2;9;0
WireConnection;6;3;8;0
WireConnection;4;1;6;0
WireConnection;22;0;21;0
WireConnection;22;1;20;0
WireConnection;22;2;4;0
WireConnection;14;1;6;0
WireConnection;11;0;3;0
WireConnection;11;1;12;0
WireConnection;11;2;4;0
WireConnection;2;0;11;0
WireConnection;2;1;14;0
WireConnection;2;4;22;0
WireConnection;2;10;4;0
ASEEND*/
//CHKSM=2B7E59C662C280B05DA50486973486B9C929A7D7