// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/ColoredPoint" {
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { }

        // The SubShader block containing the Shader code.
        SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag
            // These lines makes it compatible with Entities rendering
            #pragma target 4.5
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "UnityCG.cginc" // redefinition of stuff when using core from URP

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct appdata
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
        
            struct v2f
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float4 positionWorld  : COLOR;
                // use this to access instanced properties in the fragment shader.
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            v2f vert(appdata v)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWorld = float4(TransformObjectToWorld(v.positionOS.xyz), 1);
                //o.positionHCS = v.positionOS; // doesn't work. Why? No clue
                // Convert from object space to worldspace
                //o.positionHCS = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(v.positionOS.xyz, 1.0)));
                //o.positionHCS = mul(unity_ObjectToWorld, v.positionOS);
                //o.positionHCS = UnityObjectToClipPos(v.positionOS);
                // Returning the output.
                return o;
            }

            // The fragment shader definition.
            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                // Defining the color based on its position and returning it.
                float4 customColor = i.positionWorld * 0.5 + 0.5;
                return customColor;
            }
            ENDHLSL
        }
    }
}