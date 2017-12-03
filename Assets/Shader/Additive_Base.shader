Shader "VFX/Additive_Base" {
    Properties {
        _MainTexture ("MainTexture", 2D) = "white" {}
        _Glow_Intensity ("Glow_Intensity", Float ) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            
            Name "FORWARD"

            Tags { "LightMode"="ForwardBase" }

            Blend One One
            ZWrite Off
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define UNITY_PASS_FORWARDBASE

            #include "UnityCG.cginc"

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0

            uniform sampler2D _MainTexture; 
            float4 _MainTexture_ST;
            float _Glow_Intensity;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                float4 node_5999 = tex2D(_MainTexture,TRANSFORM_TEX(i.uv0, _MainTexture));
                float3 emissive = (node_5999.rgb*i.vertexColor.rgb*i.vertexColor.a*_Glow_Intensity);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
