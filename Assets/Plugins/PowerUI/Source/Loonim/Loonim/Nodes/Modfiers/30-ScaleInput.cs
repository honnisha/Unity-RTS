using System;
using UnityEngine;

namespace Loonim{
	
	public class ScaleInput : TextureNode{
		
		public TextureNode X{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode Y{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode Z{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public ScaleInput():base(4){}
		
		public ScaleInput(TextureNode _0,TextureNode _1,TextureNode _2,TextureNode _3):base(4){
			Sources[0]=_0;
			Sources[1]=_1;
			Sources[2]=_2;
			Sources[3]=_3;
		}
		
		public ScaleInput(TextureNode _0,TextureNode _1,TextureNode _2):base(4){
			Sources[0]=_0;
			Sources[1]=_1;
			Sources[2]=_2;
			Sources[3]=new Property(1f);
		}
		
		public ScaleInput(TextureNode _0,TextureNode _1):base(4){
			Sources[0]=_0;
			Sources[1]=_1;
			Sources[2]=new Property(1f);
			Sources[3]=new Property(1f);
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			double scaleX=X.GetValue(x,y);
			double scaleY=Y.GetValue(x,y);
			
			return SourceModule.GetColour(x * scaleX,y * scaleY);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			double scaleX=X.GetWrapped(x,y,wrap);
			double scaleY=Y.GetWrapped(x,y,wrap);
			
			return SourceModule.GetWrapped(x * scaleX, y * scaleY, wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			
			double scaleX=X.GetValue(x,y,z);
			double scaleY=Y.GetValue(x,y,z);
			double scaleZ=Z.GetValue(x,y,z);
			
			return SourceModule.GetValue(x * scaleX, y * scaleY, z * scaleZ);
		}
		
		public override double GetValue(double x, double y){
			
			double scaleX=X.GetValue(x,y);
			double scaleY=Y.GetValue(x,y);
			
			return SourceModule.GetValue(x * scaleX, y * scaleY);
		}
		
		public override double GetValue(double t){
			
			// Change frequency:
			t*=X.GetValue(t);
			
			// Sample at that point:
			return SourceModule.GetValue(t);
			
		}
		
		public override int TypeID{
			get{
				return 30;
			}
		}
		
	}
	
}
