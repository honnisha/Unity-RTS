Shader "Loonim/65" { // Combine _Src0, _Src1, _Src2

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Src2("Source 2",2D) = "white" {}
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
		
		float4 frag(v2f i) : COLOR
		{
			
			float2 pt=i.uv;
			float r=tex2D(_Src0,pt).r;
			float g=tex2D(_Src1,pt).r;
			float b=tex2D(_Src2,pt).r;
			
			return float4(r,g,b,1);
			
		}
		
		ENDCG
	}
}

}