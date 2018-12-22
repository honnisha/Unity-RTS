Shader "Loonim/70" { // Gamma correct _Src0

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Gamma",2D) = "white" {}
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
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			
			// Read Gamma:
			float _1=tex2D(_Src1,pt).r;
			
			// Map gamma into the 0.1 to 10 range, and invert it:
			_1=1.0 / pow(10.0,_1);
			
			_0.rgb*=_1;
			
			return _0;
		}
		
		ENDCG
	}
}

}