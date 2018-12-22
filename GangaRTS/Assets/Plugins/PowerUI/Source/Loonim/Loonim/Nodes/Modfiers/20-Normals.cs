using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Computes the normals from a height map. Uses the greyscale brightness.
	/// </summary>
	
	public class Normals : Std2InputNode{
		
		/// <summary>The strength of the normals. This is usually 2.</summary>
		public TextureNode Strength{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>Maps 0 to 1 x into 0 to (Width-1).</summary>
		public double MapX;
		/// <summary>Maps 0 to 1 y into 0 to (Height-1).</summary>
		public double MapY;
		
		
		public Normals(){}
		
		public Normals(TextureNode sourceModule,TextureNode strength){
			SourceModule=sourceModule;
			Strength=strength;
		}
		
		public override void Prepare(DrawInfo info){
			base.Prepare(info);
			MapX=info.DeltaX;
			MapY=info.DeltaY;
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// 1/pStr:
			float str=(float)SourceModule2.GetValue(x,y);
			
			double mapX=MapX;
			double mapY=MapY;
			
			// Read the surrounding heights:
			double tl = SourceModule1.GetValue(x - mapX,y + mapY);
			double t = SourceModule1.GetValue(x,y + mapY);
			double tr = SourceModule1.GetValue(x + mapX,y + mapY);
			double r = SourceModule1.GetValue(x + mapX,y);
			double br = SourceModule1.GetValue(x + mapX,y - mapY);
			double b = SourceModule1.GetValue(x,y - mapY);
			double bl = SourceModule1.GetValue(x - mapX,y - mapY);
			double l = SourceModule1.GetValue(x - mapX,y);
		
			// sobel filter
			float dX = (float)( (tr + 2.0 * r + br) - (tl + 2.0 * l + bl) );
			float dY = (float)( (bl + 2.0 * b + br) - (tl + 2.0 * t + tr) );
			float dZ = str;
			
			// Normalise:
			float length=(float)System.Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
			
			dX/=length;
			dY/=length;
			dZ/=length;
			
			return new Color(dX,dY,dZ,1f);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			return GetValue(x,y);
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			return GetValue(x,y);
			
		}
		
		public override double GetValue(double x, double y){
			
			// pStr:
			double str=SourceModule2.GetValue(x,y);
			
			double mapX=MapX;
			double mapY=MapY;
			
			// Read the surrounding heights:
			double tl = SourceModule1.GetValue(x - mapX,y + mapY);
			double t = SourceModule1.GetValue(x,y + mapY);
			double tr = SourceModule1.GetValue(x + mapX,y + mapY);
			double r = SourceModule1.GetValue(x + mapX,y);
			double br = SourceModule1.GetValue(x + mapX,y - mapY);
			double b = SourceModule1.GetValue(x,y - mapY);
			double bl = SourceModule1.GetValue(x - mapX,y - mapY);
			double l = SourceModule1.GetValue(x - mapX,y);
		
			// sobel filter
			double dX = ( (tr + 2.0 * r + br) - (tl + 2.0 * l + bl) );
			double dY = ( (bl + 2.0 * b + br) - (tl + 2.0 * t + tr) );
			double dZ = 1.0 / str;
			
			// Normalise:
			double length=System.Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
			
			return dZ / length;
			
		}
		
		public override int TypeID{
			get{
				return 20;
			}
		}
		
	}
}
