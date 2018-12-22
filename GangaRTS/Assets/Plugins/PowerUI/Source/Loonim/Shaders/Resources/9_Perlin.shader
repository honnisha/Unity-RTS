Shader "Loonim/9" { // Perlin (Single octave. Must additively blend)

Properties {
	_Data("Seed (x), Frequency (y) and Amplitude(z)",Vector) = (0,0,0)
}

SubShader {
	Pass {
		
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		
		#pragma target 3.0
		#include "UnityCG.cginc" 
		#pragma vertex vert
		#pragma fragment frag
		
		#include "noiseSimplex.cginc"
		
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
		
		uniform float3 _Data;
		
		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos =	v.vertex;
			o.uv=(v.texcoord * _Data.y) + _Data.x;
			
			return o;
		}
		
		float4 frag(v2f i) : COLOR
		{
			float ns = ( _Data.z * snoise(i.uv) ) / 2 + 0.5f;
			return float4(ns, ns, ns, 1.0f);
		}
		
		ENDCG
	}
}

}