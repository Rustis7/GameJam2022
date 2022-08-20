Shader "Custom/Texture Blend" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Blend("Texture Blend", Range(0,1)) = 0.0
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _MainTex2("Albedo 2 (RGB)", 2D) = "white" {}

        _RoughTex1("Rougness 1", 2D) = "white" {}
        _RoughTex2("Rougness 2", 2D) = "white" {}

        _NormalTex1("Normal map 1", 2D) = "white" {}
        _NormalTex2("Normal map 2", 2D) = "white" {}

        //_Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0


            sampler2D _MainTex;
            sampler2D _MainTex2;

            sampler2D _RoughTex1;
            sampler2D _RoughTex2;

            sampler2D _NormalTex1;
            sampler2D _NormalTex2;



            struct Input {
                float2 uv_MainTex;
                float2 uv_MainTex2;

                float2 uv_RoughTex1;
                float2 uv_RoughTex2;

                float2 uv_NormalTex1;
                float2 uv_NormalTex2;
            };

            half _Blend;
            //half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            void surf(Input IN, inout SurfaceOutputStandard o) {
                // Albedo comes from a texture tinted by color
                fixed4 c = lerp(tex2D(_MainTex, IN.uv_MainTex), tex2D(_MainTex2, IN.uv_MainTex2), _Blend) * _Color;
                o.Albedo = c.rgb;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;

                fixed4 r = lerp(tex2D(_RoughTex1, IN.uv_RoughTex1), tex2D(_RoughTex2, IN.uv_RoughTex2), _Blend);
                o.Smoothness = r.rgb;

                fixed4 n = lerp(tex2D(_NormalTex1, IN.uv_NormalTex1), tex2D(_NormalTex2, IN.uv_NormalTex2), _Blend);
                o.Normal = n.rgb;
                //o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}