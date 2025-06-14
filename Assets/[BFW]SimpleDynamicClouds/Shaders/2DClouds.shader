﻿Shader "FStarVR/2DClouds" {
    Properties {         
        _CloudColor ("Color", Color) = (1, 1, 1, 0.5)
        _Density ("Density", Range(0.0, 2.0)) = 0.2
        _Speed ("Speed", Range(-1.0, 1.0)) = 0.1
        _ScatterMap0 ("Scatter Map 1", 2D) = "white" {}
        _ScatterMap1 ("Scatter Map 2", 2D) = "white" {}
        _DensityMap ("Density Map", 2D) = "white" {}
        _TextureMap ("Texture Map", 2D) = "white" {}
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            ZTest LEqual
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON STEREO_CUBEMAP_RENDER_ON
            
            #include "UnityCG.cginc"

            fixed4 _CloudColor;
            sampler2D _ScatterMap0;
            sampler2D _ScatterMap1;
            sampler2D _DensityMap;
            sampler2D _TextureMap;
            fixed4 _ScatterMap0_ST;
            fixed4 _ScatterMap1_ST;
            fixed4 _DensityMap_ST;
            fixed4 _TextureMap_ST;
            fixed _Speed;
            fixed _Density;

            struct appdata {
                fixed4 vertex : POSITION;
                fixed2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                fixed4 pos : SV_POSITION;
                fixed4 uv0 : TEXCOORD0;
                fixed4 uv1 : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv0.xy = TRANSFORM_TEX(v.texcoord, _ScatterMap0) * fixed2(2, 2) + _Time.x * _Speed * fixed2(1.5, 1.0);
                o.uv0.zw = TRANSFORM_TEX(v.texcoord, _ScatterMap1) * fixed2(2, 2) + _Time.x * _Speed * fixed2(1.0, 1.2);
                o.uv1.xy = TRANSFORM_TEX(v.texcoord, _DensityMap) * fixed2(10, 10.5) + _Time.x * _Speed * fixed2(0.75, 0.5);
                o.uv1.zw = TRANSFORM_TEX(v.texcoord, _TextureMap) * fixed2(10.6, 10) + _Time.x * _Speed * fixed2(0.5, 0.6);
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                fixed4 col = 0;

                fixed4 n0 = tex2D(_ScatterMap0, UnityStereoTransformScreenSpaceTex(i.uv0.xy));
                fixed4 n1 = tex2D(_ScatterMap1, UnityStereoTransformScreenSpaceTex(i.uv0.zw));
                fixed4 n2 = tex2D(_DensityMap, UnityStereoTransformScreenSpaceTex(i.uv1.xy));
                fixed4 n3 = tex2D(_TextureMap, UnityStereoTransformScreenSpaceTex(i.uv1.zw));

                fixed4 fbm = saturate(n0 + n1 + n2 + n3 - _Density);
                col.a = saturate(fbm.xyz * _CloudColor.a * 2);
                col.rgb = _CloudColor.rgb;
                return col;
            }
            ENDCG
        }
    }
}
