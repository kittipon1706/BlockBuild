Shader "Custom/PixelSnapShader"
{
    Properties
    {
        _MainTex ("Texture Atlas", 2D) = "white" {}
        _AtlasSize ("Atlas Size", Vector) = (16, 16, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _AtlasSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pixelUV = i.uv * _AtlasSize.xy;

                pixelUV = floor(pixelUV) + 0.5;

                float2 snappedUV = pixelUV / _AtlasSize.xy;

                fixed4 col = tex2D(_MainTex, snappedUV);

                return col;
            }
            ENDCG
        }
    }
}
