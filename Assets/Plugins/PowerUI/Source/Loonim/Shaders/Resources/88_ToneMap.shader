Shader "Loonim/88" { // ToneMap.

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
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			// Read source:
			float4 _0=tex2D(_Src0,pt);
			
			// Read the tone value for each channel (except alpha):
			pt.x=_0.r;
			_0.r=tex2D(_Src1,pt).r;
			
			pt.x=_0.g;
			_0.g=tex2D(_Src1,pt).r;
			
			pt.x=_0.b;
			_0.b=tex2D(_Src1,pt).r;
			
			return _0;
			
		}
		
		ENDCG
	}
}

}