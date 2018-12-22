using System;
using UnityEngine;

namespace Loonim{
	
	public class Modulus : Std2InputNode{
		
		public TextureNode DividendModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode DivisorModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		

		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=DividendModule.GetColour(x,y);
			
			// Read colour:
			UnityEngine.Color col2=DivisorModule.GetColour(x,y);
			
			col1.r=(float)(col2.r % col1.r);
			col1.g=(float)(col2.g % col1.g);
			col1.b=(float)(col2.b % col1.b);
			
			return col1;
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			return DividendModule.GetWrapped(x,y,wrap) % DivisorModule.GetWrapped(x,y,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			return DividendModule.GetValue(x, y, z) % DivisorModule.GetValue(x, y, z);
		}
		
		public override double GetValue(double x, double y){
			return DividendModule.GetValue(x, y) % DivisorModule.GetValue(x, y);
		}
		
		public override int TypeID{
			get{
				return 60;
			}
		}
		
	}
	
}
