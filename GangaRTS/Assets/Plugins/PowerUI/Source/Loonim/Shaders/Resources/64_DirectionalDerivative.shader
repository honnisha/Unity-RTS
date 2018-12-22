Shader "Loonim/64" { // Directional derivative of _Src0 with angle _Src1

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Src1("Angle",2D) = "white" {}
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
			
			const float LineLength=0.01;
			const float HalfLine=LineLength/2;
			
			float2 pt=i.uv;
			
			float angle=tex2D(_Src1,pt).r;
			
			// 2 points (horizontal line), rotated clockwise by Angle.
			// Length of the line is a constant.
			// Sample those two points, then simply compute the difference.
			
			// Read angle:
			float sA=sin(angle);
			float cA=cos(angle);
			
			// Base points (rotate first)
			float rightX=cA * HalfLine;
			float rightY=sA * HalfLine;
			
			// Read colour:
			float4 _0a=tex2D(_Src0,float2(pt.x-rightX,pt.y-rightY));
			
			// Read colour:
			float4 _0b=tex2D(_Src0,float2(pt.x+rightX,pt.y+rightY));
			
			// Difference divided by distance:
			_0a.rgb=(_0b.r-_0a.r) / LineLength;
			
			return _0a;
		}
		
		ENDCG
	}
}

}