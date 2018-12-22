Shader "Loonim/33" { // Smaller Output

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
	_Data("Offset X (x), Offset Y (y), Scale X (z), Scale Y (w)",Vector) = (0,0,1,1)
}

SubShader {
	Pass {
		
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		
		#define Blend
		#include "StdLoonimDraw.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			
			float2 pt=i.uv;
			float4 _0=tex2D(_Src0,pt);
			float4 _1=tex2D(_Src1,pt);
			
			_0.rgb=min(_0.rgb,_1.rgb);
			
			return _0;
		}
		
		ENDCG
	}
}

}