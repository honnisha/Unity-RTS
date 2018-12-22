using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a gaussian curve.
	/// Input1 is mu.
	/// Input2 is sigma squared (controls the width).
    /// </summary>
    public class Gaussian:Std2InputNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			// Read params:
			double mu=SourceModule1.GetValue(x);
			double sigma2=SourceModule2.GetValue(x);
			
			double tMu=x - mu;
			
			// Amplitude of 1.
			return System.Math.Exp( (- tMu * tMu) / (2.0 * sigma2) );
			
		}
		
		public override double GetValue(double x){
			
			// Read params:
			double mu=SourceModule1.GetValue(x);
			double sigma2=SourceModule2.GetValue(x);
			
			double tMu=x - mu;
			
			// Amplitude of 1.
			return System.Math.Exp( (- tMu * tMu) / (2.0 * sigma2) );
			
		}
		
		public override int TypeID{
			get{
				return 104;
			}
		}
		
	}
	
}