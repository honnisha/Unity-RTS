Shader "Custom/Texture Blend" {
Properties{
	// _Color ("Overall Color", Color) = (1,0.5,0.5,1)

	 _ac("alpha cutoff for textures", Range(.0,1)) = 0
	 _acm("alpha cutoff for map", Range(.0,1)) = 0
	 _t1("texture in black", 2D) = "white" {}
	 _tint1("Tint1", Color) = (1.0, 1, 1, 0)

	 _t2("texture in red", 2D) = "white" {}
	 _tint2("Tint2", Color) = (1.0, 1, 1, 0)
	 _rc("amplify red", Range(1,80)) = 0.0

	 _t3("texture in blue", 2D) = "white" {}
	 _tint3("Tint3", Color) = (1.0,1,1,0)
	 _rb("amplify blue", Range(1,80)) = 0.0

	 _t5("texture in green", 2D) = "white" {}
	 _tint5("Tint4", Color) = (1.0,1,1,0)
	 _rg("amplify green", Range(1,80)) = 0.0

	 _t4("texture map", 2D) = "white" {}

	 num("Number of textures used", Int) = 5

}
SubShader{
	Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
	LOD 200

	CGPROGRAM
		 #pragma surface surf Lambert alpha
	int num;
	float _rc;
	float _rb; float _rg;
	float _ac;
	float _acm;
	sampler2D _t1;
	sampler2D _t2;
	sampler2D _t3;
	sampler2D _t4;
	 sampler2D _t5;
	fixed4 _tint1;
	fixed4 _tint2;
	fixed4 _tint3;
	 fixed4 _tint5;
	struct Input {

		float2 uv_t1;
		float2 uv_t2;
		float2 uv_t3;
		float2 uv_t4;
		float2 uv_t5;
	};


	void surf(Input IN, inout SurfaceOutput o) {

		float f; float f2;

		half4 pix2;
		half4 pix = tex2D(_t1, IN.uv_t1)*_tint1;
			  pix.a = tex2D(_t1, IN.uv_t1).a;
		half4 map = tex2D(_t4, IN.uv_t4);
		float alph;

			if (num > 1) {
			alph = tex2D(_t2, IN.uv_t2).a;

			if (alph > _ac) {

			f = map.r;
			if (f > 0) {
			f = f * _rc;

			if (f > 1) { f = 1; }

			f *= alph;
			f2 = 1 - f;
			pix = pix * f2;

			pix = pix + tex2D(_t2, IN.uv_t2)*f*_tint2;
			pix.a += tex2D(_t2, IN.uv_t2).a*f;
			}
}


if (num > 2) {
alph = tex2D(_t3, IN.uv_t3).a;

if (alph > _ac) {
f = map.b;
if (f > 0) {
f = f * _rb;
if (f > 1) { f = 1; }
f *= alph;
f2 = 1 - f;
pix = pix * f2;

pix = pix + tex2D(_t3, IN.uv_t3)*f*_tint3;
}
pix.a += tex2D(_t3, IN.uv_t3).a*f;
}
if (num > 3) {
alph = tex2D(_t5, IN.uv_t5).a;

if (alph > _ac) {
f = map.g;
if (f > 0) {
f = f * _rg;
if (f > 1) { f = 1; }
f *= alph;
f2 = 1 - f;
pix = pix * f2;

pix = pix + tex2D(_t5, IN.uv_t5)*f*_tint5;
}
pix.a += tex2D(_t5, IN.uv_t5).a*f;
}


  }
}

}

o.Albedo = pix.rgb;
f = pix.a;
if (map.a < _acm) { map.a = 0; }
if (map.a < pix.a) { f = map.a; }

o.Alpha = f;

}
ENDCG
	 }
		 FallBack "Diffuse"
 }