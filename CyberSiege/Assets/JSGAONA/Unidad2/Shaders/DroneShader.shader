Shader "Custom/DroneShader"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float4, _DroneData) // x=baseX, y=baseZ, z=offset, w=Y
            UNITY_INSTANCING_BUFFER_END(Props)

            float _GlobalTime;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);

                float4 data = UNITY_ACCESS_INSTANCED_PROP(Props, _DroneData);
                float baseX = data.x;
                float baseZ = data.y;
                float offset = data.z;
                float baseY = data.w;

                float t = _GlobalTime + offset;
                float x = baseX + sin(t * 1.5) * 2.5; // Movimiento en X
                float y = baseY + sin(t * 3.0) * 0.1;  // Flotación suave
                float3 worldPos = float3(x, y, baseZ) + IN.positionOS.xyz;

                OUT.positionHCS = TransformObjectToHClip(worldPos);
                OUT.uv = IN.uv;

                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                half4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return tex * color;
            }

            ENDHLSL
        }
    }
}
