Shader "Loonim/113-2" { // Occlusion

Properties {
	_Src0("Source 0",2D) = "white" {} // Depth
	_Src1("Source 1",2D) = "white" {} // Normals
	_Src2("Source 2",2D) = "white" {} // Random (Always present)
	_Data("Radius (X), min z (Y), attenuation power (Z), SSAO power (W)",Vector) = (0,0,1,1)
}

Subshader {
	Pass {
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "loonim_ao.cginc"
		
		struct v2f_ao {
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f_ao vert_ao (appdata_img v)
		{
			v2f_ao o;
			o.pos = v.vertex;
			o.uv = v.texcoord;
			return o;
		}
		
		sampler2D _Src0;
		sampler2D _Src1;
		sampler2D _Src2;
		float4 _Data;
		
		#pragma vertex vert_ao
		#pragma fragment frag
		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest


		half4 frag (v2f_ao i) : COLOR
		{
			#define INPUT_SAMPLE_COUNT 26
			const float3 RAND_SAMPLES[INPUT_SAMPLE_COUNT] = {
				float3(0.2196607,0.9032637,0.2254677),
				float3(0.05916681,0.2201506,-0.1430302),
				float3(-0.4152246,0.1320857,0.7036734),
				float3(-0.3790807,0.1454145,0.100605),
				float3(0.3149606,-0.1294581,0.7044517),
				float3(-0.1108412,0.2162839,0.1336278),
				float3(0.658012,-0.4395972,-0.2919373),
				float3(0.5377914,0.3112189,0.426864),
				float3(-0.2752537,0.07625949,-0.1273409),
				float3(-0.1915639,-0.4973421,-0.3129629),
				float3(-0.2634767,0.5277923,-0.1107446),
				float3(0.8242752,0.02434147,0.06049098),
				float3(0.06262707,-0.2128643,-0.03671562),
				float3(-0.1795662,-0.3543862,0.07924347),
				float3(0.06039629,0.24629,0.4501176),
				float3(-0.7786345,-0.3814852,-0.2391262),
				float3(0.2792919,0.2487278,-0.05185341),
				float3(0.1841383,0.1696993,-0.8936281),
				float3(-0.3479781,0.4725766,-0.719685),
				float3(-0.1365018,-0.2513416,0.470937),
				float3(0.1280388,-0.563242,0.3419276),
				float3(-0.4800232,-0.1899473,0.2398808),
				float3(0.6389147,0.1191014,-0.5271206),
				float3(0.1932822,-0.3692099,-0.6060588),
				float3(-0.3465451,-0.1654651,-0.6746758),
				float3(0.2448421,-0.1610962,0.1289366),
			};
			return frag_ao (i, RAND_SAMPLES);
		}
		ENDCG

	}

}

Fallback off
}