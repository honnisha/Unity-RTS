using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a gain curve.
	/// Input1 is the gain factor. If 0, it comes out as a straight line. If 1, it looks more like half of a gaussian function.
    /// </summary>
	
    public class GainCurve : Std1InputNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			// Algorithm developed by Christophe Schlick,
			// "Fast alternative to Perlin's bias and gain functions" GPU Gems (1994).
			// Implementation based off http://www.gmlscripts.com/script/gain. 
			// See GMLscripts.com/license
			
			// Read factor:
			double gain=SourceModule.GetValue(x);
			
			double value = (1.0 / gain - 2.0) * (1.0 - 2.0 * x);
			
			if (x < 0.5){
				return x / (value + 1.0);
			}
			
			return (value - x) / (value - 1.0);
			
		}
		
		public override double GetValue(double x){
			
			// Algorithm developed by Christophe Schlick,
			// "Fast alternative to Perlin's bias and gain functions" GPU Gems (1994).
			// Implementation based off http://www.gmlscripts.com/script/gain. 
			// See GMLscripts.com/license
			
			// Read factor:
			double gain=SourceModule.GetValue(x);
			
			double value = (1.0 / gain - 2.0) * (1.0 - 2.0 * x);
			
			if (x < 0.5){
				return x / (value + 1.0);
			}
			
			return (value - x) / (value - 1.0);
			
		}
		
		public override int TypeID{
			get{
				return 103;
			}
		}
		
	}
	
}