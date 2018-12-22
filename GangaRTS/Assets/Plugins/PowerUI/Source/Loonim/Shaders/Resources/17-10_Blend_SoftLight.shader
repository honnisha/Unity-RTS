Shader "Loonim/17-10" { // Blend (Soft Light)

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Src2("Weight",2D) = "white" {}
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
			float4 _0=tex2D(_Src0,pt);
			float4 _1=tex2D(_Src1,pt);
			
			// Blend factor is..
			float dstA=_0.a;
			float srcA=tex2D(_Src2,pt).r * _1.a;
			
			// W3C soft light blend here.
			float g;
			float a;
			float b;
			
			a=_0.r;
			b=_1.r;
			
			if(b <= 0.5f){
				
				_1.r = a - (1 - 2 * b) * a * (1 - a);
				
			}else{
				
				if(a<=0.25){
					g=((16 * a - 12) * a +4) * a;
				}else{
					g=sqrt(a);
				}
				
				_1.r = a + (2 * b - 1) * (g - a);
				
			}
			
			a=_0.g;
			b=_1.g;
			
			if(b <= 0.5){
				
				_1.g = a - (1 - 2 * b) * a * (1 - a);
				
			}else{
				
				if(a<=0.25){
					g=((16 * a - 12) * a +4) * a;
				}else{
					g=sqrt(a);
				}
				
				_1.g = a + (2 * b - 1) * (g - a);
				
			}
			
			a=_0.b;
			b=_1.b;
			
			if(b <= 0.5f){
				
				_1.b = a - (1 - 2 * b) * a * (1 - a);
				
			}else{
				
				if(a<=0.25){
					g=((16 * a - 12) * a +4) * a;
				}else{
					g=sqrt(a);
				}
				
				_1.b = a + (2 * b - 1) * (g - a);
				
			}
			
			
			// Time to alpha blend!
			
			float dstAinvSrc=dstA * (1 - srcA);
			float outA=max(srcA + dstAinvSrc,0.001);
			
			_0.rgb = ( (_1.rgb * srcA) + (_0.rgb * dstAinvSrc) ) / outA;
			_0.a=outA;
			
			return _0;
			
		}
		
		ENDCG
	}
}

}