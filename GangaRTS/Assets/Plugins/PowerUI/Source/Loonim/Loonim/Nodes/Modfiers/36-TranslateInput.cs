using System;
using UnityEngine;

namespace Loonim{
	
	public class TranslateInput : TextureNode{
		
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
		
		public TranslateInput():base(4){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			double tX=X.GetValue(x,y);
			double tY=Y.GetValue(x,y);
			
			return SourceModule.GetColour(x + tX,y + tY);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			double tX=X.GetWrapped(x,y,wrap);
			double tY=Y.GetWrapped(x,y,wrap);
			
			return SourceModule.GetWrapped(x + tX, y + tY, wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			
			double tX=X.GetValue(x,y,z);
			double tY=Y.GetValue(x,y,z);
			double tZ=Z.GetValue(x,y,z);
			
			return SourceModule.GetValue(x + tX, y + tY, z + tZ);
		}
		
		public override double GetValue(double x, double y){
			
			double tX=X.GetValue(x,y);
			double tY=Y.GetValue(x,y);
			
			return SourceModule.GetValue(x + tX, y + tY);
		}
		
		public override double GetValue(double t){
			
			// Shift:
			t+=X.GetValue(t);
			
			// Sample at that point:
			return SourceModule.GetValue(t);
			
		}
		
		public override int TypeID{
			get{
				return 36;
			}
		}
		
	}
	
}
