Shader "Loonim/13-3" { // Voronoi (Minkowski) F0

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Data("Seed (x), Frequency (y), Amplitude(z), Jitter (w)",Vector) = (0,0,0,0)
}

SubShader {
	Pass {
	
		Blend SrcAlpha OneMinusSrcAlpha
		
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		#pragma target 3.0
		#define OneInput
		#include "StdLoonimDraw.cginc"
		#include "ImprovedVoronoiNoise2D.cginc"
		
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			
			float mkNumber=tex2D(_Src0,i.uv).r;
			
			float2 f = inoisemi(i.uv, _Data.w,mkNumber) * _Data.z;
			
			float n=0.1 + sqrt(f[0]);
			// 0.1 + sqrt(f[1]) - sqrt(f[0]);
			
			return float4(n,n,n,1.0);
			
		}
		
		ENDCG
		
	}
	
}

}
