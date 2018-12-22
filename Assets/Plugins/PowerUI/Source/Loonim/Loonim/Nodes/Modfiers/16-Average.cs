using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Similar to a 50% blend but this one ignores alpha.
	/// </summary>
	public class Average : Std2InputNode{
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule1.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=SourceModule2.GetColour(x,y);
			
			// Middle (ignoring alpha):
			col1.r=col1.r + ( (col2.r-col1.r) * 0.5f );
			col1.g=col1.g + ( (col2.g-col1.g) * 0.5f );
			col1.b=col1.b + ( (col2.b-col1.b) * 0.5f );
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double a=SourceModule1.GetWrapped(x, y,wrap);
			double b=SourceModule2.GetWrapped(x, y,wrap);
			
			return a + ( (b-a) * 0.5f );
			
		}
		
		public override double GetValue(double x, double y, double z){
			double a=SourceModule1.GetValue(x, y,z);
			double b=SourceModule2.GetValue(x, y,z);
			
			return a + ( (b-a) * 0.5f );
			
		}
		
		/// <summary>
		/// Returns the output of the two source modules added together.
		/// </summary>
		public override double GetValue(double x, double y){
			double a=SourceModule1.GetValue(x, y);
			double b=SourceModule2.GetValue(x, y);
			
			return a + ( (b-a) * 0.5f );
			
		}
		
		public override double GetValue(double t){
			
			double a=SourceModule1.GetValue(t);
			double b=SourceModule2.GetValue(t);
			
			return a + ( (b-a) * 0.5f );
			
		}
		
		public override int TypeID{
			get{
				return 16;
			}
		}
		
	}
	
}
