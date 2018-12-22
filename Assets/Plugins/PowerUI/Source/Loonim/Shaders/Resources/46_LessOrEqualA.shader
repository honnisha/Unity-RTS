Shader "Loonim/46" { // _Src0 <= _Src1 ? _Src2 : _Src3

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Src2("If True",2D) = "white" {}
	_Src3("If False",2D) = "white" {}
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
			float4 _2=tex2D(_Src2,pt);
			float4 _3=tex2D(_Src3,pt);
			
			if(_0.r<=_1.r){
				_0.r=_2.r;
			}else{
				_0.r=_3.r;
			}
			
			if(_0.g<=_1.g){
				_0.g=_2.g;
			}else{
				_0.g=_3.g;
			}
			
			if(_0.b<=_1.b){
				_0.b=_2.b;
			}else{
				_0.b=_3.b;
			}
			
			if(_0.a<=_1.a){
				_0.a=_2.a;
			}else{
				_0.a=_3.a;
			}
			
			return _0;
			
		}
		
		ENDCG
	}
}

}