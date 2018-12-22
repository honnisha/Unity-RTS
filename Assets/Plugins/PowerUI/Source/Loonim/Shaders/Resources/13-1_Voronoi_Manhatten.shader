Shader "Loonim/13-1" { // Voronoi (Manhatten) F0

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
		#include "UnityCG.cginc" 
		#pragma vertex vert
		#pragma fragment frag
		
		#include "ImprovedVoronoiNoise2D.cginc"
		
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
		
		uniform float4 _Data;
		
		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos =	v.vertex;
			o.uv=(v.texcoord * _Data.y) + _Data.x;
			
			return o;
		}
		
		float4 frag(v2f i) : COLOR
		{
			
			float2 f = inoisemh(i.uv, _Data.w) * _Data.z;
			
			float n=0.1 + sqrt(f[0]);
			// 0.1 + sqrt(f[1]) - sqrt(f[0]);
			
			return float4(n,n,n,1.0);
			
		}
		
		ENDCG
		
	}
	
}

}
