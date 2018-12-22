using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a bias curve.
	/// Input1 is bias factor.
    /// </summary>
    public class BiasCurve:Std1InputNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			// Algorithm developed by Christophe Schlick,
			// "Fast alternative to Perlin's bias and gain functions" GPU Gems (1994).
			// Implementation based off http://www.gmlscripts.com/script/bias. 
			// See GMLscripts.com/license
			
			// Read factor:
			double bias=SourceModule.GetValue(x);
			
			return x / ((1.0 / bias - 2.0) * (1.0 - x) + 1.0);
			
		}
		
		public override double GetValue(double x){
			
			// Algorithm developed by Christophe Schlick,
			// "Fast alternative to Perlin's bias and gain functions" GPU Gems (1994).
			// Implementation based off http://www.gmlscripts.com/script/bias. 
			// See GMLscripts.com/license
			
			// Read factor:
			double bias=SourceModule.GetValue(x);
			
			return x / ((1.0 / bias - 2.0) * (1.0 - x) + 1.0);
			
		}
		
		public override int TypeID{
			get{
				return 107;
			}
		}
		
	}
	
}