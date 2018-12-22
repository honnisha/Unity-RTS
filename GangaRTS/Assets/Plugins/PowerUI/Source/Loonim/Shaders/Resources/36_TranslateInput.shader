Shader "Loonim/36" { // Translate Input

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("X",Float) = 0
	_Src2("Y",Float) = 0
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
			
			return tex2D(_Src0,float2(pt.x + _Src1, pt.y + _Src2));
			
		}
		
		ENDCG
	}
}

}