Shader "Loonim/43" { // Modify lum (HSL) in _Src0

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "black" {}
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
		
		float4 frag(v2f i) : COLOR{
			
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			
			// Amount to vary:
			float lumBoost=1+tex2D(_Src1,pt).r;
			
			// Compute the hue (Using HSL here):
			float3 hsl=RGBtoHSL(_0.rgb);
			
			// Boost the lum:
			hsl.b*=lumBoost;
			
			// Back to RGB:
			_0.rgb=HSLtoRGB(hsl);
			
			return _0;
		}
		
		ENDCG
	}
}

}