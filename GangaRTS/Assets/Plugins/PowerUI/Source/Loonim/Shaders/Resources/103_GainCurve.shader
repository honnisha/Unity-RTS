Shader "Loonim/103" { // GainCurve

Properties {
	_Src0("Source 0",2D) = "white" {}
}

SubShader {
	Pass {
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		
		#define OneInput
		#define NoDataInput
		#include "StdLoonimDraw.cginc"
		#include "StdLoonimColours.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			float gain=tex2D(_Src0,pt);
			
			float v = (1.0 / gain - 2.0) * (1.0 - 2.0 * pt.x);
			
			v=(pt.x < 0.5) ? (pt.x / (v + 1.0)) : ( (v - pt.x) / (v - 1.0) );
			
			return float4(v,v,v,1);
			
		}
		
		ENDCG
	}
}

}