Shader "Loonim/85" { // Levels

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Min (Black)",2D) = "black" {}
	_Src2("Max (White)",2D) = "white" {}
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
		
		sampler2D _Src2;
		
		float4 frag(v2f i) : COLOR{
			
			float2 pt=i.uv;
			float4 _0=tex2D(_Src0,pt);
			float4 _1=tex2D(_Src1,pt);
			float4 _2=tex2D(_Src2,pt);
			
			// Divide by range and offset by min:
			_0.rgb=( (_0.rgb) / (_2.rgb - _1.rgb) ) - _1.rgb;
			
			// Offset:
			return _0;
			
		}
		
		ENDCG
	}
}

}