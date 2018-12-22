Shader "Loonim/86" { // Threshold

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Threshold",2D) = "black" {}
	_Src2("Smoothing",2D) = "white" {}
	_Src3("High",2D) = "white" {}
	_Src4("Low",2D) = "black" {}
}

SubShader {
	Pass {
		
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		
		#define NoDataInput
		#include "StdLoonimDraw.cginc"
		#include "StdLoonimColours.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		sampler2D _Src2;
		sampler2D _Src3;
		sampler2D _Src4;
		
		float4 frag(v2f i) : COLOR{
			
			float2 pt=i.uv;
			float4 _0=tex2D(_Src0,pt);
			
			// Threshold:
			float _1=tex2D(_Src1,pt).r;
			
			// Smoothing range (+- half this):
			float _2=tex2D(_Src2,pt).r;
			
			// Get input colours intensity:
			float intensity=(_0.r + _0.g + _0.b) / 3;
			
			// High:
			float4 _3=tex2D(_Src3,pt);
			
			// Low:
			float4 _4=tex2D(_Src4,pt);
			
			// Bottom of the threshold:
			float min=_1-(_2 * 0.5);
			
			// Smooth. Blend between high and low.
			
			// Blending factor is..
			float blend=saturate( ( (intensity - min) / _2) );
			
			// Blend and return:
			_0.rgb=_4.rgb + (_3.rgb - _4.rgb) * blend;
			
			return _0;
		}
		
		ENDCG
	}
}

}