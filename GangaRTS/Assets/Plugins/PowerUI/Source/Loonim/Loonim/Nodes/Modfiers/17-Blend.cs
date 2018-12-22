using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Module that blends the output of two source modules using the output
	/// of an weight module as the blending weight.
	/// </summary>
	public class Blend : TextureNode{
		
		/// <summary>The last blend mode value.</summary>
		private int LastBlendMode_;
		
		/// <summary>
		/// The module from which to retrieve noise to be used as the blending weight.
		/// </summary>
		public TextureNode WeightModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>
		/// The module which defines the blending mode. Note that this has to be a 
		/// constant value across the whole image; anything else will be ignored.
		/// </summary>
		public TextureNode ModeModule{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>
		/// The blending mode to use.
		/// </summary>
		public BlendingMode Mode{
			get{
				return (BlendingMode)LastBlendMode_;
			}
		}
		
		/// <summary>By default, materials are named Loonim/Texture_node_id, however some nodes have "sub-materials"
		/// where they essentially have a bunch of different shaders. An example is the Blend node.</summary>
		public override int SubMaterialID{
			get{
				return LastBlendMode_;
			}
		}
		
		/// <summary>
		/// Initialises a new instance of the Blend class.
		/// </summary>
		public Blend():base(4){}
		
		public Blend(TextureNode src1,TextureNode src2,TextureNode weight,TextureNode mode):base(4){
			SourceModule1=src1;
			SourceModule2=src2;
			WeightModule=weight;
			ModeModule=mode;
		}
		
		public override void Draw(DrawInfo info){
			
			// Always pull the latest mode, checking if it's changed:
			Property pv=ModeModule as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				pv.UpdateIfChanged(info);
			
			}
			
			int blendMode=(int)(ModeModule.GetValue(0,0));
			
			if(blendMode!=LastBlendMode_){
				
				// Change!
				LastBlendMode_=blendMode;
				
				// Allocate shader now:
				SubMaterialChanged();
				
			}
			
			// Draw now:
			base.Draw(info);
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule1.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule2.GetColour(x,y);
			
			// Blend factor is..
			float dstA=col1.a;
			float srcA=(float)WeightModule.GetColour(x,y).r * col2.a;
			
			switch(Mode){
				case BlendingMode.Normal:
				
					// Do nothing here - we just alpha blend.
				
				break;
				case BlendingMode.Darken:
					
					// Pick the smallest of the two and set col2:
					if(col1.r<col2.r){
						col2.r=col1.r;
					}
					
					if(col1.g<col2.g){
						col2.g=col1.g;
					}
					
					if(col1.b<col2.b){
						col2.b=col1.b;
					}
					
				break;
				case BlendingMode.Multiply:
					
					col2.r*=col1.r;
					col2.g*=col1.g;
					col2.b*=col1.b;
					
				break;
				case BlendingMode.ColourBurn:
					
					col2.r=1f - ((1f - col1.r) / col2.r);
					col2.g=1f - ((1f - col1.g) / col2.g);
					col2.b=1f - ((1f - col1.b) / col2.b);
					
				break;
				case BlendingMode.LinearBurn:
				
					col2.r+=col1.r-1f;
					col2.g+=col1.g-1f;
					col2.b+=col1.b-1f;
					
				break;
				case BlendingMode.Lighten:
					
					// Pick the biggest of the two and set col2:
					if(col1.r>col2.r){
						col2.r=col1.r;
					}
					
					if(col1.g>col2.g){
						col2.g=col1.g;
					}
					
					if(col1.b>col2.b){
						col2.b=col1.b;
					}
					
				break;
				case BlendingMode.Screen:
					
					col2.r = 1f - (1f - col1.r) * (1f - col2.r);
					col2.g = 1f - (1f - col1.g) * (1f - col2.g);
					col2.b = 1f - (1f - col1.b) * (1f - col2.b);
					
				break;
				case BlendingMode.ColourDodge:
					
					// Bottom layer / (inverted Top layer)
					if(col2.r!=1f){
						col2.r=col1.r / (1f - col2.r);
					}
					
					if(col2.g!=1f){
						col2.g=col1.g / (1f - col2.g);
					}
					
					if(col2.b!=1f){
						col2.b=col1.b / (1f - col2.b);
					}
					
				break;
				case BlendingMode.LinearDodge:
					
					// Sum the two layers:
					col2.r+=col1.r;
					col2.g+=col1.g;
					col2.b+=col1.b;
					
				break;
				case BlendingMode.Overlay:
					
					if(col1.r < 0.5f){
						col2.r = 2f * col1.r * col2.r;
					}else{
						col2.r = 1f - 2f * (1f - col1.r) * (1f - col2.r);
					}
					
					if(col1.g < 0.5f){
						col2.g = 2f * col1.g * col2.g;
					}else{
						col2.g = 1f - 2f * (1f - col1.g) * (1f - col2.g);
					}
					
					if(col1.b < 0.5f){
						col2.b = 2f * col1.b * col2.b;
					}else{
						col2.b = 1f - 2f * (1f - col1.b) * (1f - col2.b);
					}
					
				break;
				case BlendingMode.SoftLight:
					
					// W3C soft light blend here.
					float g;
					float a;
					float b;
					
					a=col1.r;
					b=col2.r;
					
					if(b <= 0.5f){
						
						col2.r = a - (1f - 2f * b) * a * (1f - a);
						
					}else{
						
						if(a<=0.25f){
							g=((16f * a - 12f) * a +4f) * a;
						}else{
							g=(float)System.Math.Sqrt(a);
						}
						
						col2.r = a + (2f * b - 1f) * (g - a);
						
					}
					
					a=col1.g;
					b=col2.g;
					
					if(b <= 0.5f){
						
						col2.g = a - (1f - 2f * b) * a * (1f - a);
						
					}else{
						
						if(a<=0.25f){
							g=((16f * a - 12f) * a +4f) * a;
						}else{
							g=(float)System.Math.Sqrt(a);
						}
						
						col2.g = a + (2f * b - 1f) * (g - a);
						
					}
					
					a=col1.b;
					b=col2.b;
					
					if(b <= 0.5f){
						
						col2.b = a - (1f - 2f * b) * a * (1f - a);
						
					}else{
						
						if(a<=0.25f){
							g=((16f * a - 12f) * a +4f) * a;
						}else{
							g=(float)System.Math.Sqrt(a);
						}
						
						col2.b = a + (2f * b - 1f) * (g - a);
						
					}
					
				break;
				case BlendingMode.HardLight:
					
					if(col2.r < 0.5f){
						col2.r = 2f * col2.r * col1.r;
					}else{
						col2.r = 1f - 2f * (1f - col2.r) * (1f - col1.r);
					}
					
					if(col2.g < 0.5f){
						col2.g = 2f * col2.g * col1.g;
					}else{
						col2.g = 1f - 2f * (1f - col2.g) * (1f - col1.g);
					}
					
					if(col2.b < 0.5f){
						col2.b = 2f * col2.b * col1.b;
					}else{
						col2.b = 1f - 2f * (1f - col2.b) * (1f - col1.b);
					}
					
				break;
				case BlendingMode.VividLight:
				
					if(col2.r < 0.5f){
						// Colour Burn
						col2.r=1f - ((1f - col1.r) / col2.r);
						
					}else{
						// Colour Dodge
						col2.r=col1.r / (1f - col2.r);
						
					}
					
					if(col2.g < 0.5f){
						// Colour Burn
						col2.g=1f - ((1f - col1.g) / col2.g);
						
					}else{
						// Colour Dodge
						col2.g=col1.g / (1f - col2.g);
						
					}
					
					if(col2.b < 0.5f){
						// Colour Burn
						col2.b=1f - ((1f - col1.b) / col2.b);
						
					}else{
						// Colour Dodge
						col2.b=col1.b / (1f - col2.b);
						
					}
					
				break;
				case BlendingMode.LinearLight:
				
					if(col2.r < 0.5f){
						// Linear Burn
						col2.r+=col1.r-1f;
					}else{
						// Linear Dodge
						col2.r+=col1.r;
					}
					
					if(col2.g < 0.5f){
						// Linear Burn
						col2.g+=col1.g-1f;
					}else{
						// Linear Dodge
						col2.g+=col1.g;
					}
				
					if(col2.b < 0.5f){
						// Linear Burn
						col2.b+=col1.b-1f;
					}else{
						// Linear Dodge
						col2.b+=col1.b;
					}
				
				break;
				case BlendingMode.Hue:
					
					// Top layers hue and the sat/lum from bottom layer.
					
					float s=col1.r;
					float l=col1.g;
					HsyRgb.ToSatLum(ref s,ref l,col1.b);
					
					// Top hue:
					float h=HsyRgb.Hue(col2.r,col2.g,col2.b);
					
					// Recombine to RGB.
					HsyRgb.ToRgb(ref h,ref s,ref l);
					
					// They're now RGB.
					col2.r=h;
					col2.g=s;
					col2.b=l;
					
				break;
				case BlendingMode.Saturation:
				
					// Top layers saturation and the hue/lum from bottom layer.
					
					h=col1.r;
					l=col1.g;
					HsyRgb.ToHueLum(ref h,ref l,col1.b);
					
					// Top saturation:
					s=HsyRgb.Saturation(col2.r,col2.g,col2.b);
					
					// Recombine to RGB.
					HsyRgb.ToRgb(ref h,ref s,ref l);
					
					// They're now RGB.
					col2.r=h;
					col2.g=s;
					col2.b=l;
					
				break;
				case BlendingMode.Colour:
				
					// Top layers hue/sat and the luminosity from bottom layer.
					
					h=col2.r;
					s=col2.g;
					HsyRgb.ToHueSat(ref h,ref s,col2.b);
					
					// Bottom luminosity:
					l=HsyRgb.Luminance(col1.r,col1.g,col1.b);
					
					// Recombine to RGB.
					HsyRgb.ToRgb(ref h,ref s,ref l);
					
					// They're now RGB.
					col2.r=h;
					col2.g=s;
					col2.b=l;
					
				break;
				case BlendingMode.Luminosity:
					
					// Top layers luminosity and the hue/sat from bottom layer.
					
					h=col1.r;
					s=col1.g;
					HsyRgb.ToHueSat(ref h,ref s,col1.b);
					
					// Top luminosity:
					l=HsyRgb.Luminance(col2.r,col2.g,col2.b);
					
					// Recombine to RGB.
					HsyRgb.ToRgb(ref h,ref s,ref l);
					
					// They're now RGB.
					col2.r=h;
					col2.g=s;
					col2.b=l;
					
				break;
				case BlendingMode.Divide:
					
					// Bottom layer / Top layer
					if(col2.r!=0f){
						col2.r=col1.r / col2.r;
					}
					
					if(col2.g!=0f){
						col2.g=col1.g / col2.g;
					}
					
					if(col2.b!=0f){
						col2.b=col1.b / col2.b;
					}
					
				break;
				case BlendingMode.Subtract:
				
					// Subtract:
					col2.r=(col1.r-col2.r);
					col2.g=(col1.g-col2.g);
					col2.b=(col1.b-col2.b);
					
				break;
				case BlendingMode.Difference:
				case BlendingMode.Exclusion:
				
					// Subtract:
					g=(col1.r - col2.r);
					
					if(g<0f){
						col2.r=-g;
					}else{
						col2.r=g;
					}
					
					g=(col1.g - col2.g);
					
					if(g<0f){
						col2.g=-g;
					}else{
						col2.g=g;
					}
					
					g=(col1.b - col2.b);
					
					if(g<0f){
						col2.b=-g;
					}else{
						col2.b=g;
					}
					
				break;
				case BlendingMode.HardMix:
					
					// Linear light first:
					
					if(col2.r < 0.5f){
						// Linear Burn
						col2.r+=col1.r-1f;
					}else{
						// Linear Dodge
						col2.r+=col1.r;
					}
					
					if(col2.g < 0.5f){
						// Linear Burn
						col2.g+=col1.g-1f;
					}else{
						// Linear Dodge
						col2.g+=col1.g;
					}
				
					if(col2.b < 0.5f){
						// Linear Burn
						col2.b+=col1.b-1f;
					}else{
						// Linear Dodge
						col2.b+=col1.b;
					}
					
					// Threshold:
					if(col2.r<0.5f){
						col2.r=0f;
					}else{
						col2.r=1f;
					}
					
					if(col2.g<0.5f){
						col2.g=0f;
					}else{
						col2.g=1f;
					}
					
					if(col2.b<0.5f){
						col2.b=0f;
					}else{
						col2.b=1f;
					}
					
				break;
				case BlendingMode.PinLight:
					
					if(col2.r<0.5f){
						
						// Darken
						if(col1.r<col2.r){
							col2.r=col1.r;
						}
						
					}else{
						
						// Lighten
						if(col1.r>col2.r){
							col2.r=col1.r;
						}
						
					}
				
					if(col2.g<0.5f){
						
						// Darken
						if(col1.g<col2.g){
							col2.g=col1.g;
						}
						
					}else{
						
						// Lighten
						if(col1.g>col2.g){
							col2.g=col1.g;
						}
						
					}
				
					if(col2.b<0.5f){
						
						// Darken
						if(col1.b<col2.b){
							col2.b=col1.b;
						}
						
					}else{
						
						// Lighten
						if(col1.b>col2.b){
							col2.b=col1.b;
						}
						
					}
				
				break;
				case BlendingMode.LighterColor:
					
					// Like lighten but applies to the composite channel.
					float total1=col1.r + col1.g + col1.b;
					float total2=col2.r + col2.g + col2.b;
					
					if(total1>total2){
						
						// Blend will do nothing:
						col1.a=srcA + dstA * (1f - srcA);
						return col1;
						
					}
					
				break;
				case BlendingMode.DarkerColor:
				
					// Like darken but applies to the composite channel.
					total1=col1.r + col1.g + col1.b;
					total2=col2.r + col2.g + col2.b;
					
					if(total1<total2){
						
						// Blend will do nothing:
						col1.a=srcA + dstA * (1f - srcA);
						return col1;
						
					}
					
				break;

			}
			
			// Time to alpha blend!
			
			if(srcA==1f){
				// Just foreground:
				
				col2.a=1f;
				return col2;
				
			}
			
			float dstAinvSrc=dstA * (1f - srcA);
			float outA=srcA + dstAinvSrc;
			
			if(outA==0f){
				col1.r=0f;
				col1.g=0f;
				col1.b=0f;
			}else{
				col1.r = ( (col2.r * srcA) + (col1.r * dstAinvSrc) ) / outA;
				col1.g = ( (col2.g * srcA) + (col1.g * dstAinvSrc) ) / outA;
				col1.b = ( (col2.b * srcA) + (col1.b * dstAinvSrc) ) / outA;
			}
			
			col1.a=outA;
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return Loonim.Math.LinearInterpolate(SourceModule1.GetWrapped(x,y,wrap),
				SourceModule2.GetWrapped(x,y,wrap),
				(WeightModule.GetWrapped(x,y,wrap) + 1.0) / 2.0);
		}
		
		public override double GetValue(double x, double y, double z){
			return Loonim.Math.LinearInterpolate(SourceModule1.GetValue(x, y,z),
				SourceModule2.GetValue(x, y,z),
				(WeightModule.GetValue(x, y,z) + 1.0) / 2.0);
		}
		
		/// <summary>
		/// Returns the result of blending the output of the two source modules using the 
		/// output of the weight module as the blending weight.
		/// </summary>
		public override double GetValue(double x, double y){
			return Loonim.Math.LinearInterpolate(SourceModule1.GetValue(x, y),
				SourceModule2.GetValue(x, y),
				(WeightModule.GetValue(x, y) + 1.0) / 2.0);
		}
		
		public override double GetValue(double t){
			
			// Read the values:
			double a=SourceModule1.GetValue(t);
			double b=SourceModule2.GetValue(t);
			
			// Blend factor:
			double blend=WeightModule.GetValue(t);
			
			// Blend now!
			return a+((b-a) * blend);
			
		}
		
		public override int TypeID{
			get{
				return 17;
			}
		}
		
	}
}
