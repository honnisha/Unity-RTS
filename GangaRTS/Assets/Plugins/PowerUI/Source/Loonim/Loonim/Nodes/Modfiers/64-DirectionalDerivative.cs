using System;
using UnityEngine;

namespace Loonim{

	/// <summary>Computes the directional derivative.</summary>
	public class DirectionalDerivative : Std2InputNode{
		
		public const float LineLength=0.01f;
		public const float HalfLine=LineLength/2f;
		
		/// <summary>Angle of the line in radians.</summary>
		public TextureNode AngleModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// 2 points (horizontal line), rotated clockwise by Angle.
			// Length of the line is a constant.
			// Sample those two points, then simply compute the difference.
			
			// Read angle:
			double angle=AngleModule.GetValue(x,y);
			float sA=(float)System.Math.Sin(angle);
			float cA=(float)System.Math.Cos(angle);
			
			// Base points (rotate first)
			float rightX=cA * HalfLine;
			float rightY=sA * HalfLine;
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x-rightX,y-rightY);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule.GetColour(x+rightX,y+rightY);
			
			// Difference divided by distance:
			col1.r=(col2.r-col1.r) / LineLength;
			col1.g=(col2.g-col1.g) / LineLength;
			col1.b=(col2.b-col1.b) / LineLength;
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Act like intensity:
			Color col=GetColour(x,y);
			
			return (col.r + col.g + col.b) / 3.0;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Act like intensity:
			Color col=GetColour(x,y);
			
			return (col.r + col.g + col.b) / 3.0;
			
		}
		
		public override double GetValue(double x, double y){
			
			// Act like intensity:
			Color col=GetColour(x,y);
			
			return (col.r + col.g + col.b) / 3.0;
			
		}
		
		public override int TypeID{
			get{
				return 64;
			}
		}
		
	}
	
}
