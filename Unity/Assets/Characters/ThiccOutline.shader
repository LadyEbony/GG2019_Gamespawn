Shader "Custom/ThiccOutline" {
  Properties{
    _Color("Color", Color) = (1,1,1,1)
    _MainTex("Albedo (RGB)", 2D) = "white" {}
    _Glossiness("Smoothness", Range(0,1)) = 0.5
    _Metallic("Metallic", Range(0,1)) = 0.0
    _Amount("Highlight Width (0 to disable)", Range(0,1)) = 0.5
    _HighlightColor("Highlight Color", Color) = (1,1,1,1)

    _ZBias("ZBias",Float) = 0
    _ZBiasInterior("ZBias InteriorLines",Float) = 0
    _AmountInterior("Width Interior",Range(0,1)) = 0.001
  }
    SubShader{
      Tags { "RenderType" = "Opaque" }
      LOD 200

      cull off
      Stencil {
        Ref 1
        Comp Always
        Pass Replace
        ZFail Keep
      }
      CGPROGRAM
      // Physically based Standard lighting model, and enable shadows on all light types
      #pragma surface surf Standard fullforwardshadows

      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      sampler2D _MainTex;

      struct Input {
        float2 uv_MainTex;

        float4 color : COLOR;
      };



      half _Glossiness;
      half _Metallic;
      half _ZBias;
      fixed4 _Color;

      void surf(Input IN, inout SurfaceOutputStandard o) {
        // Albedo comes from a texture tinted by color
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        if (c.a < 0.5) clip(-1);
        o.Albedo = c.rgb;
        o.Albedo *= IN.color.xyz;
        // Metallic and smoothness come from slider variables
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Alpha = 1;
      }
      ENDCG
    
      cull off
      Stencil{
          Ref 1
          Comp NotEqual
          Pass Keep
      }
      CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard noshadow vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        half _ZBias;

        struct Input {
          float2 uv_MainTex;
        };

        float _Amount;
        void vert(inout appdata_full v) {
          //v.vertex.xyz += normalize(v.normal.xyz) * -_Amount;
          v.vertex.xyz += v.normal * _Amount;

          float3 fwd = ObjSpaceViewDir(v.vertex);

          v.vertex.xyz += fwd * _ZBias;
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _HighlightColor;

        void surf(Input IN, inout SurfaceOutputStandard o) {
          if (_HighlightColor.a < 1) clip(-1);

          float4 c = tex2D(_MainTex, IN.uv_MainTex);
          if (c.a < 1) clip(-1);

          o.Albedo = 0;
          o.Emission = _HighlightColor;
          //o.Metallic = _Metallic;
          //o.Smoothness = _Glossiness;
          o.Alpha = 0;
        }
        ENDCG

        
        cull front
        Stencil{
          Ref 1
          Comp Equal
          Pass Keep
        }
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard noshadow vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        half _ZBiasInterior;

        struct Input {
          float2 uv_MainTex;
        };

        float _AmountInterior;
        void vert(inout appdata_full v) {
          //v.vertex.xyz += normalize(v.normal.xyz) * -_Amount;
          v.vertex.xyz += v.normal * _AmountInterior;

          float3 fwd = ObjSpaceViewDir(v.vertex);

          v.vertex.xyz += fwd * _ZBiasInterior;
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float4 _HighlightColor;

        void surf(Input IN, inout SurfaceOutputStandard o) {
          if (_HighlightColor.a < 1) clip(-1);

          float4 c = tex2D(_MainTex, IN.uv_MainTex);
          if (c.a < 1) clip(-1);

          o.Albedo = 0;
          o.Emission = _HighlightColor;
          //o.Metallic = _Metallic;
          //o.Smoothness = _Glossiness;
          o.Alpha = 0;
        }
        ENDCG
      }

      
      FallBack "Diffuse"
}