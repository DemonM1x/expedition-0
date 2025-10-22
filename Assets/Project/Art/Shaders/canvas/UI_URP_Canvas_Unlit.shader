Shader "UI/URP Canvas Blended Unlit (SDF)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture (SDF or RGBA)", 2D) = "white" {}
        _Color   ("Tint", Color) = (1,1,1,1)

        // -------- Canvas masking / stencil (RectMask2D, Mask) --------
        _StencilComp       ("Stencil Comparison", Float) = 8
        _Stencil           ("Stencil ID", Float) = 0
        _StencilOp         ("Stencil Operation", Float) = 0
        _StencilWriteMask  ("Stencil Write Mask", Float) = 255
        _StencilReadMask   ("Stencil Read Mask", Float) = 255
        _ColorMask         ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        _ClipRect          ("Clip Rect", Vector) = (-32767,-32767,32767,32767)
        _UIMaskSoftnessX   ("Softness X (px)", Float) = 0
        _UIMaskSoftnessY   ("Softness Y (px)", Float) = 0

        // -------- Blend controls (runtime switchable) --------
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend  ("Src RGB",   Float) = 5   // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend  ("Dst RGB",   Float) = 10  // OneMinusSrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendA ("Src Alpha", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlendA ("Dst Alpha", Float) = 10
        [Enum(UnityEngine.Rendering.BlendOp)]   _BlendOp   ("BlendOp RGB",   Float) = 0 // Add
        [Enum(UnityEngine.Rendering.BlendOp)]   _BlendOpA  ("BlendOp Alpha", Float) = 0

        // -------- SDF controls --------
        _SDF          ("SDF Enabled (0/1)", Float) = 1
        _SDFThreshold ("SDF Threshold", Range(0,1)) = 0.5
        _SDFSoftness  ("SDF Softness",  Range(0,0.2)) = 0.03
        _Emission     ("Emission Multiplier", Range(0,10)) = 1
    }

    SubShader
    {
        Tags{
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        // Dynamic, property-driven blending:
        BlendOp [_BlendOp], [_BlendOpA]
        Blend   [_SrcBlend] [_DstBlend], [_SrcBlendA] [_DstBlendA]
        ColorMask [_ColorMask]

        Pass
        {
            Name "BlendedUI"
            Tags{ "LightMode"="UniversalForward" }

            Stencil{
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask  [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex   vert
            #pragma fragment frag

            // URP + UGUI helpers
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;

            float _SDF, _SDFThreshold, _SDFSoftness, _Emission;
            float4 _ClipRect;
            float _UIMaskSoftnessX, _UIMaskSoftnessY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
                float4 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 wpos = TransformObjectToWorld(v.vertex.xyz);
                o.pos      = TransformWorldToHClip(wpos);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.color    = v.color * _Color;
                o.worldPos = float4(wpos,1);
                return o;
            }

            // SDF decode (assumes distance in R channel)
            float4 SampleSDF(float2 uv, float4 tint)
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                if (_SDF < 0.5)
                {
                    // Plain RGBA
                    return tex * tint;
                }
                else
                {
                    float d     = tex.r; // 0..1
                    float alpha = smoothstep(_SDFThreshold - _SDFSoftness, _SDFThreshold + _SDFSoftness, d);
                    float3 col  = tint.rgb; // color driven by material/tint
                    return float4(col, alpha * tint.a);
                }
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = SampleSDF(i.uv, i.color);

                // RectMask2D clipping (softness in pixels)
                #ifdef UNITY_UI_CLIP_RECT
                    float2 pixelSize = 1.0 / float2(length(mul((float2x2)UNITY_MATRIX_P, float2(1,0))),
                                                    length(mul((float2x2)UNITY_MATRIX_P, float2(0,1))));
                    float2 softness = float2(_UIMaskSoftnessX, _UIMaskSoftnessY) * pixelSize;
                    float mask = UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                    #if !defined(UNITY_UI_CLIP_RECT_SOFTNESS)
                        // simple multiply if your UGUI version doesn't support softness directly
                        col.a *= mask;
                    #else
                        col.a *= UnityGet2DClippingWithSoftness(i.worldPos.xy, _ClipRect, softness);
                    #endif
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(col.a - 0.001);
                #endif

                col.rgb *= _Emission; // bloom driver if HDR
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
