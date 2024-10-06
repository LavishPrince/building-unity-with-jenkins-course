Shader "Custom/Unlit Rainbow" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _Speed ("Speed", Range(0, 10)) = 1
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Input structure for vertex shader
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Output structure from vertex shader
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Uniform variables
            float4 _MainColor;
            float _Speed;

            // Vertex shader
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Fragment shader
            fixed4 frag (v2f i) : SV_Target {
                float4 rainbowColor = fixed4(0, 0, 0, 1);
                rainbowColor.r = (sin(_Time.y * _Speed * 0.5) + 1) * 0.5;
                rainbowColor.g = (sin(_Time.y * _Speed * 0.5 + 2) + 1) * 0.5;
                rainbowColor.b = (sin(_Time.y * _Speed * 0.5 + 4) + 1) * 0.5;
                return _MainColor * rainbowColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
