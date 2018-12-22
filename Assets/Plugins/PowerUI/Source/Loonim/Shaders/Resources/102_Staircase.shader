Shader "Loonim/102" { // Staircase

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
			
			// How many stairs?
			float count=tex2D(_Src1,pt);
			
			// Map x into our current step:
			pt.x*=count;
			
			// Get the base step:
			float baseStep=floor(pt.x);
			
			// Read the curve at x-baseStep (0-1 inside the current step):
			pt.x-=baseStep;
			
			float v=tex2D(_Src0,pt);
			
			// Gotta now offset and compress it back:
			v=( baseStep + v) / count;
			
			return float4(v,v,v,1);
			
		}
		
		ENDCG
	}
}

}