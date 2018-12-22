Shader "Loonim/113-1" { // Occlusion

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
			#define INPUT_SAMPLE_COUNT 14
			const float3 RAND_SAMPLES[INPUT_SAMPLE_COUNT] = {
				float3(0.4010039,0.8899381,-0.01751772),
				float3(0.1617837,0.1338552,-0.3530486),
				float3(-0.2305296,-0.1900085,0.5025396),
				float3(-0.6256684,0.1241661,0.1163932),
				float3(0.3820786,-0.3241398,0.4112825),
				float3(-0.08829653,0.1649759,0.1395879),
				float3(0.1891677,-0.1283755,-0.09873557),
				float3(0.1986142,0.1767239,0.4380491),
				float3(-0.3294966,0.02684341,-0.4021836),
				float3(-0.01956503,-0.3108062,-0.410663),
				float3(-0.3215499,0.6832048,-0.3433446),
				float3(0.7026125,0.1648249,0.02250625),
				float3(0.03704464,-0.939131,0.1358765),
				float3(-0.6984446,-0.6003422,-0.04016943),
			};
			return frag_ao (i, RAND_SAMPLES);
		}
		ENDCG

	}

}

Fallback off
}