Shader "Custom/VignetteEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _VignetteColor ("Vignette Color", Color) = (0, 0, 0, 1)
        _VignetteIntensity ("Intensity", Range(0, 1)) = 0.5
        _VignetteRadius ("Radius", Range(0, 1)) = 0.5
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
            float4 _VignetteColor;
            float _VignetteIntensity;
            float _VignetteRadius;
            float4 _MainTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);

                // Calculate distance from the center
                float dist = distance(uv, center);

                // Smooth vignette effect
                float vignette = smoothstep(_VignetteRadius - _VignetteIntensity * 0.5, _VignetteRadius, dist);

                // Sample the texture
                float4 color = tex2D(_MainTex, uv);

                // Blend vignette with the base color
                color.rgb = lerp(color.rgb, _VignetteColor.rgb, vignette);

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
