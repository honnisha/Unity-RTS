Shader "Loonim/28" { // Rotate Input

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Rotation",Vector) = (1,0,0,1)
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
		
		uniform float4 _Src1;
		
		float4 frag(v2f i) : COLOR
		{
			
			float2 pt=i.uv;
			
			float nx = (_Src1[0] * pt.x) + (_Src1[2] * pt.y);
            float ny = (_Src1[1] * pt.x) + (_Src1[3] * pt.y);
			
			return tex2D(_Src0,float2(nx,ny));
			
		}
		
		ENDCG
	}
}

}