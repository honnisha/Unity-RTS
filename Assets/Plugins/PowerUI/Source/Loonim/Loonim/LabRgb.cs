//--------------------------------------
//       Loonim Image Generator
//    Partly derived from LibNoise
//    See License.txt for more info
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace Loonim{
	
	/// <summary>http://www.brucelindbloom.com/index.html?Equations.html.
	/// Note that this is remapped and is all in the 0-1 range.</summary>
	
	public static class LabRgb{
		
		public static string ToString(float l,float a,float b){
			
			// Re-map:
			return "lab( "+(l*100f)+" , "+((a-0.5f) * 127f)+" , "+((b-0.5f) * 127f)+" )";
			
		}
		
		public static void ToRgb(ref float l,ref float a,ref float b){
			
			// Lab range in 0-1
			// RGB range out 0-1
			
			double fY = System.Math.Pow((l + 0.16f) / 1.16f, 3.0);
			
			if (fY < 0.008856){
				fY = l / 9.033;
			}
			
			double Y = fY;
			
			if (fY > 0.008856){
				fY = System.Math.Pow(fY, 1.0/3.0);
			}else{
				fY = 7.787 * fY + 16.0/116.0;
			}

			double fX = (a-0.5f) * 2f + fY;     
			double X;
			
			if (fX > 0.206893){
				X = System.Math.Pow(fX, 3.0);
			}else{
				X = (fX - 16.0/116.0) / 7.787;
			}
			
			double fZ = fY - (b-0.5f) * 2f;
			double Z;
			
			if (fZ > 0.206893){
				Z = System.Math.Pow(fZ, 3.0);
			}else{
				Z = (fZ - 16.0/116.0) / 7.787;
			}
			
			X *= 0.950456f;
			Z *= 1.088754f;

			l =  (float)(3.240479f*X - 1.537150f*Y - 0.498535f*Z + 0.5f);
			a = (float)(-0.969256f*X + 1.875992f*Y + 0.041556f*Z + 0.5f);
			b =  (float)(0.055648f*X - 0.204043f*Y + 1.057311f*Z + 0.5f);
			
			// Clip:
			if(l<0f){
				l=0f;
			}else if(l>1f){
				l=1f;
			}
			
			if(a<0f){
				a=0f;
			}else if(a>1f){
				a=1f;
			}
			
			if(b<0f){
				b=0f;
			}else if(b>1f){
				b=1f;
			}
			
		}
		
		public static void ToLab(ref float r,ref float g,ref float b){
			
			// RGB in 0-1
			// Lab range out 0-1
			
			if ( r > 0.04045f ){
				r = (float)System.Math.Pow( (( r + 0.055f ) / 1.055f ), 2.4f );
			}else{
				r = r / 12.92f;
			}
			
			if( g > 0.04045f ){
				g = (float)System.Math.Pow( ( ( g + 0.055f ) / 1.055f ), 2.4f);
			}else{
				g = g / 12.92f;
			}
			
			if( b > 0.04045f ){
				b = (float)System.Math.Pow( ( ( b + 0.055f ) / 1.055f ), 2.4f);
			}else{
				b = b / 12.92f;
			}
			
			// rgb range still 0-1 here.
			
			//Observer. = 2°, Illuminant = D65
			float var_X = r * (0.4124f * 100f / 95.047f) + g * (0.3576f * 100f / 95.047f) + b * (0.1805f * 100f / 95.047f);
			float var_Y = r * 0.2126f + g * 0.7152f + b * 0.0722f; // * 100/100 cancels out.
			float var_Z = r * (0.0193f * 100f / 108.883f) + g * (0.1192f * 100f / 108.883f) + b * (0.9505f * 100f / 108.883f);
			
			// xyz range 0-1 here.
			
			if ( var_X > 0.008856f ){
				var_X = (float)System.Math.Pow(var_X , ( 1f/3f ) );
			}else{
				var_X = ( 7.787f * var_X ) + ( 16f / 116f );
			}
			
			if ( var_Y > 0.008856f ){
				var_Y = (float)System.Math.Pow(var_Y , ( 1f/3f ));
			}else{
				var_Y = ( 7.787f * var_Y ) + ( 16f / 116f );
			}
			
			if ( var_Z > 0.008856f ){
				var_Z = (float)System.Math.Pow(var_Z , ( 1f/3f ));
			}else{
				var_Z = ( 7.787f * var_Z ) + ( 16f / 116f );
			}
			
			// xyz range 0-1 here.
			
			// Output, remapped to 0-1:
			
			r = ( 1.16f * var_Y ) - 0.16f;
			g = ( 0.5f * ( var_X - var_Y ) ) + 0.5f;
			b = ( 0.5f * ( var_Y - var_Z ) ) + 0.5f;
			
		}
		
		public static float Luminance(float r,float g,float b){
			
			// RGB in 0-1
			// Lab range out 0-1
			
			if ( r > 0.04045f ){
				r = (float)System.Math.Pow( (( r + 0.055f ) / 1.055f ), 2.4f );
			}else{
				r = r / 12.92f;
			}
			
			if( g > 0.04045f ){
				g = (float)System.Math.Pow( ( ( g + 0.055f ) / 1.055f ), 2.4f);
			}else{
				g = g / 12.92f;
			}
			
			if( b > 0.04045f ){
				b = (float)System.Math.Pow( ( ( b + 0.055f ) / 1.055f ), 2.4f);
			}else{
				b = b / 12.92f;
			}
			
			// rgb range still 0-1 here.
			
			//Observer. = 2°, Illuminant = D65
			float var_Y = r * 0.2126f + g * 0.7152f + b * 0.0722f; // * 100/100 cancels out.
			
			// xyz range 0-1 here.
			
			if ( var_Y > 0.008856f ){
				var_Y = (float)System.Math.Pow(var_Y , ( 1f/3f ));
			}else{
				var_Y = ( 7.787f * var_Y ) + ( 16f / 116f );
			}
			
			// xyz range 0-1 here.
			
			// Output, remapped to 0-1:
			
			return ( 1.16f * var_Y ) - 0.16f;
			
		}
		
		public static float A(float r,float g,float b){
			
			// RGB in 0-1
			// Lab range out 0-1
			
			if ( r > 0.04045f ){
				r = (float)System.Math.Pow( (( r + 0.055f ) / 1.055f ), 2.4f );
			}else{
				r = r / 12.92f;
			}
			
			if( g > 0.04045f ){
				g = (float)System.Math.Pow( ( ( g + 0.055f ) / 1.055f ), 2.4f);
			}else{
				g = g / 12.92f;
			}
			
			if( b > 0.04045f ){
				b = (float)System.Math.Pow( ( ( b + 0.055f ) / 1.055f ), 2.4f);
			}else{
				b = b / 12.92f;
			}
			
			// rgb range still 0-1 here.
			
			//Observer. = 2°, Illuminant = D65
			float var_X = r * (0.4124f * 100f / 95.047f) + g * (0.3576f * 100f / 95.047f) + b * (0.1805f * 100f / 95.047f);
			float var_Y = r * 0.2126f + g * 0.7152f + b * 0.0722f; // * 100/100 cancels out.
			
			// xyz range 0-1 here.
			
			if ( var_X > 0.008856f ){
				var_X = (float)System.Math.Pow(var_X , ( 1f/3f ) );
			}else{
				var_X = ( 7.787f * var_X ) + ( 16f / 116f );
			}
			
			if ( var_Y > 0.008856f ){
				var_Y = (float)System.Math.Pow(var_Y , ( 1f/3f ));
			}else{
				var_Y = ( 7.787f * var_Y ) + ( 16f / 116f );
			}
			
			// xyz range 0-1 here.
			
			// Output, remapped to 0-1:
			
			return ( 0.5f * ( var_X - var_Y ) ) + 0.5f;
			
		}
		
		public static float B(float r,float g,float b){
			
			// RGB in 0-1
			// Lab range out 0-1
			
			if ( r > 0.04045f ){
				r = (float)System.Math.Pow( (( r + 0.055f ) / 1.055f ), 2.4f );
			}else{
				r = r / 12.92f;
			}
			
			if( g > 0.04045f ){
				g = (float)System.Math.Pow( ( ( g + 0.055f ) / 1.055f ), 2.4f);
			}else{
				g = g / 12.92f;
			}
			
			if( b > 0.04045f ){
				b = (float)System.Math.Pow( ( ( b + 0.055f ) / 1.055f ), 2.4f);
			}else{
				b = b / 12.92f;
			}
			
			// rgb range still 0-1 here.
			
			//Observer. = 2°, Illuminant = D65
			float var_Y = r * 0.2126f + g * 0.7152f + b * 0.0722f; // * 100/100 cancels out.
			float var_Z = r * (0.0193f * 100f / 108.883f) + g * (0.1192f * 100f / 108.883f) + b * (0.9505f * 100f / 108.883f);
			
			// xyz range 0-1 here.
			
			if ( var_Y > 0.008856f ){
				var_Y = (float)System.Math.Pow(var_Y , ( 1f/3f ));
			}else{
				var_Y = ( 7.787f * var_Y ) + ( 16f / 116f );
			}
			
			if ( var_Z > 0.008856f ){
				var_Z = (float)System.Math.Pow(var_Z , ( 1f/3f ));
			}else{
				var_Z = ( 7.787f * var_Z ) + ( 16f / 116f );
			}
			
			// xyz range 0-1 here.
			
			// Output, remapped to 0-1:
			
			return ( 0.5f * ( var_Y - var_Z ) ) + 0.5f;
			
		}
		
	}
	
}