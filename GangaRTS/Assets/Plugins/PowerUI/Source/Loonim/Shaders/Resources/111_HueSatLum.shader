Shader "Loonim/111" { // Modify hue/sat/lum in _Src0

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "black" {}
	_Src2("Source 2",2D) = "black" {}
	_Src3("Source 3",2D) = "black" {}
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
		
		sampler2D _Src2;
		sampler2D _Src3;
		
		float4 frag(v2f i) : COLOR{
			
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			
			// Amount to vary:
			float hueBoost=1+tex2D(_Src1,pt).r;
			float satBoost=1+tex2D(_Src2,pt).r;
			float lumBoost=1+tex2D(_Src3,pt).r;
			
			// Compute the hue (Using HSL here):
			float3 hsl=RGBtoHSL(_0.rgb);
			
			// Boost the hue/sat/lum:
			hsl.r=(hueBoost + hsl.r) % 1;
			hsl.g*=satBoost;
			hsl.b*=lumBoost;
			
			// Back to RGB:
			_0.rgb=HSLtoRGB(hsl);
			
			return _0;
		}
		
		ENDCG
	}
}

}