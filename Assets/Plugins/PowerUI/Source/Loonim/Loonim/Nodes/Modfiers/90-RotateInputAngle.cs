using System;
using UnityEngine;

namespace Loonim{

    public class RotateInputAngle : Std2InputNode{
        
        /// <summary>The angle to rotate by</summary>
        public TextureNode RotationModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Get the angle to rotate by (rad):
			double deltaAngle=RotationModule.GetValue(x,y);
			
			// rotate the point x/y deltaAngle about 0.5/0.5
			x-=0.5;
			y-=0.5;
			
			// Get sin/cos:
			double cAngle=System.Math.Cos(deltaAngle);
			double sAngle=System.Math.Sin(deltaAngle);
			
			double tx=x*cAngle - y*sAngle;
			y=y*cAngle + x*sAngle;
			x=tx;
			
			x+=0.5;
			y+=0.5;
			
			return SourceModule.GetColour(x,y);
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Get the angle to rotate by (rad):
			double deltaAngle=RotationModule.GetWrapped(x,y,wrap);
			
			// rotate the point x/y deltaAngle about 0.5/0.5
			x-=0.5;
			y-=0.5;
			
			// Get sin/cos:
			double cAngle=System.Math.Cos(deltaAngle);
			double sAngle=System.Math.Sin(deltaAngle);
			
			double tx=x*cAngle - y*sAngle;
			y=y*cAngle + x*sAngle;
			x=tx;
			
			x+=0.5;
			y+=0.5;
			
            return SourceModule.GetWrapped(x,y,wrap);
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Get the angle to rotate by (rad):
			double deltaAngle=RotationModule.GetValue(x,y,z);
			
			// rotate the point x/y deltaAngle about 0.5/0.5
			x-=0.5;
			y-=0.5;
			
			// Get sin/cos:
			double cAngle=System.Math.Cos(deltaAngle);
			double sAngle=System.Math.Sin(deltaAngle);
			
			double tx=x*cAngle - y*sAngle;
			y=y*cAngle + x*sAngle;
			x=tx;
			
			x+=0.5;
			y+=0.5;
			
            return SourceModule.GetValue(x, y ,z);
		}
		
        public override double GetValue(double x, double y){
            
			// Get the angle to rotate by (rad):
			double deltaAngle=RotationModule.GetValue(x,y);
			
			// rotate the point x/y deltaAngle about 0.5/0.5
			x-=0.5;
			y-=0.5;
			
			// Get sin/cos:
			double cAngle=System.Math.Cos(deltaAngle);
			double sAngle=System.Math.Sin(deltaAngle);
			
			double tx=x*cAngle - y*sAngle;
			y=y*cAngle + x*sAngle;
			x=tx;
			
			x+=0.5;
			y+=0.5;
			
            return SourceModule.GetValue(x, y);
        }
		
		public override int TypeID{
			get{
				return 90;
			}
		}
		
    }
	
}