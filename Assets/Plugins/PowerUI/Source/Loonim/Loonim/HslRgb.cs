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
		
	/// <summary>
	/// HSL to and from RGB. All remapped to 0-1.</summary>

	public static class HslRgb{
		
		public static string ToString(float h,float s,float l){
			
			// Re-map:
			return "hsl( "+(h*360f)+"deg , "+(s * 100f)+"% , "+(l * 100f)+"% )";
			
		}
		
		/// <summary>Gets RGB from HSL. Outputs right over the inputs. Inputs are all 0-1.</summary>
		public static void ToRgb(ref float h,ref float s,ref float l){

			float v;
			
			if(l <= 0.5f){
				v=(l * (1f + s));
			}else{
				v=(l + s - l * s);
			}
			
			if (v <= 0f){
				// Grey.
				h=l;
				s=l;
				return;
			}
			
			float m = l + l - v;
			
			float sv = (v - m ) / v;
			
			h *= 6f;
			
			int sextant = (int)h;
			
			float fract = h - sextant;
			
			float vsf = v * sv * fract;
			
			float mid1 = m + vsf;
			
			float mid2 = v - vsf;
			
			switch (sextant){
				
				case 0:
					
					h = v;
					s = mid1;
					l = m;
					
				break;
				case 1:
					
					h = mid2;
					s = v;
					l = m;
					
				break;
				case 2:
					
					h = m;
					s = v;
					l = mid1;
					
				break;
				case 3:
					
					h = m;
					s = mid2;
					l = v;
					
				break;
				case 4:
					
					h = mid1;
					s = m;
					l = v;
					
				break;
				case 5:

					h = v;
					s = m;
					l = mid2;
					
				break;
				
			}

		}
		
		/// <summary>Gets hsl from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToHsl(ref float r,ref float g,ref float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			float h;
			float s;
			float l = (m + v) / 2f;
			
			if (l<=0f){
				r=0f;
				g=0f;
				b=l; // hdr
				return;
			}
			
			float vm = v - m;
			s = vm;

			if (s>0f){
				
				// Note: it can only be zero otherwise.
				// This takes the small range and stretches it outwards to being in 0-1.
				
				if(l <= 0.5f){
					s/=(v + m);
				}else{
					s/=(2f - vm);
				}
				
			}else{
				r=0f;
				g=s;
				b=l;
				return;
			}

			float r2 = (v - r) / vm;

			float g2 = (v - g) / vm;

			float b2 = (v - b) / vm;

			if (r == v){
				
				if(g == m){
					h=5f + b2;
				}else{
					h=1f - g2;
				}
				
			}else if (g == v){
				
				if(b == m){
					h=1f + r2;
				}else{
					h=3f - b2;
				}
				
			}else if(r == m){
				
				h=3f + g2;
				
			}else{
				h=5f - r2;
			}
			
			// Output:
			r=h/6f;
			g=s;
			b=l;
			
		}
		
		/// <summary>Gets hue and luminosity from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToHueLum(ref float r,ref float g,float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			float h;
			float l = (m + v) / 2f;
			
			float vm = v - m;
			
			if(vm<=0f){
				r=0f;
				g=l;
				return;
			}

			float r2 = (v - r) / vm;

			float g2 = (v - g) / vm;

			float b2 = (v - b) / vm;

			if (r == v){
				
				if(g == m){
					h=5f + b2;
				}else{
					h=1f - g2;
				}
				
			}else if (g == v){
				
				if(b == m){
					h=1f + r2;
				}else{
					h=3f - b2;
				}
				
			}else if(r == m){
				
				h=3f + g2;
				
			}else{
				h=5f - r2;
			}
			
			// Output:
			r=h/6f;
			g=l;
			
		}
		
		/// <summary>Gets hue and saturation from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToHueSat(ref float r,ref float g,float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			float h;
			float s;
			
			float vm = v - m;
			s = vm;

			if (s>0f){
				
				float sum=(v+m);
				
				if(sum <= 1f){
					s/=sum;
				}else{
					s/=(2f - vm);
				}
				
			}else{
				r=0f;
				g=s;
				return;
			}

			float r2 = (v - r) / vm;

			float g2 = (v - g) / vm;

			float b2 = (v - b) / vm;

			if (r == v){
				
				if(g == m){
					h=5f + b2;
				}else{
					h=1f - g2;
				}
				
			}else if (g == v){
				
				if(b == m){
					h=1f + r2;
				}else{
					h=3f - b2;
				}
				
			}else if(r == m){
				
				h=3f + g2;
				
			}else{
				h=5f - r2;
			}
			
			// Output:
			r=h/6f;
			g=s;
			
		}
		
		/// <summary>Gets saturation and luminance from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToSatLum(ref float r,ref float g,float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			float s;
			float l = (m + v) / 2f;
			
			if (l<=0f){
				r=0f;
				g=0f;
				return;
			}
			
			float vm = v - m;
			s = vm;
			
			if (s>0f){
				
				// Note: it can only be zero otherwise.
				// This takes the small range and stretches it outwards to being in 0-1.
				
				if(l <= 0.5f){
					s/=(v + m);
				}else{
					s/=(2f - vm);
				}
				
			}
			
			r=s;
			g=l;
			
		}
		
		/// <summary>Gets hue from RGB. 0-1.</summary>
		public static float Hue(float r,float g,float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			float vm = v - m;
			
			if(vm==0f){
				return 0f;
			}
			
			float r2 = (v - r) / vm;

			float g2 = (v - g) / vm;

			float b2 = (v - b) / vm;
			
			if (r == v){
				
				if(g == m){
					return (5f + b2) / 6f;
				}
				
				return (1f - g2) / 6f;
				
			}else if (g == v){
				
				if(b == m){
					return (1f + r2) / 6f;
				}
				
				return (3f - b2) / 6f;
				
			}else if(r == m){
				
				return (3f + g2) / 6f;
				
			}
			
			return (5f - r2) / 6f;
			
		}
		
		/// <summary>Gets saturation from RGB. 0-1.</summary>
		public static float Saturation(float r,float g,float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			float vm = v - m;
			
			float s = vm;
			
			if (s>0f){
				
				// Note: it can only be zero otherwise.
				// This takes the small range and stretches it outwards to being in 0-1.
				
				float vpm=v+m;
				
				if(vpm <= 1f){
					s/=vpm;
				}else{
					s/=(2f - vpm);
				}
				
			}
			
			return s;
			
		}
		
		/// <summary>Gets luminance from RGB. 0-1.</summary>
		public static float Luminance(float r,float g,float b){
			
			float v=r; // Max of rgb
			float m=r; // Min of rgb
			
			if(g>r){
				v=g;
			}else{
				m=g;
			}
			
			if(b>v){
				v=b;
			}
			
			if(b<m){
				m=b;
			}
			
			return (m + v) / 2f;
			
		}
		
	}

}