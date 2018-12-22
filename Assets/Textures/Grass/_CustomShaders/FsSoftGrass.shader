Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
	//(C) Jere Heikura - FoggySoft
	Properties{
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 1
	}

		SubShader{
		Tags{
		"Queue" = "Geometry+200"
		"Queue"="Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Grass"
		"DisableBatching" = "True"
	}
		Cull Off
		LOD 150
		ColorMask RGB

CGPROGRAM
#pragma surface surf Lambert alpha
#include "TerrainEngine.cginc"

	sampler2D _MainTex;
	sampler2D _CutTex;
	float _Cutoff;

	struct Input {
		float2 uv_MainTex;
		fixed4 color : COLOR;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
		float ca = tex2D(_CutTex, IN.uv_MainTex).a;
		o.Albedo = c.rgb;
		
		if (ca > _Cutoff)
			o.Alpha = c.a;
		else
			o.Alpha = 0;
	}
	ENDCG
	}

		Fallback Off
}
