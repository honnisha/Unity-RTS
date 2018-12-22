using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a sine wave.
    /// </summary>
    public class SineWave:TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return ( ( System.Math.Sin(x) + 1) * 0.5);
		}
		
		public override double GetValue(double x, double y, double z){
			return ( ( System.Math.Sin(x) + 1) * 0.5);
		}
		
		public override double GetValue(double x, double y){
			return ( ( System.Math.Sin(x) + 1) * 0.5);
		}
		
		public override double GetValue(double x){
			return ( ( System.Math.Sin(x) + 1) * 0.5);
		}
		
		public override int TypeID{
			get{
				return 101;
			}
		}
		
	}
	
}