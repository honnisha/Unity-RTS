//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//
// Auto-generated shader code, don't hand edit!
//   Compiled with MicroSplat 2.1
//   Unity : 5.6.6f2
//   Platform : OSXEditor
//   RenderLoop : Surface Shader
//////////////////////////////////////////////////////

Shader "MicroSplat/Example" {
   Properties {
      [HideInInspector] _Control0 ("Control0", 2D) = "red" {}
      [HideInInspector] _Control1 ("Control1", 2D) = "black" {}
      

      // Splats
      [NoScaleOffset]_Diffuse ("Diffuse Array", 2DArray) = "white" {}
      [NoScaleOffset]_NormalSAO ("Normal Array", 2DArray) = "bump" {}
      [NoScaleOffset]_PerTexProps("Per Texture Properties", 2D) = "black" {}
      _Contrast("Blend Contrast", Range(0.01, 0.99)) = 0.4
      _UVScale("UV Scales", Vector) = (45, 45, 0, 0)



         _UVMeshRange("UV Mesh Range", Vector) = (1, 1, 0.5, 0.5)












   }

   CGINCLUDE
   ENDCG

   SubShader {
      Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+100" }
      Cull Back
      ZTest LEqual
      CGPROGRAM
      #pragma exclude_renderers d3d9
      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
      #include "Lighting.cginc"
      #include "UnityPBSLighting.cginc"
      #include "UnityStandardBRDF.cginc"
      #pragma surface surf Standard vertex:vert fullforwardshadows addshadow


      #pragma target 3.5



      #define _MAX8TEXTURES 1
      #define _MICROSPLAT 1

      #define _MSRENDERLOOP_SURFACESHADER 1
      #if _USEGRADMIP
         #define MIPFORMAT float4
      #else
         #define MIPFORMAT float
      #endif

      #if _MESHSUBARRAY
         half4 _MeshSubArrayIndexes;
      #endif

      // splat
      UNITY_DECLARE_TEX2DARRAY(_Diffuse);
      float4 _Diffuse_TexelSize;
      UNITY_DECLARE_TEX2DARRAY(_NormalSAO);
      float4 _NormalSAO_TexelSize;

      #if _PACKINGHQ
      UNITY_DECLARE_TEX2DARRAY(_SmoothAO);
      #endif

      #if _USEEMISSIVEMETAL
         UNITY_DECLARE_TEX2DARRAY(_EmissiveMetal);
         float4 _EmissiveMetal_TexelSize;
      #endif

      half _Contrast;
      UNITY_DECLARE_TEX2D(_Control0);
      #if !_MAX4TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control1);
      #endif
      #if !_MAX4TEXTURES && !_MAX8TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control2);
      #endif
      #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control3);
      #endif
      #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control4);
      #endif
      #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control5);
      #endif
      #if _MAX28TEXTURES || _MAX32TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control6);
      #endif
      #if _MAX32TEXTURES
      UNITY_DECLARE_TEX2D_NOSAMPLER(_Control7);
      #endif

      sampler2D_float _PerTexProps;
      float2 uv_Control0;

      float4 _UVScale; // scale and offset

      struct TriplanarConfig
      {
         float3x3 uv0;
         float3x3 uv1;
         float3x3 uv2;
         float3x3 uv3;
         half3 pN;
         half3 pN0;
         half3 pN1;
         half3 pN2;
         half3 pN3;
      };


      struct Config
      {
         float2 uv;
         float3 uv0;
         float3 uv1;
         float3 uv2;
         float3 uv3;

         half4 cluster0;
         half4 cluster1;
         half4 cluster2;
         half4 cluster3;

      };


      struct MicroSplatLayer
      {
         half3 Albedo;
         half3 Normal;
         half Smoothness;
         half Occlusion;
         half Metallic;
         half Height;
         half3 Emission;
         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
         half Alpha;
         #endif
      };


      struct appdata 
      {
         float4 vertex : POSITION;
         float4 tangent : TANGENT;
         float3 normal : NORMAL;
         float2 texcoord : TEXCOORD0;
         float2 texcoord1 : TEXCOORD1;
         float2 texcoord2 : TEXCOORD2;
         #if _TERRAINBLENDING || _VSSHADOWMAP || _WINDSHADOWS || _SNOWSHADOWS
         half4 color : COLOR;
         #endif
         UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct Input 
      {
         float2 uv_Control0;
         #if _MICROMESH && _MESHUV2
         float2 uv2_Diffuse;
         #endif
         float3 viewDir;
         float3 worldPos;
         float3 worldNormal;
         #if _TERRAINBLENDING || _VSSHADOWMAP || _WINDSHADOWS || _SNOWSHADOWS
         fixed4 color : COLOR;
         #endif
         #if _MSRENDERLOOP_SURFACESHADER
         INTERNAL_DATA
         #else
         float3x3 TBN;
         #endif
      };

      // raw, unblended samples from arrays
      struct RawSamples
      {
         half4 albedo0;
         half4 albedo1;
         half4 albedo2;
         half4 albedo3;
         half4 normSAO0;
         half4 normSAO1;
         half4 normSAO2;
         half4 normSAO3;
         #if _USEEMISSIVEMETAL
            half4 emisMetal0;
            half4 emisMetal1;
            half4 emisMetal2;
            half4 emisMetal3;
         #endif
      };

      void InitRawSamples(inout RawSamples s)
      {
         s.normSAO0 = half4(0,0,0,1);
         s.normSAO1 = half4(0,0,0,1);
         s.normSAO2 = half4(0,0,0,1);
         s.normSAO3 = half4(0,0,0,1);
      }




      #if _MAX2LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y; }
      #elif _MAX3LAYER
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z; }
      #else
         inline half BlendWeights(half s1, half s2, half s3, half s4, half4 w)      { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half2 BlendWeights(half2 s1, half2 s2, half2 s3, half2 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half3 BlendWeights(half3 s1, half3 s2, half3 s3, half3 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
         inline half4 BlendWeights(half4 s1, half4 s2, half4 s3, half4 s4, half4 w) { return s1 * w.x + s2 * w.y + s3 * w.z + s4 * w.w; }
      #endif

      #if _MAX3LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = tex2Dlod(_PerTexProps, float4(config.uv0.z/32, pixel/16, 0, 0)); \
            varName##1 = tex2Dlod(_PerTexProps, float4(config.uv1.z/32, pixel/16, 0, 0)); \
            varName##2 = tex2Dlod(_PerTexProps, float4(config.uv2.z/32, pixel/16, 0, 0)); \

      #elif _MAX2LAYER
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = defVal; \
            half4 varName##1 = defVal; \
            half4 varName##2 = defVal; \
            half4 varName##3 = defVal; \
            varName##0 = tex2Dlod(_PerTexProps, float4(config.uv0.z/32, pixel/16, 0, 0)); \
            varName##1 = tex2Dlod(_PerTexProps, float4(config.uv1.z/32, pixel/16, 0, 0)); \

      #else
         #define SAMPLE_PER_TEX(varName, pixel, config, defVal) \
            half4 varName##0 = tex2Dlod(_PerTexProps, float4(config.uv0.z/32, pixel/16, 0, 0)); \
            half4 varName##1 = tex2Dlod(_PerTexProps, float4(config.uv1.z/32, pixel/16, 0, 0)); \
            half4 varName##2 = tex2Dlod(_PerTexProps, float4(config.uv2.z/32, pixel/16, 0, 0)); \
            half4 varName##3 = tex2Dlod(_PerTexProps, float4(config.uv3.z/32, pixel/16, 0, 0)); \

      #endif

      // 2 component normal blend?
      half2 BlendNormal2(half2 base, half2 blend) { return normalize(float3(base.xy + blend.xy, 1)).xy; }
      half3 BlendOverlay(half3 base, half3 blend) { return (base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend))); }
      half3 BlendMult2X(half3  base, half3 blend) { return (base * (blend * 2)); }


      half4 ComputeWeights(half4 iWeights, half h0, half h1, half h2, half h3, half contrast)
      {
          #if _DISABLEHEIGHTBLENDING
             return iWeights;
          #else
             // compute weight with height map
             //half4 weights = half4(iWeights.x * h0, iWeights.y * h1, iWeights.z * h2, iWeights.w * h3);
             half4 weights = half4(iWeights.x * max(h0,0.001), iWeights.y * max(h1,0.001), iWeights.z * max(h2,0.001), iWeights.w * max(h3,0.001));
             
             // Contrast weights
             half maxWeight = max(max(weights.x, max(weights.y, weights.z)), weights.w);
             half transition = max(contrast * maxWeight, 0.0001);
             half threshold = maxWeight - transition;
             half scale = 1.0 / transition;
             weights = saturate((weights - threshold) * scale);
             // Normalize weights.
             half weightScale = 1.0f / (weights.x + weights.y + weights.z + weights.w);
             weights *= weightScale;
             return weights;
          #endif
      }

      half HeightBlend(half h1, half h2, half slope, half contrast)
      {
         #if _DISABLEHEIGHTBLENDING
            return slope;
         #else
            h2 = 1 - h2;
            half tween = saturate((slope - min(h1, h2)) / max(abs(h1 - h2), 0.001)); 
            half blend = saturate( ( tween - (1-contrast) ) / max(contrast, 0.001));
            return blend;
         #endif
      }

      #if _MAX4TEXTURES
         #define TEXCOUNT 4
      #elif _MAX8TEXTURES
         #define TEXCOUNT 8
      #elif _MAX12TEXTURES
         #define TEXCOUNT 12
      #elif _MAX20TEXTURES
         #define TEXCOUNT 20
      #elif _MAX24TEXTURES
         #define TEXCOUNT 24
      #elif _MAX28TEXTURES
         #define TEXCOUNT 28
      #elif _MAX32TEXTURES
         #define TEXCOUNT 32
      #else
         #define TEXCOUNT 16
      #endif

      void Setup(out half4 weights, float2 uv, out Config config, fixed4 w0, fixed4 w1, fixed4 w2, fixed4 w3, fixed4 w4, fixed4 w5, fixed4 w6, fixed4 w7, float3 worldPos)
      {
         config = (Config)0;
         half4 indexes = 0;

         config.uv = uv;

         #if _WORLDUV
         uv = worldPos.xz;
         #endif

         float2 scaledUV = uv * _UVScale.xy + _UVScale.zw;

         // if only 4 textures, and blending 4 textures, skip this whole thing..
         // this saves about 25% of the ALU of the base shader on low end
         float4 defaultIndexes = float4(0,1,2,3);
         #if _MESHSUBARRAY
            defaultIndexes = _MeshSubArrayIndexes;
         #endif

         #if _MESHSUBARRAY || (_MAX4TEXTURES && !_MAX3LAYER && !_MAX2LAYER)
            weights = w0;
            config.uv0 = float3(scaledUV, defaultIndexes.x);
            config.uv1 = float3(scaledUV, defaultIndexes.y);
            config.uv2 = float3(scaledUV, defaultIndexes.z);
            config.uv3 = float3(scaledUV, defaultIndexes.w);
            return;
         #endif

         fixed splats[TEXCOUNT];

         splats[0] = w0.x;
         splats[1] = w0.y;
         splats[2] = w0.z;
         splats[3] = w0.w;
         #if !_MAX4TEXTURES
            splats[4] = w1.x;
            splats[5] = w1.y;
            splats[6] = w1.z;
            splats[7] = w1.w;
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES
            splats[8] = w2.x;
            splats[9] = w2.y;
            splats[10] = w2.z;
            splats[11] = w2.w;
         #endif
         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
            splats[12] = w3.x;
            splats[13] = w3.y;
            splats[14] = w3.z;
            splats[15] = w3.w;
         #endif
         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            splats[16] = w4.x;
            splats[17] = w4.y;
            splats[18] = w4.z;
            splats[19] = w4.w;
         #endif
         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
            splats[20] = w5.x;
            splats[21] = w5.y;
            splats[22] = w5.z;
            splats[23] = w5.w;
         #endif
         #if _MAX28TEXTURES || _MAX32TEXTURES
            splats[24] = w6.x;
            splats[25] = w6.y;
            splats[26] = w6.z;
            splats[27] = w6.w;
         #endif
         #if _MAX32TEXTURES
            splats[28] = w7.x;
            splats[29] = w7.y;
            splats[30] = w7.z;
            splats[31] = w7.w;
         #endif



         weights[0] = 0;
         weights[1] = 0;
         weights[2] = 0;
         weights[3] = 0;
         indexes[0] = 0;
         indexes[1] = 0;
         indexes[2] = 0;
         indexes[3] = 0;

         int i = 0;
         for (i = 0; i < TEXCOUNT; ++i)
         {
            fixed w = splats[i];
            if (w >= weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = i;
            }
            else if (w >= weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = i;
            }
            else if (w >= weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = i;
            }
            else if (w >= weights[3])
            {
               weights[3] = w;
               indexes[3] = i;
            }
         }

         // clamp and renormalize
         #if _MAX2LAYER
         weights.zw = 0;
         weights.xy *= (1.0 / (weights.x + weights.y));
         #endif
         #if _MAX3LAYER
         weights.w = 0;
         weights.xyz *= (1.0 / (weights.x + weights.y + weights.z));
         #endif

         config.uv0 = float3(scaledUV, indexes.x);
         config.uv1 = float3(scaledUV, indexes.y);
         config.uv2 = float3(scaledUV, indexes.z);
         config.uv3 = float3(scaledUV, indexes.w);
      }

      float ComputeMipLevel(float2 uv, float2 textureSize)
      {
         uv *= textureSize;
         float2  dx_vtc        = ddx(uv);
         float2  dy_vtc        = ddy(uv);
         float delta_max_sqr   = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
         return 0.5 * log2(delta_max_sqr);
      }

      inline fixed2 UnpackNormal2(fixed4 packednormal)
      {
         #if defined(UNITY_NO_DXT5nm)
          return packednormal.xy * 2 - 1;
         #else
          return packednormal.wy * 2 - 1;
         #endif
      }

      half3 TriplanarHBlend(half h0, half h1, half h2, half3 pN, half contrast)
      {
         half3 blend = pN / dot(pN, half3(1,1,1));
         float3 heights = float3(h0, h1, h2) + (blend * 3.0);
         half height_start = max(max(heights.x, heights.y), heights.z) - contrast;
         half3 h = max(heights - height_start.xxx, half3(0,0,0));
         blend = h / dot(h, half3(1,1,1));
         return blend;
      }

      // man I wish unity would wrap everything instead of only what they use. Just seems like a landmine for
      // people like myself.. especially as they keep changing things around and I have to figure out all the new defines
      // and handle changes across Unity versions, which would be automatically handled if they just wrapped these themselves without
      // as much complexity..

      #if (UNITY_VERSION >= 201810 && (defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (SHADER_TARGET_SURFACE_ANALYSIS && !SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))) || (UNITY_VERSION < 201810 && (defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL))) 
           #define MICROSPLAT_SAMPLE_TEX2D_LOD(tex,coord, lod) tex.SampleLevel (sampler##tex,coord, lod)
           #define MICROSPLAT_SAMPLE_TEX2D_SAMPLER_LOD(tex,samplertex,coord, lod) tex.SampleLevel (sampler##samplertex,coord, lod)
        #else
           #define MICROSPLAT_SAMPLE_TEX2D_LOD(tex,coord,lod) tex2D (tex,coord,0,lod)
           #define MICROSPLAT_SAMPLE_TEX2D_SAMPLER_LOD(tex,samplertex,coord,lod) tex2D (tex,coord,0,lod)
        #endif
     


        #if (UNITY_VERSION >= 201810 && (defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (SHADER_TARGET_SURFACE_ANALYSIS && !SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))) || (UNITY_VERSION < 201810 && (defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL))) 
           #define MICROSPLAT_SAMPLE_TEX2D_GRAD(tex,coord,dx,dy) tex.SampleGrad (sampler##tex,coord,dx,dy)
        #elif defined(SHADER_API_D3D9)
           #define MICROSPLAT_SAMPLE_TEX2D_GRAD(tex,coord,dx,dy) half4(0,1,0,0) 
        #elif defined(UNITY_COMPILER_HLSL2GLSL) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
           #define MICROSPLAT_SAMPLE_TEX2D_GRAD(tex,coord,dx,dy) texCUBEgrad (tex,coord,float3(dx.x,dx.y,0),float3(dy.x,dy.y,0))
        #elif defined(SHADER_API_GLES)
           #define MICROSPLAT_SAMPLE_TEX2D_GRAD(tex,coord,dx,dy) half4(1,1,0,0)
        #elif defined(SHADER_API_D3D11_9X)
           #define MICROSPLAT_SAMPLE_TEX2D_GRAD(tex,coord,dx,dy) half4(0,1,1,0) 
        #else
           #define MICROSPLAT_SAMPLE_TEX2D_GRAD(tex,coord,dx,dy) half4(0,0,1,0) 
        #endif
      


              #if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL)
            #define MY_UNITY_SAMPLE_TEX2DARRAY_GRAD(tex,coord,dx,dy) tex.SampleGrad (sampler##tex,coord,dx,dy)
        #else

            #if defined(UNITY_COMPILER_HLSL2GLSL) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
                #define MY_UNITY_SAMPLE_TEX2DARRAY_GRAD(tex,coord,dx,dy) texCUBEgrad (tex,coord,float3(dx.x,dx.y,0),float3(dy.x,dy.y,0))
            #endif
        #endif 



      #if _USELODMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) UNITY_SAMPLE_TEX2DARRAY_LOD(tex, u, l.x)
      #elif _USEGRADMIP
         #define MICROSPLAT_SAMPLE(tex, u, l) MICROSPLAT_SAMPLE_TEX2D_GRAD(tex, u, l.xy, l.zw)
      #else
         #define MICROSPLAT_SAMPLE(tex, u, l) UNITY_SAMPLE_TEX2DARRAY(tex, u)
      #endif


      #define MICROSPLAT_SAMPLE_DIFFUSE(u, cl, l) MICROSPLAT_SAMPLE(_Diffuse, u, l)
      #define MICROSPLAT_SAMPLE_EMIS(u, cl, l) MICROSPLAT_SAMPLE(_EmissiveMetal, u, l)
      #define MICROSPLAT_SAMPLE_DIFFUSE_LOD(u, cl, l) UNITY_SAMPLE_TEX2DARRAY_LOD(_Diffuse, u, l)

      #if _PACKINGHQ
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) half4(MICROSPLAT_SAMPLE(_NormalSAO, u, l).ga, MICROSPLAT_SAMPLE(_SmoothAO, u, l).ga).brag
      #else
         #define MICROSPLAT_SAMPLE_NORMAL(u, cl, l) MICROSPLAT_SAMPLE(_NormalSAO, u, l)
      #endif

#if _VSSHADOWMAP
      float4 gVSSunDirection;
#endif



#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X)
    sampler2D _TerrainHeightmapTexture;
    sampler2D _TerrainNormalmapTexture;
    float4    _TerrainHeightmapRecipSize;   // float4(1.0f/width, 1.0f/height, 1.0f/(width-1), 1.0f/(height-1))
    float4    _TerrainHeightmapScale;       // float4(hmScale.x, hmScale.y / (float)(kMaxHeight), hmScale.z, 0.0f)
#endif

#if UNITY_VERSION >= 201830
UNITY_INSTANCING_BUFFER_START(Terrain)
    UNITY_DEFINE_INSTANCED_PROP(float4, _TerrainPatchInstanceData) // float4(xBase, yBase, skipScale, ~)
UNITY_INSTANCING_BUFFER_END(Terrain)
#endif


      // surface shaders + tessellation, do not pass go, or
      // collect $500 - sucks it up and realize you can't use
      // an Input struct, so you have to hack UV coordinates
      // and live with only the magic keywords..
      void vert (inout appdata i) 
      {
         #if UNITY_VERSION >= 201830
            #if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X)
               float2 patchVertex = i.vertex.xy;
               float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);

               float4 uvscale = instanceData.z * _TerrainHeightmapRecipSize;
               float4 uvoffset = instanceData.xyxy * uvscale;
               uvoffset.xy += 0.5f * _TerrainHeightmapRecipSize.xy;
               float2 sampleCoords = (patchVertex.xy * uvscale.xy + uvoffset.xy);

               float hm = UnpackHeightmap(tex2Dlod(_TerrainHeightmapTexture, float4(sampleCoords, 0, 0)));
               i.vertex.xz = (patchVertex.xy + instanceData.xy) * _TerrainHeightmapScale.xz * instanceData.z;  //(x + xBase) * hmScale.x * skipScale;
               i.vertex.y = hm * _TerrainHeightmapScale.y;
               i.vertex.w = 1.0f;

               i.texcoord.xy = (patchVertex.xy * uvscale.zw + uvoffset.zw);
               i.texcoord2 = i.texcoord1 = i.texcoord;

               #ifdef TERRAIN_INSTANCED_PERPIXEL_NORMAL
                 i.normal = float3(0, 1, 0); // TODO: reconstruct the tangent space in the pixel shader. Seems to be hard with surface shader especially when other attributes are packed together with tSpace.
                 iata.tc.zw = sampleCoords;
               #else
                 float3 nor = tex2Dlod(_TerrainNormalmapTexture, float4(sampleCoords, 0, 0)).xyz;
                 i.normal = 2.0f * nor - 1.0f;
               #endif
            #endif
         #endif


         #if _TRIPLANAR
         half3 normal  = UnityObjectToWorldNormal(i.normal);
         float3 worldPos = mul(unity_ObjectToWorld, i.vertex).xyz;
         float3 sgn = sign(normal);
         half3 tnorm = max(pow(abs(normal), 10), 0.0001);
         tnorm /= dot(tnorm, half3(1,1,1));

         i.tangent.xyz = cross(i.normal, mul(unity_WorldToObject, fixed4(0, sgn.x, 0, 0)).xyz * tnorm.x)
                 + cross(i.normal, mul(unity_WorldToObject, fixed4(0, 0, sgn.y, 0)).xyz * tnorm.y)
                 + cross(i.normal, mul(unity_WorldToObject, fixed4(0, sgn.z, 0, 0)).xyz * tnorm.z);
         i.tangent.w = -1;
         #elif !_MICROMESH
         float4 tangent;
         tangent.xyz = cross(UnityObjectToWorldNormal( i.normal ), float3(0,0,1));
         tangent.w = -1;
         i.tangent = tangent;
         #endif


         #if _VSSHADOWMAP || _WINDSHADOWS || _SNOWSHADOWS
         float3 N = mul((float3x3)unity_ObjectToWorld, i.normal);
         float3 T = mul((float3x3)unity_ObjectToWorld, i.tangent.xyz);
         float3 B = cross(N,T) * i.tangent.w;
         float3x3 worldToTangent = float3x3(T,B,N);
            #if _VSSHADOWMAP
            i.color.rgb = mul( worldToTangent, gVSSunDirection.xyz ).xyz;
            #else
            i.color.rgb = mul( worldToTangent, normalize(_WorldSpaceLightPos0.xyz) ).xyz;
            #endif

         #endif

      }




   
      half4 _UVMeshRange;



      void SampleAlbedo(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel)
      {

         #if _TRIPLANAR

            half4 contrasts = _Contrast.xxxx;
            #if _PERTEXTRIPLANARCONTRAST
               SAMPLE_PER_TEX(ptc, 5.5, config, half4(1,0.5,0,0));
               contrasts = half4(ptc0.y, ptc1.y, ptc2.y, ptc3.y);
            #endif

            {
               half4 a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[0], config.cluster0, mipLevel);
               half4 a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[1], config.cluster0, mipLevel);
               half4 a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv0[2], config.cluster0, mipLevel);
               half3 bf = tc.pN0;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN0, contrasts.x);
               tc.pN0 = bf;
               #endif

               s.albedo0 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            {
               half4 a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[0], config.cluster1, mipLevel);
               half4 a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[1], config.cluster1, mipLevel);
               half4 a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv1[2], config.cluster1, mipLevel);
               half3 bf = tc.pN1;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN1, contrasts.x);
               tc.pN1 = bf;
               #endif
               s.albedo1 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #if !_MAX2LAYER
            {
               half4 a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[0], config.cluster2, mipLevel);
               half4 a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[1], config.cluster2, mipLevel);
               half4 a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv2[2], config.cluster2, mipLevel);
               half3 bf = tc.pN2;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN2, contrasts.x);
               tc.pN2 = bf;
               #endif
               s.albedo2 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            {
               half4 a0 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[0], config.cluster3, mipLevel);
               half4 a1 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[1], config.cluster3, mipLevel);
               half4 a2 = MICROSPLAT_SAMPLE_DIFFUSE(tc.uv3[2], config.cluster3, mipLevel);
               half3 bf = tc.pN3;
               #if _TRIPLANARHEIGHTBLEND
               bf = TriplanarHBlend(a0.a, a1.a, a2.a, tc.pN3, contrasts.x);
               tc.pN3 = bf;
               #endif
               s.albedo3 = a0 * bf.x + a1 * bf.y + a2 * bf.z;
            }
            #endif

         #else
            s.albedo0 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv0, config.cluster0, mipLevel);
            s.albedo1 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv1, config.cluster1, mipLevel);
            #if !_MAX2LAYER
            s.albedo2 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv2, config.cluster2, mipLevel); 
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            s.albedo3 = MICROSPLAT_SAMPLE_DIFFUSE(config.uv3, config.cluster3, mipLevel);
            #endif
         #endif

         #if _PERTEXHEIGHTOFFSET || _PERTEXHEIGHTCONTRAST
            SAMPLE_PER_TEX(ptHeight, 10.5, config, 1);

            #if _PERTEXHEIGHTOFFSET
               s.albedo0.a = saturate(s.albedo0.a + ptHeight0.b - 1);
               s.albedo1.a = saturate(s.albedo1.a + ptHeight1.b - 1);
               s.albedo2.a = saturate(s.albedo2.a + ptHeight2.b - 1);
               s.albedo3.a = saturate(s.albedo3.a + ptHeight3.b - 1);
            #endif
            #if _PERTEXHEIGHTCONTRAST
               s.albedo0.a = saturate(pow(s.albedo0.a + 0.5, ptHeight0.a) - 0.5);
               s.albedo1.a = saturate(pow(s.albedo1.a + 0.5, ptHeight1.a) - 0.5);
               s.albedo2.a = saturate(pow(s.albedo2.a + 0.5, ptHeight2.a) - 0.5);
               s.albedo3.a = saturate(pow(s.albedo3.a + 0.5, ptHeight3.a) - 0.5);
            #endif
         #endif
      }

      void SampleNormal(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel)
      {
         #if _TRIPLANAR

            {
               half4 a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[0], config.cluster0, mipLevel).garb;
               half4 a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[1], config.cluster0, mipLevel).garb;
               half4 a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv0[2], config.cluster0, mipLevel).garb;
               s.normSAO0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               s.normSAO0.xy = s.normSAO0.xy * 2 - 1;
            }
            {
               half4 a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[0], config.cluster1, mipLevel).garb;
               half4 a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[1], config.cluster1, mipLevel).garb;
               half4 a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv1[2], config.cluster1, mipLevel).garb;
               s.normSAO1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               s.normSAO1.xy = s.normSAO1.xy * 2 - 1;
            }
            #if !_MAX2LAYER
            {
               half4 a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[0], config.cluster2, mipLevel).garb;
               half4 a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[1], config.cluster2, mipLevel).garb;
               half4 a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv2[2], config.cluster2, mipLevel).garb;
               s.normSAO2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
            }
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            {
               half4 a0 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[0], config.cluster3, mipLevel).garb;
               half4 a1 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[1], config.cluster3, mipLevel).garb;
               half4 a2 = MICROSPLAT_SAMPLE_NORMAL(tc.uv3[2], config.cluster3, mipLevel).garb;
               s.normSAO3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
            }
            #endif

         #else
            s.normSAO0 = MICROSPLAT_SAMPLE_NORMAL(config.uv0, config.cluster0, mipLevel).garb;
            s.normSAO1 = MICROSPLAT_SAMPLE_NORMAL(config.uv1, config.cluster1, mipLevel).garb;
            s.normSAO0.xy = s.normSAO0.xy * 2 - 1;
            s.normSAO1.xy = s.normSAO1.xy * 2 - 1;
            #if !_MAX2LAYER
            s.normSAO2 = MICROSPLAT_SAMPLE_NORMAL(config.uv2, config.cluster2, mipLevel).garb;
            s.normSAO2.xy = s.normSAO2.xy * 2 - 1;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
            s.normSAO3 = MICROSPLAT_SAMPLE_NORMAL(config.uv3, config.cluster3, mipLevel).garb;
            s.normSAO3.xy = s.normSAO3.xy * 2 - 1;
            #endif
         #endif
      }

      void SampleEmis(Config config, TriplanarConfig tc, inout RawSamples s, MIPFORMAT mipLevel)
      {
         #if _USEEMISSIVEMETAL
            #if _TRIPLANAR

               {
                  half4 a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[0], config.cluster0, mipLevel);
                  half4 a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[1], config.cluster0, mipLevel);
                  half4 a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv0[2], config.cluster0, mipLevel);
                  s.emisMetal0 = a0 * tc.pN0.x + a1 * tc.pN0.y + a2 * tc.pN0.z;
               }
               {
                  half4 a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[0], config.cluster1, mipLevel);
                  half4 a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[1], config.cluster1, mipLevel);
                  half4 a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv1[2], config.cluster1, mipLevel);
                  s.emisMetal1 = a0 * tc.pN1.x + a1 * tc.pN1.y + a2 * tc.pN1.z;
               }
               #if !_MAX2LAYER
               {
                  half4 a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[0], config.cluster2, mipLevel);
                  half4 a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[1], config.cluster2, mipLevel);
                  half4 a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv2[2], config.cluster2, mipLevel);
                  s.emisMetal2 = a0 * tc.pN2.x + a1 * tc.pN2.y + a2 * tc.pN2.z;
               }
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               {
                  half4 a0 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[0], config.cluster3, mipLevel);
                  half4 a1 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[1], config.cluster3, mipLevel);
                  half4 a2 = MICROSPLAT_SAMPLE_EMIS(tc.uv3[2], config.cluster3, mipLevel);
                  s.emisMetal3 = a0 * tc.pN3.x + a1 * tc.pN3.y + a2 * tc.pN3.z;
               }
               #endif

            #else
               s.emisMetal0 = MICROSPLAT_SAMPLE_EMIS(config.uv0, config.cluster0, mipLevel);
               s.emisMetal1 = MICROSPLAT_SAMPLE_EMIS(config.uv1, config.cluster1, mipLevel);
               #if !_MAX2LAYER
               s.emisMetal2 = MICROSPLAT_SAMPLE_EMIS(config.uv2, config.cluster2, mipLevel);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
               s.emisMetal3 = MICROSPLAT_SAMPLE_EMIS(config.uv3, config.cluster3, mipLevel);
               #endif
            #endif
         #endif
      }



      MicroSplatLayer Sample(Input i, half4 weights, inout Config config, float camDist, float3 worldNormalVertex)
      {
         half4 fxLevels = half4(0,0,0,0);
         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         half burnLevel = 0;
         half wetLevel = 0;
         fxLevels = SampleFXLevels(config.uv, wetLevel, burnLevel);
         #endif

         TriplanarConfig tc = (TriplanarConfig)0;
         UNITY_INITIALIZE_OUTPUT(TriplanarConfig,tc);
         #if _TRIPLANAR
         PrepTriplanar(worldNormalVertex, i.worldPos, config, tc, weights);
         #endif

         MIPFORMAT albedoLOD = 0;
         MIPFORMAT normalLOD = 0;
         MIPFORMAT emisLOD = 0;

         #if _USELODMIP
            #if _TRIPLANAR
               float2 cuv0 = tc.uv0[0].xy;
            #else
               float2 cuv0 = config.uv0.xy;
            #endif
            albedoLOD = ComputeMipLevel(cuv0, _Diffuse_TexelSize.zw);
            normalLOD = ComputeMipLevel(cuv0, _NormalSAO_TexelSize.zw);
            #if _USEEMISSIVEMETAL
            emisLOD   = ComputeMipLevel(cuv0, _EmissiveMetal_TexelSize.zw);
            #endif
         #endif

         #if _USEGRADMIP
            float2 dxdyuv = config.uv.xy * _UVScale.xy + _UVScale.zw;
            albedoLOD = float4(ddx(dxdyuv), ddy(dxdyuv));
            normalLOD = albedoLOD;
            #if _USEEMISSIVEMETAL
               emisLOD = albedoLOD;
            #endif
         #endif

         #if _DISTANCERESAMPLE
            #if _TRIPLANAR
               float2 tuv0 = tc.uv0[0].xy;
            #else
               float2 tuv0 = config.uv0.xy;
            #endif
            float resampleLOD = ComputeMipLevel(tuv0 * _ResampleDistanceParams.xx, _Diffuse_TexelSize.zw);
         #endif


         // uvScale before anything
         #if _PERTEXUVSCALEOFFSET && !_TRIPLANAR
            SAMPLE_PER_TEX(ptUVScale, 0.5, config, half4(1,1,0,0));
            config.uv0.xy = config.uv0.xy * ptUVScale0.rg + ptUVScale0.ba;
            config.uv1.xy = config.uv1.xy * ptUVScale1.rg + ptUVScale1.ba;
            #if !_MAX2LAYER
               config.uv2.xy = config.uv2.xy * ptUVScale2.rg + ptUVScale2.ba;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               config.uv3.xy = config.uv3.xy * ptUVScale3.rg + ptUVScale3.ba;
            #endif

            // fix for pertex uv scale using gradient sampler and weight blended derivatives
            #if _USEGRADMIP
               albedoLOD = albedoLOD * ptUVScale0.rgrg * weights.x + 
                           albedoLOD * ptUVScale1.rgrg * weights.y + 
                           albedoLOD * ptUVScale2.rgrg * weights.z + 
                           albedoLOD * ptUVScale3.rgrg * weights.w;
               normalLOD = albedoLOD;
               #if _USEEMISSIVEMETAL
                  emisLOD = albedoLOD;
               #endif
            #endif
         #endif



         MicroSplatLayer o = (MicroSplatLayer)0;
         UNITY_INITIALIZE_OUTPUT(MicroSplatLayer,o);

         RawSamples samples = (RawSamples)0;
         InitRawSamples(samples);

         SampleAlbedo(config, tc, samples, albedoLOD);

         #if _STREAMS || _PARALLAX
         half earlyHeight = BlendWeights(samples.albedo0.w, samples.albedo1.w, samples.albedo2.w, samples.albedo3.w, weights);
         #endif

         half3 waterNormalFoam = half3(0, 0, 0);
         #if _STREAMS
         waterNormalFoam = GetWaterNormal(config.uv, worldNormalVertex);
         DoStreamRefract(config, tc, waterNormalFoam, fxLevels.b, earlyHeight);
         #endif

         #if _PARALLAX
            DoParallax(i, earlyHeight, config, tc, samples, weights, camDist);
         #endif


         // Blend results
         #if _PERTEXINTERPCONTRAST
            SAMPLE_PER_TEX(ptContrasts, 1.5, config, 0.5);
            half4 contrast = 0.5;
            contrast.x = ptContrasts0.a;
            contrast.y = ptContrasts1.a;
            #if !_MAX2LAYER
               contrast.z = ptContrasts2.a;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               contrast.w = ptContrasts3.a;
            #endif
            contrast = clamp(contrast + _Contrast, 0.0001, 1.0); 
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, contrast);
         #else
            half4 heightWeights = ComputeWeights(weights, samples.albedo0.a, samples.albedo1.a, samples.albedo2.a, samples.albedo3.a, _Contrast);
         #endif


         #if _PARALLAX || _STREAMS
            SampleAlbedo(config, tc, samples, albedoLOD);
         #endif

         SampleNormal(config, tc, samples, normalLOD);

         #if _USEEMISSIVEMETAL
            SampleEmis(config, tc, samples, emisLOD);
         #endif

         #if _DISTANCERESAMPLE
         DistanceResample(samples, config, tc, camDist, i.viewDir, fxLevels, resampleLOD);
         #endif

         // PerTexture sampling goes here, passing the samples structure

         #if _PERTEXTINT
            SAMPLE_PER_TEX(ptTints, 1.5, config, half4(1, 1, 1, 1));
            samples.albedo0.rgb *= ptTints0.rgb;
            samples.albedo1.rgb *= ptTints1.rgb;
            #if !_MAX2LAYER
               samples.albedo2.rgb *= ptTints2.rgb;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.albedo3.rgb *= ptTints3.rgb;
            #endif
         #endif

         half porosity = 0.4;
         float streamFoam = 1.0f;

         #if _WETNESS || _PUDDLES || _STREAMS
         porosity = _GlobalPorosity;
         #endif


         #if _PERTEXBRIGHTNESS || _PERTEXCONTRAST || _PERTEXPOROSITY || _PERTEXFOAM
            SAMPLE_PER_TEX(ptBC, 3.5, config, half4(1, 1, 1, 1));
            #if _PERTEXCONTRAST
               samples.albedo0.rgb = saturate(((samples.albedo0.rgb - 0.5) * ptBC0.g) + 0.5);
               samples.albedo1.rgb = saturate(((samples.albedo1.rgb - 0.5) * ptBC1.g) + 0.5);
               #if !_MAX2LAYER
                 samples.albedo2.rgb = saturate(((samples.albedo2.rgb - 0.5) * ptBC2.g) + 0.5);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(((samples.albedo3.rgb - 0.5) * ptBC3.g) + 0.5);
               #endif
            #endif
            #if _PERTEXBRIGHTNESS
               samples.albedo0.rgb = saturate(samples.albedo0.rgb + ptBC0.rrr);
               samples.albedo1.rgb = saturate(samples.albedo1.rgb + ptBC1.rrr);
               #if !_MAX2LAYER
                  samples.albedo2.rgb = saturate(samples.albedo2.rgb + ptBC2.rrr);
               #endif
               #if !_MAX3LAYER || !_MAX2LAYER
                  samples.albedo3.rgb = saturate(samples.albedo3.rgb + ptBC3.rrr);
               #endif
            #endif
            #if _PERTEXPOROSITY
            porosity = BlendWeights(ptBC0.b, ptBC1.b, ptBC2.b, ptBC3.b, heightWeights);
            #endif

            #if _PERTEXFOAM
            streamFoam = BlendWeights(ptBC0.a, ptBC1.a, ptBC2.a, ptBC3.a, heightWeights);
            #endif

         #endif

         #if _PERTEXNORMSTR || _PERTEXAOSTR || _PERTEXSMOOTHSTR || _PERTEXMETALLIC
            SAMPLE_PER_TEX(perTexMatSettings, 2.5, config, half4(1.0, 1.0, 1.0, 0.0));
         #endif

         #if _PERTEXNORMSTR

            samples.normSAO0.xy *= perTexMatSettings0.r;
            samples.normSAO1.xy *= perTexMatSettings1.r;
            #if !_MAX2LAYER
               samples.normSAO2.xy *= perTexMatSettings2.r;
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.xy *= perTexMatSettings3.r;
            #endif
         #endif

         #if _PERTEXAOSTR
            samples.normSAO0.a = pow(samples.normSAO0.a, perTexMatSettings0.b);
            samples.normSAO1.a = pow(samples.normSAO1.a, perTexMatSettings1.b);
            #if !_MAX2LAYER
               samples.normSAO2.a = pow(samples.normSAO2.a, perTexMatSettings2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.a = pow(samples.normSAO3.a, perTexMatSettings3.b);
            #endif
         #endif

         #if _PERTEXSMOOTHSTR
            samples.normSAO0.b += perTexMatSettings0.g;
            samples.normSAO1.b += perTexMatSettings1.g;
            samples.normSAO0.b = saturate(samples.normSAO0.b);
            samples.normSAO1.b = saturate(samples.normSAO1.b);
            #if !_MAX2LAYER
               samples.normSAO2.b += perTexMatSettings2.g;
               samples.normSAO2.b = saturate(samples.normSAO2.b);
            #endif
            #if !_MAX3LAYER || !_MAX2LAYER
               samples.normSAO3.b += perTexMatSettings3.g;
               samples.normSAO3.b = saturate(samples.normSAO3.b);
            #endif
         #endif

         #if ((_DETAILNOISE && _PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && _PERTEXDISTANCENOISESTRENGTH)) || (_NORMALNOISE && _PERTEXNORMALNOISESTRENGTH)
         ApplyDetailDistanceNoisePerTex(samples, config, camDist);
         #endif

         #if _ANTITILEARRAYDETAIL || _ANTITILEARRAYDISTANCE || _ANTITILEARRAYNORMAL
         ApplyAntiTilePerTex(samples, config, camDist);
         #endif

         #if _GEOMAP && _PERTEXGEO
         GeoTexturePerTex(samples, i.worldPos, config);
         #endif

         #if _GLOBALTINT && _PERTEXGLOBALTINTSTRENGTH
         GlobalTintTexturePerTex(samples, config, camDist);
         #endif

         #if _GLOBALNORMALS && _PERTEXGLOBALNORMALSTRENGTH
         GlobalNormalTexturePerTex(samples, config, camDist);
         #endif

         #if _PERTEXMETALLIC
            half metallic = BlendWeights(perTexMatSettings0.a, perTexMatSettings1.a, perTexMatSettings2.a, perTexMatSettings3.a, heightWeights);
            o.Metallic = metallic;
         #endif

         #if _GLITTER
            DoGlitter(i, samples, config, camDist, worldNormalVertex);
         #endif
         // Blend em..
         half4 albedo = BlendWeights(samples.albedo0, samples.albedo1, samples.albedo2, samples.albedo3, heightWeights);
         half4 normSAO = BlendWeights(samples.normSAO0, samples.normSAO1, samples.normSAO2, samples.normSAO3, heightWeights);

         #if _USEEMISSIVEMETAL
         half4 emisMetal = BlendWeights(samples.emisMetal0, samples.emisMetal1, samples.emisMetal2, samples.emisMetal3, heightWeights);
         #endif

		   // ADVANCEDTERRAIN_ENTRYPOINT	

         #if _MESHOVERLAYSPLATS || _MESHCOMBINED
            o.Alpha = 1.0;
            if (config.uv0.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.x;
            else if (config.uv1.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.y;
            else if (config.uv2.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.z;
            else if (config.uv3.z == _MeshAlphaIndex)
               o.Alpha = 1 - heightWeights.w;
         #endif



         // effects which don't require per texture adjustments and are part of the splats sample go here. 
         // Often, as an optimization, you can compute the non-per tex version of above effects here..


         #if ((_DETAILNOISE && !_PERTEXDETAILNOISESTRENGTH) || (_DISTANCENOISE && !_PERTEXDISTANCENOISESTRENGTH) || (_NORMALNOISE && !_PERTEXNORMALNOISESTRENGTH))
         ApplyDetailDistanceNoise(albedo.rgb, normSAO, config, camDist);
         #endif

         #if _GEOMAP && !_PERTEXGEO
         GeoTexture(albedo.rgb, i.worldPos, config);
         #endif

         #if _GLOBALTINT && !_PERTEXTINTSTRENGTH
         GlobalTintTexture(albedo.rgb, config, camDist);
         #endif

         #if _VSGRASSMAP
         VSGrassTexture(albedo.rgb, config, camDist);
         #endif

         #if _GLOBALNORMALS && !_PERTEXGLOBALNORMAL
         GlobalNormalTexture(normSAO, config, camDist);
         #endif

         o.Albedo = albedo.rgb;
         o.Height = albedo.a;
         o.Normal = half3(normSAO.xy, 1);
         o.Smoothness = normSAO.b;
         o.Occlusion = normSAO.a;

         #if _USEEMISSIVEMETAL
         o.Emission = emisMetal.rgb;
         o.Metallic = emisMetal.a;
         #endif

         #if _MESHCOMBINED
         SampleMeshCombined(o, config);
         #endif

         half pud = 0;


         #if _WETNESS || _PUDDLES || _STREAMS || _LAVA
         pud = DoStreams(o, fxLevels, config.uv, porosity, waterNormalFoam, worldNormalVertex, streamFoam, wetLevel, burnLevel, i.worldPos);
         #endif

         half snowCover = 0;
         #if _SNOW
         snowCover = DoSnow(o, config.uv, WorldNormalVector(i, o.Normal), worldNormalVertex, i.worldPos, pud, porosity, camDist, config, weights);
         #endif

         #if _SNOWGLITTER
            DoSnowGlitter(i, config, o, camDist, worldNormalVertex, snowCover);
         #endif

         #if _WINDPARTICULATE || _SNOWPARTICULATE
         DoWindParticulate(i, o, config, weights, camDist, worldNormalVertex, snowCover);
         #endif

         o.Normal.z = sqrt(1 - saturate(dot(o.Normal.xy, o.Normal.xy)));

         #if _SPECULARFADE
         {
            float specFade = saturate((i.worldPos.y - _SpecularFades.x) / max(_SpecularFades.y - _SpecularFades.x, 0.0001));
            o.Metallic *= specFade;
            o.Smoothness *= specFade;
         }
         #endif

         #if _VSSHADOWMAP
         VSShadowTexture(o, i, config, camDist);
         #endif

         return o;
      }

      MicroSplatLayer SurfImpl(Input i, float3 worldNormalVertex)
      {
      	 #if _TERRAINBLENDABLESHADER && _TRIPLANAR
      	 worldNormalVertex = WorldNormalVector(i, float3(0,0,1));
      	 #endif

         #if _ALPHABELOWHEIGHT
         ClipWaterLevel(i.worldPos);
         #endif

         #if _MICROMESH && _MESHUV2
         float2 origUV = i.uv2_Diffuse;
         #else
         float2 origUV = i.uv_Control0;
         #endif

         #if _MICROMESH
         origUV -= _UVMeshRange.zw;
         origUV *= _UVMeshRange.xy;
         origUV += _UVMeshRange.zw / _UVMeshRange.xy;
         #endif

         half4 weights;

         Config config = (Config)0;
         UNITY_INITIALIZE_OUTPUT(Config,config);

         fixed4 w0 = UNITY_SAMPLE_TEX2D(_Control0, origUV);
         fixed4 w1 = 0; fixed4 w2 = 0; fixed4 w3 = 0; fixed4 w4 = 0; fixed4 w5 = 0; fixed4 w6 = 0; fixed4 w7 = 0;

         #if !_MAX4TEXTURES
         w1 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control1, _Control0, origUV);
         #endif

         #if !_MAX4TEXTURES && !_MAX8TEXTURES
         w2 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control2, _Control0, origUV);
         #endif

         #if !_MAX4TEXTURES && !_MAX8TEXTURES && !_MAX12TEXTURES
         w3 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control3, _Control0, origUV);
         #endif

         #if _MAX20TEXTURES || _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         w4 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control4, _Control0, origUV);
         #endif

         #if _MAX24TEXTURES || _MAX28TEXTURES || _MAX32TEXTURES
         w5 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control5, _Control0, origUV);
         #endif

         #if _MAX28TEXTURES || _MAX32TEXTURES
         w6 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control6, _Control0, origUV);
         #endif

         #if _MAX32TEXTURES
         w7 = UNITY_SAMPLE_TEX2D_SAMPLER(_Control7, _Control0, origUV);
         #endif

         Setup(weights, i.uv_Control0, config, w0, w1, w2, w3, w4, w5, w6, w7, i.worldPos);

         #if _TEXTURECLUSTER2 || _TEXTURECLUSTER3
         PrepClusters(i.uv_Control0, config);
         #endif


         #if _ALPHAHOLE
         ClipAlphaHole(config.uv0.z);
         #endif



         float camDist = distance(_WorldSpaceCameraPos, i.worldPos);


 
         MicroSplatLayer l = Sample(i, weights, config, camDist, worldNormalVertex);


         // Unity has a compiler bug with surface shaders where in some situations it will strip/fuckup
         // i.worldPos or i.viewDir thinking your not using them when you are inside a function. I have
         // fought with this bug so many times it's crazy, reported it and provided repros, and nothing has
         // been done about it. So, make sure these are used, and look like they could have an effect on the final
         // output so the compiler doesn't fuck them up.
         l.Albedo *= saturate(normalize(i.viewDir + i.worldPos) + 9999);


         return l;

      }



   

      #if _BDRFLAMBERT
      void surf (Input i, inout SurfaceOutput o) 
      #else
      void surf (Input i, inout SurfaceOutputStandard o) 
      #endif
      {
         o.Normal = float3(0,0,1);
         float3 worldNormalVertex = float3(0,1,0);
         #if _SNOW || _TRIPLANAR || _STREAMS || _LAVA || _GLITTER || _SNOWGLITTER
         worldNormalVertex = WorldNormalVector(i, float3(0,0,1));
         #endif

         MicroSplatLayer l = SurfImpl(i, worldNormalVertex);

         // always write to o.Normal to keep i.viewDir consistent
         o.Normal = half3(0, 0, 1);

         #if _DEBUG_OUTPUT_ALBEDO
            o.Albedo = l.Albedo;
         #elif _DEBUG_OUTPUT_NORMAL
            o.Albedo = float3(l.Normal.xy * 0.5 + 0.5, l.Normal.z);
         #elif _DEBUG_OUTPUT_SMOOTHNESS
            o.Albedo = l.Smoothness.xxx;
         #elif _DEBUG_OUTPUT_METAL
            o.Albedo = l.Metallic.xxx;
         #elif _DEBUG_OUTPUT_AO
            o.Albedo = l.Occlusion.xxx;
         #elif _DEBUG_OUTPUT_EMISSION
            o.Albedo = l.Emission;
         #elif _DEBUG_OUTPUT_HEIGHT
            o.Albedo = l.Height.xxx;
         #else
            #if _BDRFLAMBERT
               o.Albedo = l.Albedo;
               o.Normal = l.Normal;
               o.Emission = l.Emission;
               o.Specular = l.Occlusion;
               o.Gloss = l.Smoothness;
            #else
               o.Albedo = l.Albedo;
               o.Normal = l.Normal;
               o.Smoothness = l.Smoothness;
               o.Metallic = l.Metallic;
               o.Occlusion = l.Occlusion;
               o.Emission = l.Emission;
            #endif
            #if _MESHOVERLAYSPLATS
               o.Alpha = l.Alpha;
            #endif
         #endif
      }

      // for debug shaders
      half4 LightingUnlit(SurfaceOutputStandard s, half3 lightDir, half atten)
      {
         return half4(s.Albedo, 1);
      }


   
ENDCG

   }
   Dependency "AddPassShader" = "Hidden/MicroSplat/AddPass"
   Dependency "BaseMapShader" = "Hidden/MicroSplat/Example_Base-1410978874"
   CustomEditor "MicroSplatShaderGUI"
   Fallback "Nature/Terrain/Diffuse"
}
