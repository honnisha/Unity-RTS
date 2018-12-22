Shader "Loonim/71" { // WrapClip

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Src2("Source 2",2D) = "white" {}
	_Src3("Source 3",2D) = "white" {}
	_Src4("Source 4",2D) = "white" {}
}

SubShader {
	Pass {
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		#define NoDataInput
		#include "StdLoonimDraw.cginc"
		#pragma vertex vert
		#pragma fragment frag
		sampler2D _Src2;
		sampler2D _Src3;
		sampler2D _Src4;
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			// Get the bounds:
			float2 min=float2(
				tex2D(_Src1,pt).r,
				tex2D(_Src2,pt).r
			);
			
			// Get the range:
			float2 range=float2(
				tex2D(_Src3,pt).r,
				tex2D(_Src4,pt).r
			);
			
			//Wrap and clip both:
			pt=( (pt-min) % range) + min;
			
			// Read source:
			return tex2D(_Src0,pt);
			
		}
		
		ENDCG
	}
}

}