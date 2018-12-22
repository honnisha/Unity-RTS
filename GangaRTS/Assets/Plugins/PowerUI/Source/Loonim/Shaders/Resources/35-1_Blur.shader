Shader "Loonim/35-1" { // Blur (Gaussian fast)

Properties {
	_Src0("Source 0",2D) = "white" {}
	_Data("Radius X in UV (x), Radius Y in UV (y), Radius X in UV (z), Negative Radius Y in UV (w)",Vector) = (0,0,0,0)
}

SubShader {
	Pass {
		
		Blend Off
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		#include "UnityCG.cginc" 
		#pragma vertex vert
		#pragma fragment frag
		
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			half2 taps[4] : TEXCOORD1; 
		};
		
		uniform float4 _Data;
		sampler2D _Src0;
		
		v2f vert(appdata_base v)
		{
			// xy is radius in terms of the uv space. zw is xy * (1,-1)
			
			v2f o;
			o.pos =	v.vertex;
			o.uv = v.texcoord;
			o.taps[0] = o.uv + _Data.xy;
			o.taps[1] = o.uv - _Data.xy;
			o.taps[2] = o.uv + _Data.zw;
			o.taps[3] = o.uv - _Data.zw;
			return o;
		}
		
		float4 frag(v2f i) : COLOR
		{
			half4 color = tex2D(_Src0, i.taps[0]);
			color += tex2D(_Src0, i.taps[1]);
			color += tex2D(_Src0, i.taps[2]);
			color += tex2D(_Src0, i.taps[3]); 
			return color * 0.25;
		}
		
		ENDCG
	}
}

}