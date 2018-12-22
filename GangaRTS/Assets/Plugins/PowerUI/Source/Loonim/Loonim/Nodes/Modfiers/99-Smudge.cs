using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Smudges the input.
	/// </summary>
	
	public class Smudge: Std2InputNode{
		
		/// <summary>
		/// The trail used to smudge the input When this is white, the trail is longest.
		/// </summary>
		public TextureNode Trail{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public Smudge(){}
		
		public Smudge(TextureNode src){
			SourceModule=src;
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			return SourceModule.GetColour(x,y);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			return SourceModule.GetWrapped(x,y,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			
			return SourceModule.GetValue(x,y,z);
		}
		
		public override double GetValue(double x, double y){
			
			return SourceModule.GetValue(x,y);
		}
		
		public override int TypeID{
			get{
				return 99;
			}
		}
		
	}
	
}