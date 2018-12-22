Shader "Loonim/13-2" { // Voronoi (Chebyshev) F0

Properties {
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
		#include "StdLoonimDraw.cginc"
		#include "ImprovedVoronoiNoise2D.cginc"
		
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			
			float2 f = inoisech(i.uv, _Data.w) * _Data.z;
			
			float n=0.1 + sqrt(f[0]);
			// 0.1 + sqrt(f[1]) - sqrt(f[0]);
			
			return float4(n,n,n,1.0);
			
		}
		
		ENDCG
		
	}
	
}

}
