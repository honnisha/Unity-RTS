Shader "Loonim/38" { // Contrast + Origin (_Src0 boosted by _Src1)

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
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			float _1=tex2D(_Src1,pt).r;
			float _2=tex2D(_Src1,pt).r;
			
			_0.rgb= ( (_0.rgb-_2 ) * _1 ) + _2;
			
			return _0;
			
		}
		
		ENDCG
	}
}

}