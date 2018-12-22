
/*
* Conversion between RGB and HSV colorspace.
* With thanks to http://www.chilliant.com/rgb2hsv.html
*/

const float Epsilon = 1e-10;

float3 RGBtoHSV(float3 rgb){
	float4 p = lerp(float4(rgb.b, rgb.g, -1, 2.0 / 3.0), float4(rgb.g, rgb.b, 0.0, -1.0 / 3.0), step(rgb.b, rgb.g));
	float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));
	
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float luminanceHSV(float3 rgb){
	float p = lerp(rgb.b, rgb.g, step(rgb.b, rgb.g));
	return lerp(p, rgb.r, step(p, rgb.r));
}

float3 HSVtoRGB(float3 hsv){
	hsv[0] *= 6;
	float R = abs(hsv[0] - 3) - 1;
	float G = 2 - abs(hsv[0] - 2);
	float B = 2 - abs(hsv[0] - 4);
	float3 RGB = saturate(float3(R,G,B));
	return ((RGB - 1) * hsv[1] + 1) * hsv[2];
}

float3 RGBtoHCV(float3 rgb){
	// Based on work by Sam Hocevar and Emil Persson
	float4 P = (rgb.g < rgb.b) ? float4(rgb.b, rgb.g, -1.0, 2.0/3.0) : float4(rgb.g, rgb.b, 0.0, -1.0/3.0);
	float4 Q = (rgb.r < P.x) ? float4(P.xyw, rgb.r) : float4(rgb.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
	return float3(H, C, Q.x);
}

/*
* Conversion between RGB and HSL colorspace.
*/

float3 RGBtoHSL(float3 rgb){
	float3 HCV = RGBtoHCV(rgb);
	float L = HCV.z - HCV.y * 0.5;
	float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
	return float3(HCV.x, S, L);
}

float3 HSLtoRGB(float3 hsl){
	
	float R = abs(hsl[0] * 6 - 3) - 1;
	float G = 2 - abs(hsl[0] * 6 - 2);
	float B = 2 - abs(hsl[0] * 6 - 4);
	float3 RGB = saturate(float3(R,G,B));
	float C = (1 - abs(2 * hsl[2] - 1)) * hsl[1];
	return (RGB - 0.5) * C + hsl[2];
}

float luminanceHSL(float3 rgb){
	
	// Based on work by Sam Hocevar and Emil Persson
	float4 P = (rgb.g < rgb.b) ? float4(rgb.b, rgb.g, -1.0, 2.0/3.0) : float4(rgb.g, rgb.b, 0.0, -1.0/3.0);
	float4 Q = (rgb.r < P.x) ? float4(P.xyw, rgb.r) : float4(rgb.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	return Q.x - C * 0.5;
	
}


/*
 * Conversion between RGB and LAB colorspace.
 * Thanks to https://gist.github.com/mattatz/44f081cac87e2f7c8980
 */

float3 RGBtoXYZ( float3 rgb ) {
    float3 tmp;
    tmp.x = ( rgb.r > 0.04045 ) ? pow( ( rgb.r + 0.055 ) / 1.055, 2.4 ) : rgb.r / 12.92;
    tmp.y = ( rgb.g > 0.04045 ) ? pow( ( rgb.g + 0.055 ) / 1.055, 2.4 ) : rgb.g / 12.92;
    tmp.z = ( rgb.b > 0.04045 ) ? pow( ( rgb.b + 0.055 ) / 1.055, 2.4 ) : rgb.b / 12.92;
    const float3x3 mat = float3x3(
		0.4124, 0.3576, 0.1805,
        0.2126, 0.7152, 0.0722,
        0.0193, 0.1192, 0.9505 
	);
    return 100.0 * mul(tmp, mat);
}

float3 XYZtoLAB( float3 c ) {
    float3 n = c / float3(95.047, 100, 108.883);
    float3 v;
    v.x = ( n.x > 0.008856 ) ? pow( n.x, 1.0 / 3.0 ) : ( 7.787 * n.x ) + ( 16.0 / 116.0 );
    v.y = ( n.y > 0.008856 ) ? pow( n.y, 1.0 / 3.0 ) : ( 7.787 * n.y ) + ( 16.0 / 116.0 );
    v.z = ( n.z > 0.008856 ) ? pow( n.z, 1.0 / 3.0 ) : ( 7.787 * n.z ) + ( 16.0 / 116.0 );
    return float3(( 116.0 * v.y ) - 16.0, 500.0 * ( v.x - v.y ), 200.0 * ( v.y - v.z ));
}

float3 RGBtoLAB( float3 rgb ) {
    float3 lab = XYZtoLAB( RGBtoXYZ( rgb ) );
    return float3( lab.x / 100.0, 0.5 + 0.5 * ( lab.y / 127.0 ), 0.5 + 0.5 * ( lab.z / 127.0 ));
}

float lLAB( float3 rgb ) {
    
	float3 n = RGBtoXYZ( rgb ) / float3(95.047, 100, 108.883);
    float v = ( n.y > 0.008856 ) ? pow( n.y, 1.0 / 3.0 ) : ( 7.787 * n.y ) + ( 16.0 / 116.0 );
    
    return ( 116.0 * v ) - 16.0 / 100.0;
}

float aLAB( float3 rgb ) {
    
	float3 n = RGBtoXYZ( rgb ) / float3(95.047, 100, 108.883);
    float2 v;
    v.x = ( n.x > 0.008856 ) ? pow( n.x, 1.0 / 3.0 ) : ( 7.787 * n.x ) + ( 16.0 / 116.0 );
    v.y = ( n.y > 0.008856 ) ? pow( n.y, 1.0 / 3.0 ) : ( 7.787 * n.y ) + ( 16.0 / 116.0 );
    
    return 0.5 + 0.5 * ( 500.0 * ( v.x - v.y ) / 127.0 );
}

float bLAB(float3 rgb){
	
	float3 n = RGBtoXYZ( rgb ) / float3(95.047, 100, 108.883);
    float2 v;
    v.x = ( n.y > 0.008856 ) ? pow( n.y, 1.0 / 3.0 ) : ( 7.787 * n.y ) + ( 16.0 / 116.0 );
    v.y = ( n.z > 0.008856 ) ? pow( n.z, 1.0 / 3.0 ) : ( 7.787 * n.z ) + ( 16.0 / 116.0 );
    
    return 0.5 + 0.5 * ( 200.0 * ( v.x - v.y ) / 127.0 );
	
}

float3 LABtoXYZ( float3 lab ) {
    float fy = ( lab[0] + 16.0 ) / 116.0;
    float fx = lab[1] / 500.0 + fy;
    float fz = fy - lab[2] / 200.0;
    return float3(
         95.047 * (( fx > 0.206897 ) ? fx * fx * fx : ( fx - 16.0 / 116.0 ) / 7.787),
        100.000 * (( fy > 0.206897 ) ? fy * fy * fy : ( fy - 16.0 / 116.0 ) / 7.787),
        108.883 * (( fz > 0.206897 ) ? fz * fz * fz : ( fz - 16.0 / 116.0 ) / 7.787)
    );
}

float3 XYZtoRGB( float3 c ) {
	const float3x3 mat = float3x3(
        3.2406, -1.5372, -0.4986,
        -0.9689, 1.8758, 0.0415,
        0.0557, -0.2040, 1.0570
	);
    float3 v = mul(c / 100.0, mat);
    float3 r;
    r.x = ( v.r > 0.0031308 ) ? (( 1.055 * pow( v.r, ( 1.0 / 2.4 ))) - 0.055 ) : 12.92 * v.r;
    r.y = ( v.g > 0.0031308 ) ? (( 1.055 * pow( v.g, ( 1.0 / 2.4 ))) - 0.055 ) : 12.92 * v.g;
    r.z = ( v.b > 0.0031308 ) ? (( 1.055 * pow( v.b, ( 1.0 / 2.4 ))) - 0.055 ) : 12.92 * v.b;
    return r;
}

float3 LABtoRGB( float3 lab ) {
    return XYZtoRGB( LABtoXYZ( float3( 100.0 * lab[0], 2.0 * 127.0 * (lab[1] - 0.5), 2.0 * 127.0 * (lab[2] - 0.5) )) );
}

/*
 * Conversion between RGB and HSY colorspace.
 * These are GPU friendly versions of the Loonim file HsyRgb.cs
*/

const float hsy_R=0.3;
const float hsy_B=0.11;
const float hsy_G=0.59;

const float3 hsy_RBG=float3(0.3,0.11,0.59);
const float3 hsy_GRB=float3(0.59,0.3,0.11);
const float hsy_Offsets[6]={0,2,4,3,1,5};

float3 HSYtoRGB(float3 hsy){
	
	hsy.r *= 6;
	
	int hBase=floor(hsy.r);
	float hOffset=hsy.r-hBase;
	float k=hsy.g * hOffset;
	int wrappedBase=(hBase % 3);
	
	// If hBase is even then it's 1, otherwise -1
	int evenFactor=((hBase % 2) * 2) - 1;
	
	// Terms are now as follows:
	float bgr=hsy.b - evenFactor * ( (hsy_RBG[wrappedBase] * hsy.g) + (hsy_GRB[wrappedBase] * k) );
	
	float3 grb=float3(
		clamp(bgr + evenFactor * k,0,1),
		clamp(bgr + evenFactor * hsy.g,0,1),
		clamp(bgr,0,1)
	);
	
	/*
		GRB above was originally like this, resulting in grb actually being green/red/blue:
		
		grb[wrappedBase]=clamp(bgr + evenFactor * k,0,1);
		grb[ (wrappedBase+1)%3 ]=clamp(bgr + evenFactor * hsy.g,0,1);
		grb[ (wrappedBase+2)%3 ]=clamp(bgr,0,1);
		
		However, that indexing fails for SM3 and earlier.
	*/
	
	// When wrappedBase is 0, grb is GRB.
	// When wrappedBase is 1, grb is BGR.
	// When wrappedBase is 2, grb is RBG.
	// (it's "cycling")
	
	// Return in RGB order:
	return float3(
		((wrappedBase==0) * grb[1]) + ((wrappedBase==1) * grb[2]) + ((wrappedBase==2) * grb[0]),
		((wrappedBase==0) * grb[0]) + ((wrappedBase==1) * grb[1]) + ((wrappedBase==2) * grb[2]),
		((wrappedBase==0) * grb[2]) + ((wrappedBase==1) * grb[0]) + ((wrappedBase==2) * grb[1])
	);
	
}

float3 saturationHSY(float3 rgb){
	
	// 'Sort' them. Both of these go from 0-2. They're 2 when r/b are the biggest value:
	int rIndex=(rgb.r>=rgb.g) + (rgb.g>rgb.b);
	int bIndex=(rgb.b>=rgb.r) + (rgb.r>rgb.g);
	
	// 6 possible combinations in two groups. Biggest channel on the left:
	// Sector 0: rgb
	// Sector 1: gbr*
	// Sector 2: brg
	
	// Sector 3: bgr
	// Sector 4: grb
	// Sector 5: rbg
	
	// * Note that r==g==b ends up as gbr (h and s come out as 0)
	
	// The order of these was selected based being able to figure out the sector
	// from rIndex and bIndex. Look at how 'b' goes diagonally through the top set
	// and how 'r' goes diagonally through the bottom one.
	// Group 1: sector == bIndex
	// Group 2: sector == 3 + rIndex
	
	// However, we need to distinguish which group we're in.
	// (rIndex-bIndex) is 2 or -1 for the first group and 1 or -2 for the second.
	// They're unique, so this is what we use to figure out which group we're actually in.
	
	int groupIndex=(rIndex - bIndex);
	
	// Group index will be 1 for the 2nd group:
	groupIndex=(groupIndex==1 || groupIndex==-2);
	
	// Index in the group is therefore:
	int inGroupIndex = (groupIndex * rIndex) + (1-groupIndex) * bIndex;
	
	// Make group index either 0 or 3:
	groupIndex*=3;
	
	// Compute s:
	float shValues[6]={rgb,rgb.b,rgb.g,rgb.r};
	return shValues[ groupIndex + inGroupIndex ] - shValues[ groupIndex + ((inGroupIndex+2)%3) ];
	
}

float3 RGBtoHSY(float3 rgb){
	
	// 'Sort' them. Both of these go from 0-2. They're 2 when r/b are the biggest value:
	int rIndex=(rgb.r>=rgb.g) + (rgb.g>rgb.b);
	int bIndex=(rgb.b>=rgb.r) + (rgb.r>rgb.g);
	
	// 6 possible combinations in two groups. Biggest channel on the left:
	// Sector 0: rgb
	// Sector 1: gbr*
	// Sector 2: brg
	
	// Sector 3: bgr
	// Sector 4: grb
	// Sector 5: rbg
	
	// * Note that r==g==b ends up as gbr (h and s come out as 0)
	
	// The order of these was selected based being able to figure out the sector
	// from rIndex and bIndex. Look at how 'b' goes diagonally through the top set
	// and how 'r' goes diagonally through the bottom one.
	// Group 1: sector == bIndex
	// Group 2: sector == 3 + rIndex
	
	// However, we need to distinguish which group we're in.
	// (rIndex-bIndex) is 2 or -1 for the first group and 1 or -2 for the second.
	// They're unique, so this is what we use to figure out which group we're actually in.
	
	int groupIndex=(rIndex - bIndex);
	
	// Group index will be 1 for the 2nd group:
	groupIndex=(groupIndex==1 || groupIndex==-2);
	
	// Index in the group is therefore:
	int inGroupIndex = (groupIndex * rIndex) + (1-groupIndex) * bIndex;
	
	// Make group index either 0 or 3:
	groupIndex*=3;
	
	// Compute s:
	float shValues[6]={rgb,rgb.b,rgb.g,rgb.r};
	float s=shValues[ groupIndex + inGroupIndex ] - shValues[ groupIndex + ((inGroupIndex+2)%3) ];
	
	// Compute h:
	shValues[0]=rgb.g;
	shValues[1]=rgb.b;
	shValues[2]=rgb.r;
	// It's now {g,b,r,b,g,r};
	
	// Note that the max blocks s from being 0 (which happens when r==g==b)
	
	float h=(
		
		( shValues[ groupIndex + inGroupIndex ] - shValues[ groupIndex + ((inGroupIndex+1)%3) ] ) /
		max(s,0.0001)
		
	) + hsy_Offsets[groupIndex + inGroupIndex];
	
	// Get the Y value:
	float y = hsy_R * rgb.r + hsy_G * rgb.g + hsy_B * rgb.b;

	// Approximations errors can cause values to exceed bounds.
	
	return float3(
		clamp(h / 6, 0, 1),
		clamp(s, 0, 1),
		clamp(y, 0, 1)
	);
	
}

float luminanceHSY(float3 rgb){
	
	return hsy_R * rgb.r + hsy_G * rgb.g + hsy_B * rgb.b;
	
}
