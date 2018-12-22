using System;
using UnityEngine;

namespace Loonim{
	
	public class Log : Std2InputNode {
		
		public TextureNode BaseModule{
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
			UnityEngine.Color col2=SourceModule.GetColour(x,y);
			
			col1.r=(float)System.Math.Log(col2.r,col1.r);
			col1.g=(float)System.Math.Log(col2.g,col1.g);
			col1.b=(float)System.Math.Log(col2.b,col1.b);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return System.Math.Log(SourceModule.GetWrapped(x,y,wrap),BaseModule.GetWrapped(x,y,wrap));
		}
		
		public override double GetValue(double x, double y, double z){
			return System.Math.Log(SourceModule.GetValue(x, y, z),BaseModule.GetValue(x, y, z));
		}
		
		public override double GetValue(double x, double y){
			return System.Math.Log(SourceModule.GetValue(x, y),BaseModule.GetValue(x, y));
		}
		
		public override int TypeID{
			get{
				return 50;
			}
		}
		
	}
	
}
