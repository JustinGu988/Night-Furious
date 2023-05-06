// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PowerUpShader" {

    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Starting Offset", Vector) = (0, 0, 0, 0)

        _Dist ("Move Distance", Vector) = (0, 0.5, 0, 0)
        _SpeedA ("Move Speed", Range(0, 4)) = 1

        _SpeedB ("Pulsate Speed", Range(0, 4)) = 2
        _Size ("Size", Range(0, 0.15)) = 0.15
    }

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Tint;
            sampler2D _MainTex;
            float4 _Offset;
            float4 _Dist;
            float _SpeedA, _SpeedB;
            float _Size;
    
            struct vertexIn {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOut{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
            };

            vertexOut vert (vertexIn v) {
                vertexOut o;

                float4 move = v.position + _Offset + cos(_Time.y * _SpeedA) * _Dist;
                float4 pulsate = sin(_Time.y * _SpeedB) * float4(v.normal, 0) * _Size; 
                o.position = UnityObjectToClipPos(move + pulsate);

                o.uv = v.uv;

                return o;
            }

            float4 frag (vertexOut o) : COLOR {
                float3 tex = tex2D(_MainTex, o.uv.xy).rgb * _Tint.rgb;
                
                return float4(tex, 1);
            }        

            ENDCG
        }
    }
}