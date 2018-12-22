#include "UnityCG.cginc" 

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

uniform float4 _Data;

#ifdef NoInput

#else

	#ifdef OneInput

	sampler2D _Src0;

	#else

	sampler2D _Src0;
	sampler2D _Src1;

	#endif

#endif

#ifdef Blend

v2f vert(appdata_base v)
{
	v2f o;
	o.pos =	v.vertex;
	o.uv=(v.texcoord * _Data.xy) + _Data.zw;
	
	return o;
}

#else

	#ifdef NoDataInput

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos =	v.vertex;
		o.uv=v.texcoord;
		
		return o;
	}

	#else

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos =	v.vertex;
		o.uv=(v.texcoord * _Data.y) + _Data.x;
		
		return o;
	}

	#endif
	
#endif