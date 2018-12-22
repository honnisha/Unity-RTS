using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a "bounce" wave.
    /// </summary>
    public class Bounce:TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			double value=System.Math.Sin(x);
			
			if(value<0.0){
				return -value;
			}
			
			return value;
			
		}
		
		public override double GetValue(double x){
			
			double value=System.Math.Sin(x);
			
			if(value<0.0){
				return -value;
			}
			
			return value;
			
		}
		
		public override int TypeID{
			get{
				return 109;
			}
		}
		
	}
	
}