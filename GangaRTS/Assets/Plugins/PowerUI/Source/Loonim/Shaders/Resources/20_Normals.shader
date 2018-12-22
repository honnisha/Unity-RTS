Shader "Loonim/20" { // Normals

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
			
			// 1/pStr:
			float str=tex2D(_Src1,pt).r;
			float mapX=_Data.x;
			float mapY=_Data.y;
			pt.y+=mapY;
			pt.x-=mapX;
			
			// Read the surrounding heights:
			float tl = tex2D(_Src0,pt).r; // x-mapX,y+mapY
			pt.x+=mapX;
			
			float t = tex2D(_Src0,pt).r; // x, y+mapY
			pt.x+=mapX;
			
			float tr = tex2D(_Src0,pt).r; // x+mapX, y+mapY
			pt.y-=mapY;
			
			float r = tex2D(_Src0,pt).r; // x+mapX, y
			pt.y-=mapY;
			
			float br = tex2D(_Src0,pt).r; // x+mapX, y-mapY
			pt.x-=mapX;
			
			float b = tex2D(_Src0,pt).r; // x, y-mapY
			pt.x-=mapX;
			
			float bl = tex2D(_Src0,pt).r; // x-mapX, y-mapY
			pt.y+=mapY;
			
			float l = tex2D(_Src0,pt).r; // x-mapX, y
			
			// sobel filter
			float4 _0=float4(
				( (tr + 2.0 * r + br) - (tl + 2.0 * l + bl) ),
				( (bl + 2.0 * b + br) - (tl + 2.0 * t + tr) ),
				str,
				1
			);
			
			// Normalise:
			float length=sqrt(_0.x * _0.x + _0.y * _0.y + _0.z * _0.z);
			
			_0.rgb/=length;
			
			return _0;
		}
		
		ENDCG
	}
}

}