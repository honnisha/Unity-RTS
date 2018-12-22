Shader "Loonim/17-17" { // Blend (Colour)

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
			
			// Colour:
			
			// hue/sat of top:
			float3 hueSat=RGBtoHSL(_1.rgb);
			
			// Luminosity of the underlay:
			float lum=RGBtoHSL(_0.rgb).b;
			
			// Combine:
			_1.rgb=HSLtoRGB(float3(hueSat[0],hueSat[1],lum));
			
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