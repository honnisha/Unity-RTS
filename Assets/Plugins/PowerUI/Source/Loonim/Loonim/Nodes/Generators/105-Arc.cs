using System;


namespace Loonim{
	
    /// <summary>
    /// Generates an arc of given curvature.
	/// Input: Curvature. 1 means it looks like a circle quadrant, 0 means straight line.
	/// -1 means it looks like an inverse of a circle quadrant.
    /// </summary>
    public class Arc:Std1InputNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			// Get curvature:
			double curvature=SourceModule.GetValue(x);
			
			// Power:
			return System.Math.Pow(x,curvature+1f);
			
		}
		
		public override double GetValue(double x){
			
			// Get curvature:
			double curvature=SourceModule.GetValue(x);
			
			// Power:
			return System.Math.Pow(x,curvature+1f);
			
		}
		
		public override int TypeID{
			get{
				return 105;
			}
		}
		
	}
	
}