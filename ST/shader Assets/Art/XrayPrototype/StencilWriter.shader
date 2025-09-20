Shader "Custom/StencilMask"
{
    Properties
    {
        _Scale ("Scale Factor", Range(0.9, 1.1)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Geometry-10" "RenderType"="Opaque" }
        ColorMask 0
        ZWrite Off
        
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            float _Scale;
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                // Scale the vertex position to adjust size
                float4 scaled = v.vertex * _Scale;
                o.pos = UnityObjectToClipPos(scaled);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}