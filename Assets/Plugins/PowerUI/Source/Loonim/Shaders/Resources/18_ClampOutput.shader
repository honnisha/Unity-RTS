Shader "Loonim/18" { // Clamp Output

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Lower bound",Float) = 0
	_Src2("Upper bound",Float) = 1
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
		#pragma vertex vert
		#pragma fragment frag
		
		uniform float _Src1;
		uniform float _Src2;
		
		float4 frag(v2f i) : COLOR
		{
			float2 pt=i.uv;
			
			float4 _0=tex2D(_Src0,pt);
			
			// Clamp! Ignore alpha though - it comes from col1:
			_0.rgb=clamp(_0.rgb,_Src1,_Src2);
			
			return _0;
		}
		
		ENDCG
	}
}

}