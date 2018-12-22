using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a staircase.
	/// Input2 is step count. N cubes inside which the input graph is placed.
	/// Input1 is the step curve. Copied step count times into it's own little cube of space.
    /// </summary>
    public class Staircase:Std2InputNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x, double y){
			
			// How many stairs?
			double count=SourceModule2.GetValue(x);
			
			// Map x into our current step:
			x*=count;
			
			// Get the base step:
			int baseStep=(int)x;
			
			// Read the curve at x-baseStep (0-1 inside the current step):
			double curve=SourceModule1.GetValue(x-baseStep);
			
			// Gotta now offset and compress it back:
			return ( (double)baseStep + curve) / count;
			
		}
		
		public override double GetValue(double x){
			
			// How many stairs?
			double count=SourceModule2.GetValue(x);
			
			// Map x into our current step:
			x*=count;
			
			// Get the base step:
			int baseStep=(int)x;
			
			// Read the curve at x-baseStep (0-1 inside the current step):
			double curve=SourceModule1.GetValue(x-baseStep);
			
			// Gotta now offset and compress it back:
			return ( (double)baseStep + curve) / count;
			
		}
		
		public override int TypeID{
			get{
				return 102;
			}
		}
		
	}
	
}