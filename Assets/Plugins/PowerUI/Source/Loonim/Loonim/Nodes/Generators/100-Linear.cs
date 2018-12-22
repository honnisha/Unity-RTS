using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a linear graph.
    /// </summary>
    public class Linear:TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return x;
		}
		
		public override double GetValue(double x, double y, double z){
			return x;
		}
		
		public override double GetValue(double x, double y){
			return x;
		}
		
		public override double GetValue(double x){
			return x;
		}
		
		public override int TypeID{
			get{
				return 100;
			}
		}
		
	}
	
}