Shader "Loonim/84" { // Combine _Src0, _Src1

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Alpha",2D) = "white" {}
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
			_0.a=tex2D(_Src1,pt).r;
			
			return _0;
			
		}
		
		ENDCG
	}
}

}