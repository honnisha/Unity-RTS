Shader "Loonim/19" { // Repeat

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
			
			// Stretch out t by times to repeat:
			float rep=tex2D(_Src1,pt).r;
			float noMirror=tex2D(_Src2,pt).r<0.5;
			
			pt*=rep;
			pt=(pt % (noMirror+1));
			
			// Map 0-2 to 0-1 and 1-0 if mirror.
			// It's only ever >1 if mirroring.
			// The first term zeros out when not mirror.
			pt=((pt>1)*((pt * -2)+2))+pt;
			
			// Read source:
			return tex2D(_Src0,pt);
			
		}
		
		ENDCG
	}
}

}