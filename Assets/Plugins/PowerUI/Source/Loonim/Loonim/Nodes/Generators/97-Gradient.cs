using System;
using UnityEngine;
using Blaze;

namespace Loonim{
	
	public class Gradient :TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		/// <summary>Describes the blend transition from a colour to another.</summary>
		public GraphNode Transition;
		public Gradient2D Colours;
		// public Color[] PredrawnGradient;
		
		public Gradient(){}
		
		public Gradient(Gradient2D gradient,GraphNode transition)
		{
			Colours=gradient;
			Transition=transition;
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Grab the colour:
			return Colours.Render((float)x);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Get colour:
			UnityEngine.Color colour=GetColour(x,y);
			
			return (colour.r+colour.g+colour.b)/3.0;
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Get colour:
			UnityEngine.Color colour=GetColour(x,y);
			
			return (colour.r+colour.g+colour.b)/3.0;
			
		}
		
		public override double GetValue(double x, double y){
			
			// Get colour:
			UnityEngine.Color colour=GetColour(x,y);
			
			return (colour.r+colour.g+colour.b)/3.0;
			
		}
		
		public override int TypeID{
			get{
				return 97;
			}
		}
		
	}
}