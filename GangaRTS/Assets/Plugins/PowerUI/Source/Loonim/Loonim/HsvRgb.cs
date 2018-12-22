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
		
	// Note: see http://stackoverflow.com/a/24319877/2873896 for conversion to GPU.

	public static class HsvRgb{
		
		/// <summary>Gets RGB from HSV. Outputs right over the inputs.</summary>
		public static void ToRgb(ref float h,ref float s,ref float v){
			
			if(s <= 0f){
				h=v;
				s=v;
				return;
			}
			
			// 0-6 range:
			h*=6f;
			
			int sextant = (int)h;
			
			float ff = h - sextant;
			float p = v * (1f - s);
			float q = v * (1f - (s * ff));
			float t = v * (1f - (s * (1f - ff)));

			switch(sextant) {
				case 0:
					h = v;
					s = t;
					v = p;
				break;
				case 1:
					h = q;
					s = v;
					v = p;
				break;
				case 2:
					h = p;
					s = v;
					v = t;
				break;

				case 3:
					h = p;
					s = q;
					// v=v;
				break;
				case 4:
					h = t;
					s = p;
					// v=v;
				break;
				case 5:
					h = v;
					s = p;
					v = q;
				break;
			}
			
		}
		
		public static void ToHsv(ref float r,ref float g,ref float b){
			
			float max=r; // Max of rgb
			float min=r; // Min of rgb
			
			if(g>r){
				max=g;
			}else{
				min=g;
			}
			
			if(b>max){
				max=b;
			}
			
			if(b<min){
				min=b;
			}
			
			float h;
			float s=0f;
			float v = max; // v
			float delta = max - min;
			
			if(max>0f){
				s = (delta / max); // s
			}else{
				// if max is 0, then r = g = b = 0              
				// s = 0, h is undefined (0). Clear all:
				r=0f;
				g=0f;
				b=0f;
				return;
			}
			
			if( r >= max ){
				h = ( g - b ) / delta; // between yellow & magenta
			}else if( g >= max ){
				h = 2f + ( b - r ) / delta; // between cyan & yellow
			}else{
				h = 4f + ( r - g ) / delta;  // between magenta & cyan
			}
			
			h /=6f; // Map to 0-1
			
			if( h < 0f ){
				h += 1f;
			}
			
			// Output:
			r=h;
			g=s;
			b=v;
			
		}
		
		public static float Luminance(float r,float g,float b){
			
			float max=r; // Max of rgb
			
			if(g>r){
				max=g;
			}
			
			if(b>max){
				max=b;
			}
			
			return max;
			
		}
		
	}
	
}