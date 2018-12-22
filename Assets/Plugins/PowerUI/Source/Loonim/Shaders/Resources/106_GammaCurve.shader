Shader "Loonim/106" { // GammaCurve

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
			
			// Get curvature:
			float gamma=tex2D(_Src0,pt);
			
			// Result is just a division:
			float v=pt.x/gamma;
			
			return float4(v,v,v,1);
			
		}
		
		ENDCG
	}
}

}