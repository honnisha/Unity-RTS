Shader "Loonim/90" { // Rotate Input Angle

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Source 1",2D) = "white" {}
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
			
			// Get the angle to rotate by (rad):
			float deltaAngle=tex2D(_Src1,pt).r;
			
			// rotate the point x/y deltaAngle about 0.5/0.5
			pt-=0.5;
			
			// Get sin/cos:
			float cAngle=cos(deltaAngle);
			float sAngle=sin(deltaAngle);
			
			float tx=pt.x*cAngle - pt.y*sAngle;
			pt.y=pt.y*cAngle + pt.x*sAngle;
			pt.x=tx;
			
			pt+=0.5;
			
			// Read source:
			return tex2D(_Src0,pt);
			
		}
		
		ENDCG
	}
}

}