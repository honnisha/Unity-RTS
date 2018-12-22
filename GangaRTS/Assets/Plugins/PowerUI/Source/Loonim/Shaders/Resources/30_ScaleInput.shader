Shader "Loonim/30" { // Scale Input

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
			float a=tex2D(_Src1,pt);
			pt.y*=tex2D(_Src2,pt);
			
			return tex2D(_Src0,float2(pt.x * a, pt.y));
			
		}
		
		ENDCG
	}
}

}