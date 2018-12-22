Shader "Loonim/39" { // Brightness (_Src0 boosted by _Src1)

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
}

SubShader {
	Pass {
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		
		#pragma target 3.0
		#define NoDataInput
		#include "StdLoonimDraw.cginc"
		#include "StdLoonimColours.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			
			// Amount to boost:
			float lBoost=tex2D(_Src1,pt).r;
			
			// Get the raw hsl:
			float3 hsl=RGBtoHSL(_0.rgb);
			
			// Boost the brightness (l, in blue):
			hsl.b*=(1+lBoost);
			
			// Combine:
			_0.rgb=RGBtoHSL(hsl);
			
			return _0;
			
		}
		
		ENDCG
	}
}

}