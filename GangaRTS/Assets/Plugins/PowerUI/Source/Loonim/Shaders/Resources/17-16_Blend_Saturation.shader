Shader "Loonim/17-16" { // Blend (Sat)

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Src2("Weight",2D) = "white" {}
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
		
		float4 frag(v2f i) : COLOR{
		
			float2 pt=i.uv;
			float4 _0=tex2D(_Src0,pt);
			float4 _1=tex2D(_Src1,pt);
			
			// Blend factor is..
			float dstA=_0.a;
			float srcA=tex2D(_Src2,pt).r * _1.a;
			
			// Saturation:
			
			// Hue/Lum of underlay:
			float3 hueLum=RGBtoHSL(_0.rgb);
			
			// Saturation of the top:
			float sat=saturationHSY(_1.rgb);
			
			// Combine:
			_1.rgb=HSLtoRGB(float3(hueLum[0],sat,hueLum[2]));
			
			// Time to alpha blend!
			
			float dstAinvSrc=dstA * (1 - srcA);
			float outA=max(srcA + dstAinvSrc,0.001);
			
			_0.rgb = ( (_1.rgb * srcA) + (_0.rgb * dstAinvSrc) ) / outA;
			_0.a=outA;
			
			return _0;
			
		}
		
		ENDCG
	}
}

}