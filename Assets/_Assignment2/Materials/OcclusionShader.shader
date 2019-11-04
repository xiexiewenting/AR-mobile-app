Shader "Unlit/OcclusionShader"
{
    SubShader
    {
        // Render the Occlusion shader before all
        // opaque geometry to prime the depth buffer.
        Tags { "Queue"="Geometry-1" }
 
        ZWrite On
        ZTest LEqual
        ColorMask 0
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            struct v2f
            {
                float4 position : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
 
            v2f vert (appdata input)
            {
                v2f output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
 
                output.position = UnityObjectToClipPos(input.vertex);
                return output;
            }
 
            fixed4 frag (v2f input) : SV_Target
            {
                return fixed4(0.0, 0.0, 0.0, 0.0);
            }
            ENDCG
        }
    }
}
