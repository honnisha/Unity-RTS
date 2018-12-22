Shader "Loonim/91" { // Abs Threshold

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Src2("Source 2",2D) = "white" {}
	_Src3("Source 3",2D) = "white" {}
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
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			float4 _1=tex2D(_Src1,pt);
			float threshold=tex2D(_Src2,pt).r;
			float gain=(tex2D(_Src3,pt).r*4)+1;
			
			// Get deltas:
			float3 delta = abs(_0.rgb - _1.rgb);
			
			// Threshold test:
			float3 tests=( delta.rgb>=threshold );
			
			_0.rgb = ((1-tests.rgb) * _0.rgb) + ( tests.rgb * (gain * delta.rgb + _1.rgb) );
			
			return _0;
		}
		
		ENDCG
	}
}

}