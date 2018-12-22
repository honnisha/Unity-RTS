using System;
using UnityEngine;

namespace Loonim
{
	
	/// <summary>
	/// Generates a noisy gradient. Essentially adds a random HSL colour to the base colour.
	/// The range of available colours is given with h,s and l. Base colour is source.
	/// The randomness factors are given with the 3 noise details.
	/// </summary>
	
    public class NoisyGradient: TextureNode{
		
		/// <summary>Perlin for hue.</summary>
		private Perlin PerlinH;
		/// <summary>Perlin for saturation.</summary>
		private Perlin PerlinS;
		/// <summary>Perlin for luminance.</summary>
		private Perlin PerlinL;
		/// <summary>Hue range of values.</summary>
		public TextureNode RangeHModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>Saturation range of values.</summary>
		public TextureNode RangeSModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>Luminance range of values.</summary>
		public TextureNode RangeLModule{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		
		public int Seed;
		
		internal override int OutputDimensions{
			get{
				// 2D.
				return 2;
			}
		}
		
        public TextureNode OctaveCount{ // 0 is 1 octave, and 1 is 11 octaves.
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
        public TextureNode LacunarityModule{
			get{
				return Sources[5];
			}
			set{
				Sources[5]=value;
			}
		}
		
        public TextureNode PersistenceModule{ // Roughness / 100. Low = smooth.
			get{
				return Sources[6];
			}
			set{
				Sources[6]=value;
			}
		}
		
		public NoiseQuality NoiseQuality;
		
		
        public NoisyGradient():base(7){
			
		}
		
		public void Setup(){
			
			// Create basic perlin:
			if(PerlinH==null){
				PerlinH=new Perlin();
			}
			
			PerlinH.Seed=Seed-124;
			PerlinH.OctaveCount=OctaveCount;
			PerlinH.NoiseQuality=NoiseQuality;
			PerlinH.Persistence=PersistenceModule;
			PerlinH.Lacunarity=LacunarityModule;
			
			// Create basic perlin:
			if(PerlinS==null){
				PerlinS=new Perlin();
			}
			
			PerlinS.Seed=Seed;
			PerlinS.OctaveCount=OctaveCount;
			PerlinS.NoiseQuality=NoiseQuality;
			PerlinS.Persistence=PersistenceModule;
			PerlinS.Lacunarity=LacunarityModule;
			
			// Create basic perlin:
			if(PerlinL==null){
				PerlinL=new Perlin();
			}
			
			PerlinL.Seed=Seed+302;
			PerlinL.OctaveCount=OctaveCount;
			PerlinL.NoiseQuality=NoiseQuality;
			PerlinL.Persistence=PersistenceModule;
			PerlinL.Lacunarity=LacunarityModule;
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Base colour is..
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Map to HSL:
			float r=col1.r;
			float g=col1.g;
			float b=col1.b;
			HslRgb.ToHsl(ref r,ref g,ref b);
			
			// RGB is now HSL.
			
			
			// Get a H range value:
			double hPoint=PerlinH.GetValue(x,y);
			
			// Get an S range value:
			double sPoint=PerlinS.GetValue(x,y);
			
			// Get an L range value:
			double lPoint=PerlinL.GetValue(x,y);
			
			// Read ranges:
			double halfHRange=RangeHModule.GetValue(x,y) * 0.5;
			double halfSRange=RangeSModule.GetValue(x,y) * 0.5;
			double halfLRange=RangeLModule.GetValue(x,y) * 0.5;
			
			// Slightly awkward naming here; this is all currently in HSL.
			// Map the hsl into the available range:
			r=r+(float)(halfHRange * hPoint);
			g=g+(float)(halfSRange * sPoint);
			b=b+(float)(halfLRange * lPoint);
			
			// Map to RGB:
			HslRgb.ToRgb(ref r,ref g,ref b);
			
			return new UnityEngine.Color(r,g,b,col1.a);
			
		}
		
        public override double GetValue(double x, double y){
			UnityEngine.Color col=GetColour(x,y);
			return (col.r + col.g + col.b)/3.0;
        }
		
		public override int TypeID{
			get{
				return 96;
			}
		}
		
    }
	
}
