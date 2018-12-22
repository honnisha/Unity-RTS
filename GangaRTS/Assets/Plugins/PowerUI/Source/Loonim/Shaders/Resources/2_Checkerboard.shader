Shader "Loonim/2" { // Checkerboard

Properties {
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
			pt*=2;
			pt=(floor(pt) % 2);
			
			// XOR:
			float r=(pt.x || pt.y) && !(pt.x && pt.y);
			
			return float4(r,r,r,1);
			
		}
		
		ENDCG
	}
}

}