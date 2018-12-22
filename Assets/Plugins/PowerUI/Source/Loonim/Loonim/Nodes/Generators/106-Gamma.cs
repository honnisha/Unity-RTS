using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a gamma correction curve.
	/// Input1 is gamma. Typical value is 2.2.
    /// </summary>
    public class Gamma:Std1InputNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			// Read gamma factor:
			double gamma=SourceModule.GetValue(x);
			
			// Result is just a division:
			return x/gamma;
			
		}
		
		public override double GetValue(double x){
			
			// Read gamma factor:
			double gamma=SourceModule.GetValue(x);
			
			// Result is just a division:
			return x/gamma;
			
		}
		
		public override int TypeID{
			get{
				return 106;
			}
		}
		
	}
	
}