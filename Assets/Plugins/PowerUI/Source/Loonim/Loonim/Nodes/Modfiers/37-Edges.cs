using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Edge detection method.
	/// </summary>
	public enum EdgeMethod:int{
		Sobel=0,
		Gradient=1,
		Roberts=2
	}
	
	/// <summary>
	/// Highlights edges on the greyscale image.
	/// </summary>
	
	public class Edges : TextureNode{
		
		/// <summary>The radius of the edge detection.</summary>
		public TextureNode RadiusX{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The strength of the detection.</summary>
		public TextureNode Strength{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The method to use.</summary>
		public TextureNode Method{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public Edges():base(4){}
		
		public Edges(TextureNode src,TextureNode radius,TextureNode str,TextureNode method):base(4){
			SourceModule=src;
			RadiusX=radius;
			Strength=str;
			Method=method;
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
				return 37;
			}
		}
		
	}
}
