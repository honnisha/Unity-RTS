Shader "Loonim/114" { // Custom Mesh

Properties {
	_Src0("Source 0",2D) = "white" {}
}

SubShader {
	Pass {
		
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha // This is the only shader that uses blending. The contents of the target *matter*.
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		ZWrite Off
		
		CGPROGRAM
		
		#define OneInput
		#define NoDataInput
		#include "StdLoonimDraw.cginc"
		#pragma vertex vert
		#pragma fragment frag
		
		float4 frag(v2f i) : COLOR
		{
			// We simply draw something other than a quad.
			return tex2D(_Src0,i.uv);
		}
		
		ENDCG
	}
}

}