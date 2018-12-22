Shader "Loonim/104" { // GaussianCurve

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
		
		
		#define NoDataInput
		#include "StdLoonimDraw.cginc"
		#include "StdLoonimColours.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			// Read params:
			float mu=tex2D(_Src0,pt);
			float sigma2=tex2D(_Src1,pt);
			
			float tMu=pt.x - mu;
			
			// Amplitude of 1.
			float v=exp( (- tMu * tMu) / (2.0 * sigma2) );
			
			return float4(v,v,v,1);
			
		}
		
		ENDCG
	}
}

}