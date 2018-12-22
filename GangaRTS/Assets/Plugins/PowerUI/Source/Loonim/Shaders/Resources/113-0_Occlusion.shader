Shader "Loonim/113-0" { // Occlusion

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
			#define INPUT_SAMPLE_COUNT 8
			const float3 RAND_SAMPLES[INPUT_SAMPLE_COUNT] = {
				float3(0.01305719,0.5872321,-0.119337),
				float3(0.3230782,0.02207272,-0.4188725),
				float3(-0.310725,-0.191367,0.05613686),
				float3(-0.4796457,0.09398766,-0.5802653),
				float3(0.1399992,-0.3357702,0.5596789),
				float3(-0.2484578,0.2555322,0.3489439),
				float3(0.1871898,-0.702764,-0.2317479),
				float3(0.8849149,0.2842076,0.368524),
			};
			return frag_ao (i, RAND_SAMPLES);
		}
		ENDCG

	}

}

Fallback off
}