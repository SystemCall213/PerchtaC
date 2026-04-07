Shader "Hidden/GlyphBrush"
{
    Properties
    {
        _MainTex ("Main Texture (Mask)", 2D) = "white" {}
        _PaintUV ("Paint UV", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 0.1
        _Hardness ("Hardness", Float) = 0.5
        _Strength ("Strength", Float) = 1.0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _PaintUV;
            float _Radius;
            float _Hardness;
            float _Strength;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv);
                float dist = distance(i.uv, _PaintUV.xy);
                
                // Calculate circle mask with hardness
                float edge0 = _Radius * _Hardness;
                float edge1 = _Radius;
                float paint = 1.0 - smoothstep(edge0, edge1, dist);
                
                // Add to existing mask (clamped to 1)
                return saturate(col + paint * _Strength);
            }
            ENDCG
        }
    }
}