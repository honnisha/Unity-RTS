using System;
using UnityEngine;

namespace Loonim{
	
	public class Refraction : TextureNode{
		
		/// <summary>The heightmap.</summary>
		public TextureNode Height{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The ratio of one refractive index to another. e.g. 1.33.</summary>
		public TextureNode Ratio{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The surface normals, z component (up). "Normals" module may be useful.</summary>
		public TextureNode Normals{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public Refraction():base(4){}
		
		public Refraction(TextureNode src,TextureNode height,TextureNode ratio,TextureNode normals):base(4){
			SourceModule=src;
			Height=height;
			Ratio=ratio;
			Normals=normals;
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Normal is..
			Color normal=Normals.GetColour(x,y);
			double height=Height.GetValue(x,y);
			double ratio=Ratio.GetValue(x,y);
			
			// Next, dot it with a vertical vector. (0,0,1)
			// Simplifies to just normal.z (blue):
			float dot=normal.b;
			
			double k=1.0 - ratio * ratio * (1.0 - dot * dot);
			
			if(k<0.0){
				return SourceModule.GetColour(x,y);
			}
			
			double rdSqrt=-(ratio * dot + System.Math.Sqrt(k) );
			
			// Also apply height to it here:
			rdSqrt*=height;
			
			// Refraction vector is.. (just straight offset x/y):
			x+=rdSqrt * normal.r;
			y+=rdSqrt * normal.g;
			// float refractionVectorZ=ratio + rdSqrt * normal.b;
			
			return SourceModule.GetColour(x,y);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Col intensity:
			UnityEngine.Color col1=GetColour(x,y);
			return col1.r + col1.g + col1.b / 3.0;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Col intensity:
			UnityEngine.Color col1=GetColour(x,y);
			return col1.r + col1.g + col1.b / 3.0;
			
		}
		
		public override double GetValue(double x, double y){
			
			// Col intensity:
			UnityEngine.Color col1=GetColour(x,y);
			return col1.r + col1.g + col1.b / 3.0;
			
		}
		
		public override int TypeID{
			get{
				return 94;
			}
		}
		
	}
	
}