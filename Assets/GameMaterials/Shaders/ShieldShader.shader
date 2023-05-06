// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ShieldShader" {
    Properties {
        _MainTex ("Texture", 2D) = "black" {}
        _ColorMain ("Main Colour", Color) = (0.2, 0.33, 0.4, 1)
        _ColorHighlight ("Highlight Colour", Color) = (0.47, 0.47, 0.47, 1)
        _Speed ("Speed", Range(0,2)) = 0.4
        _Spacing ("Spacing", Range(0, 25)) = 18.8
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader {
        Tags{
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass {
            Cull Off // show the 'front' and 'back'
            ZWrite Off 
            Blend One One //additive

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _ColorMain, _ColorHighlight;
            float _Speed, _Spacing, _Alpha;
            

            struct vertexIn {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertexOut {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;    
            };

            vertexOut vert(vertexIn v) {
                vertexOut o;
                o.position = UnityObjectToClipPos(v.position); // local space to clip space  
                o.uv = v.uv;
                return o;
            }

            float4 frag(vertexOut o) : COLOR  {  
                float t = cos((o.uv.x + _Time.y * _Speed) * _Spacing);
                float4 pattern = lerp(_ColorMain, _ColorHighlight, t); // interpolate colours using t 

                return pattern * (0, 0, 0, _Alpha);
            }

            ENDCG
        }
    }
}
