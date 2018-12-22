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
		

	public static class HsyRgb{
		
		// With thanks to http://stackoverflow.com/questions/12121393/
		
		private const float R=0.3f;
		private const float G=0.59f;
		private const float B=0.11f;
		
		/// <summary>Gets RGB from HSY. Outputs right over the inputs.</summary>
		public static void ToRgb(ref float h,ref float s,ref float y){
			
			h *= 360f;
			
			float r;
			float g;
			float b;

			float k; // Intermediate variable.

			if (h >= 0 && h < 60) {           // Sector 0: 0° - 60°
				k = s * h / 60;
				b = y - R * s - G * k;
				r = b + s;
				g = b + k;
			} else if (h >= 60 && h < 120) {  // Sector 1: 60° - 120°
				k = s * (h - 60) / 60;
				g = y + B * s + R * k;
				b = g - s;
				r = g - k;
			} else if (h >= 120 && h < 180) { // Sector 2: 120° - 180°
				k = s * (h - 120) / 60;
				r = y - G * s - B * k;
				g = r + s;
				b = r + k;
			} else if (h >= 180 && h < 240) { // Sector 3: 180° - 240°
				k = s * (h - 180) / 60;
				b = y + R * s + G * k;
				r = b - s;
				g = b - k;
			} else if (h >= 240 && h < 300) { // Sector 4: 240° - 300°
				k = s * (h - 240) / 60;
				g = y - B * s - R * k;
				b = g + s;
				r = g + k;
			} else {                          // Sector 5: 300° - 360°
				k = s * (h - 300) / 60;
				r = y + G * s + B * k;
				g = r - s;
				b = r - k;
			}
			
			// Approximations errors can cause values to exceed bounds.
			
			h = System.Math.Min(System.Math.Max(r, 0), 1);
			s = System.Math.Min(System.Math.Max(g, 0), 1);
			y = System.Math.Min(System.Math.Max(b, 0), 1);
			
		}
		
		/// <summary>Gets hsy from RGB. Outputted directly over the inputs.</summary>
		public static void ToHsy(ref float r,ref float g,ref float b){
			
			float h;
			float s;
			float y;
			
			// For saturation equals to 0 any value of hue are valid.
			// In this case we choose 0 as a default value.

			if (r == g && g == b) {            // Limit case.
				s = 0; 
				h = 0; 
			} else if ((r >= g) && (g >= b)) { // Sector 0: 0° - 60°
				s = r - b;
				h = 60 * (g - b) / s;
			} else if ((g > r) && (r >= b)) {  // Sector 1: 60° - 120°
				s = g - b;
				h = 60 * (g - r) / s  + 60;
			} else if ((g >= b) && (b > r)) {  // Sector 2: 120° - 180°
				s = g - r;
				h = 60 * (b - r) / s + 120;
			} else if ((b > g) && (g > r)) {   // Sector 3: 180° - 240°
				s = b - r;
				h = 60 * (b - g) / s + 180;
			} else if ((b > r) && (r >= g)) {  // Sector 4: 240° - 300°
				s = b - g;
				h = 60 * (r - g) / s + 240;
			} else {                           // Sector 5: 300° - 360°
				s = r - g;
				h = 60 * (r - b) / s + 300;
			}

			y = Luminance(r,g,b);

			// Approximations errors can cause values to exceed bounds.
			
			r = System.Math.Min(System.Math.Max(h / 360, 0), 1);
			g = System.Math.Min(System.Math.Max(s, 0), 1);
			b = System.Math.Min(System.Math.Max(y, 0), 1);
		}
		
		/// <summary>Gets hue and luminosity from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToHueLum(ref float r,ref float g,float b){
			ToHsy(ref r,ref g,ref b);
			g=b;
		}
		
		/// <summary>Gets hue and saturation from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToHueSat(ref float r,ref float g,float b){
			
			float h;
			float s;
			
			// For saturation equals to 0 any value of hue are valid.
			// In this case we choose 0 as a default value.

			if (r == g && g == b) {            // Limit case.
				s = 0; 
				h = 0; 
			} else if ((r >= g) && (g >= b)) { // Sector 0: 0° - 60°
				s = r - b;
				h = 60 * (g - b) / s;
			} else if ((g > r) && (r >= b)) {  // Sector 1: 60° - 120°
				s = g - b;
				h = 60 * (g - r) / s  + 60;
			} else if ((g >= b) && (b > r)) {  // Sector 2: 120° - 180°
				s = g - r;
				h = 60 * (b - r) / s + 120;
			} else if ((b > g) && (g > r)) {   // Sector 3: 180° - 240°
				s = b - r;
				h = 60 * (b - g) / s + 180;
			} else if ((b > r) && (r >= g)) {  // Sector 4: 240° - 300°
				s = b - g;
				h = 60 * (r - g) / s + 240;
			} else {                           // Sector 5: 300° - 360°
				s = r - g;
				h = 60 * (r - b) / s + 300;
			}
			
			// Approximations errors can cause values to exceed bounds.
			
			r = System.Math.Min(System.Math.Max(h / 360, 0), 1);
			g = System.Math.Min(System.Math.Max(s, 0), 1);
		}
		
		/// <summary>Gets hue and saturation from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static void ToSatLum(ref float r,ref float g,float b){
			
			float s;
			float y;
			
			// For saturation equals to 0 any value of hue are valid.
			// In this case we choose 0 as a default value.

			if (r == g && g == b) {            // Limit case.
				s = 0;
			} else if ((r >= g) && (g >= b)) { // Sector 0: 0° - 60°
				s = r - b;
			} else if ((g > r) && (r >= b)) {  // Sector 1: 60° - 120°
				s = g - b;
			} else if ((g >= b) && (b > r)) {  // Sector 2: 120° - 180°
				s = g - r;
			} else if ((b > g) && (g > r)) {   // Sector 3: 180° - 240°
				s = b - r;
			} else if ((b > r) && (r >= g)) {  // Sector 4: 240° - 300°
				s = b - g;
			} else {                           // Sector 5: 300° - 360°
				s = r - g;
			}

			y = Luminance(r,g,b);

			// Approximations errors can cause values to exceed bounds.
			r = System.Math.Min(System.Math.Max(s, 0), 1);
			g = System.Math.Min(System.Math.Max(y, 0), 1);
		}
		
		/// <summary>Gets hue and luminosity from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static float Hue(float r,float g,float b){
			float junked=g;
			ToHueSat(ref r,ref junked,b);
			return r;
		}
		
		/// <summary>Gets hue and saturation from RGB. Outputted directly over the inputs. 0-1.</summary>
		public static float Saturation(float r,float g,float b){
			
			float s;
			
			// For saturation equals to 0 any value of hue are valid.
			// In this case we choose 0 as a default value.

			if (r == g && g == b) {            // Limit case.
				s = 0;
			} else if ((r >= g) && (g >= b)) { // Sector 0: 0° - 60°
				s = r - b;
			} else if ((g > r) && (r >= b)) {  // Sector 1: 60° - 120°
				s = g - b;
			} else if ((g >= b) && (b > r)) {  // Sector 2: 120° - 180°
				s = g - r;
			} else if ((b > g) && (g > r)) {   // Sector 3: 180° - 240°
				s = b - r;
			} else if ((b > r) && (r >= g)) {  // Sector 4: 240° - 300°
				s = b - g;
			} else {                           // Sector 5: 300° - 360°
				s = r - g;
			}
			
			return System.Math.Min(System.Math.Max(s, 0), 1);
			
		}
		
		/// <summary>Computes the Y luminance for the given RGB colour.</summary>
		public static float Luminance(float r,float g,float b){
			
			return R * r + G * g + B * b;
			
		}
		
	}
	
}