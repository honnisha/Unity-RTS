using System;
using UnityEngine;

namespace Loonim{
	
	public class Power : Std2InputNode{
		
		public TextureNode BaseModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode PowerModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=BaseModule.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=PowerModule.GetColour(x,y);
			
			col1.r=(float)System.Math.Pow(col1.r,col2.r);
			col1.g=(float)System.Math.Pow(col1.g,col2.g);
			col1.b=(float)System.Math.Pow(col1.b,col2.b);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return System.Math.Pow(BaseModule.GetWrapped(x,y,wrap), PowerModule.GetWrapped(x,y,wrap));
		}
		
		public override double GetValue(double x, double y, double z){
			return System.Math.Pow(BaseModule.GetValue(x, y, z), PowerModule.GetValue(x, y, z));
		}
		
		public override double GetValue(double x, double y){
			return System.Math.Pow(BaseModule.GetValue(x, y), PowerModule.GetValue(x, y));
		}
		
		public override int TypeID{
			get{
				return 27;
			}
		}
		
	}
	
}
