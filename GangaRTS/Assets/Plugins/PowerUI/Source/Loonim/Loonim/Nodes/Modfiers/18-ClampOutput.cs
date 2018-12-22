using System;
using UnityEngine;

namespace Loonim{
	
	public class ClampOutput : TextureNode{
		
		/// <summary>The lower clamping bound.</summary>
		public TextureNode LowerBound{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The upper clamping bound.</summary>
		public TextureNode UpperBound{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public ClampOutput():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			float lower=(float)LowerBound.GetValue(x,y);
			float upper=(float)UpperBound.GetValue(x,y);
			
			// Clamp! Ignore alpha though - it comes from col1:
			if(col1.r<lower){
				col1.r=lower;
			}else if(col1.r>upper){
				col1.r=upper;
			}
			
			if(col1.g<lower){
				col1.g=lower;
			}else if(col1.g>upper){
				col1.g=upper;
			}
			
			if(col1.b<lower){
				col1.b=lower;
			}else if(col1.b>upper){
				col1.b=upper;
			}
			
			return col1;
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double value = SourceModule.GetWrapped(x,y,wrap);
			double lower=LowerBound.GetWrapped(x,y,wrap);
			double upper=UpperBound.GetWrapped(x,y,wrap);
			
			if (value < lower){
				return lower;
			}else if (value > upper){
				return upper;
			}
			
			return value;
		}
		
		public override double GetValue(double x, double y, double z){
			double value = SourceModule.GetValue(x, y,z);
			double lower=LowerBound.GetValue(x,y,z);
			double upper=UpperBound.GetValue(x,y,z);
			
			if (value < lower){
				return lower;
			}else if (value > upper){
				return upper;
			}
			
			return value;
		}
		
		public override double GetValue(double x, double y){
			double value = SourceModule.GetValue(x, y);
			double lower=LowerBound.GetValue(x,y);
			double upper=UpperBound.GetValue(x,y);
			
			if (value < lower){
				return lower;
			}else if (value > upper){
				return upper;
			}
			
			return value;
		}
		
		public override double GetValue(double t){
			
			// Read the values:
			double min=LowerBound.GetValue(t);
			double max=UpperBound.GetValue(t);
			double value;
			
			if(min>max){
				// Flip, using value as temp storage:
				value=min;
				min=max;
				max=value;
			}
			
			// Source:
			value=SourceModule1.GetValue(t);
			
			if(value>max){
				return max;
			}else if(value<min){
				return min;
			}
			
			return value;
			
		}
		
		public override int TypeID{
			get{
				return 18;
			}
		}
		
	}
}
