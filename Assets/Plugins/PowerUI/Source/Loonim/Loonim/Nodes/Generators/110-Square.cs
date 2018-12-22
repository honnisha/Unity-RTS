using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a square wave. Checkerboard is the 2D version of this.
    /// </summary>
    public class Square:TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public override double GetValue(double x,double y){
			
			int x0=x<0?((int)((x-1)*2)):(int)(x*2);
			
			if((x0 & 1)==0){
				return 0;
			}
			
			return 1.0;
			
		}
		
		public override double GetValue(double x){
			
			int x0=x<0?((int)((x-1)*2)):(int)(x*2);
			
			if((x0 & 1)==0){
				return 0;
			}
			
			return 1.0;
			
		}
		
		public override int TypeID{
			get{
				return 110;
			}
		}
		
	}
	
}