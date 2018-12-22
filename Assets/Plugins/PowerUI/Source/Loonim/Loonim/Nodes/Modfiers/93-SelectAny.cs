using System;
using UnityEngine;

namespace Loonim{

    /// <summary>
    /// Control(t) normalised to an index in Points, which is then read.
	/// Note that this also blends between the nearest points when possible.
    /// </summary>
    public class SelectAny : TextureNode{
		
		public TextureNode[] Points;
		
		public TextureNode ControlModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Get control (0-1):
			double control=ControlModule.GetValue(x,y);
			
			int count=Sources.Length-1;
			
			if(count==0){
				return new UnityEngine.Color(0,0,0,1);
			}else if(count==1){
				return Sources[1].GetColour(x,y);
			}
			
			// Reduce count (such that it ranges from 0->count-1):
			count--;
			
			// Map to being relative to the points:
			control *= (double)count;
			
			int index=(int)control;
			
			if(index<0){
				
				// Sample it:
				return Sources[1].GetColour(x,y);
				
			}else if(index>=count){
				
				// Sample there:
				return Sources[count+1].GetColour(x,y);
				
			}else{
				
				// Make relative:
				control-=(double)index;
				
			}
			
			// Get current:
			TextureNode current=Sources[index+1];
			
			if(control==0){
				// Just read it:
				return current.GetColour(x,y);
			}
			
			// Control is now 0-1 relative to Sources[index+1]. Get as float:
			float blend=(float)control;
			
			// Note that index is never equal to (actual count-1), therefore next is just:
			TextureNode next=Sources[index+2];
			
			// Blend:
			UnityEngine.Color col1=current.GetColour(x,y);
			UnityEngine.Color col2=next.GetColour(x,y);
			
			col1.r+=(col1.r-col2.r)*blend;
			col1.g+=(col1.g-col2.g)*blend;
			col1.b+=(col1.b-col2.b)*blend;
			col1.a+=(col1.a-col2.a)*blend;
			
			return col1;
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Get control (0-1):
			double control=ControlModule.GetWrapped(x,y,wrap);
			
			int count=Sources.Length-1;
			
			if(count==0){
				return control;
			}else if(count==1){
				return Sources[1].GetWrapped(x,y,wrap);
			}
			
			// Reduce count (such that it ranges from 0->count-1):
			count--;
			
			// Map to being relative to the points:
			control *= (double)count;
			
			int index=(int)control;
			
			if(index<0){
				
				// Sample it:
				return Sources[1].GetWrapped(x,y,wrap);
				
			}else if(index>=count){
				
				// Sample there:
				return Sources[count+1].GetWrapped(x,y,wrap);
				
			}else{
				
				// Make relative:
				control-=(double)index;
				
			}
			
			// Control is now 0-1 relative to Sources[index+1].
			
			// Get current:
			TextureNode current=Sources[index+1];
			
			if(control==0){
				// Just read it:
				return current.GetWrapped(x,y,wrap);
			}
			
			// Note that index is never equal to (actual count-1), therefore next is just:
			TextureNode next=Sources[index+2];
			
			// Blend:
			double a=current.GetWrapped(x,y,wrap);
			double b=next.GetWrapped(x,y,wrap);
			
			return a+( (b-a) * control );
			
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Get control (0-1):
			double control=ControlModule.GetValue(x,y,z);
			
			int count=Sources.Length-1;
			
			if(count==0){
				return control;
			}else if(count==1){
				return Sources[1].GetValue(x,y,z);
			}
			
			// Reduce count (such that it ranges from 0->count-1):
			count--;
			
			// Map to being relative to the points:
			control *= (double)count;
			
			int index=(int)control;
			
			if(index<0){
				
				// Sample it:
				return Sources[1].GetValue(x,y,z);
				
			}else if(index>=count){
				
				// Sample there:
				return Sources[count+1].GetValue(x,y,z);
				
			}else{
				
				// Make relative:
				control-=(double)index;
				
			}
			
			// Control is now 0-1 relative to Sources[index+1].
			
			// Get current:
			TextureNode current=Sources[index+1];
			
			if(control==0){
				// Just read it:
				return current.GetValue(x,y,z);
			}
			
			// Note that index is never equal to (actual count-1), therefore next is just:
			TextureNode next=Sources[index+2];
			
			// Blend:
			double a=current.GetValue(x,y,z);
			double b=next.GetValue(x,y,z);
			
			return a+( (b-a) * control );
			
		}
		
        /// <summary>
        /// Returns the output of the two source modules added together.
        /// </summary>
        public override double GetValue(double x, double y){
            
			// Get control (0-1):
			double control=ControlModule.GetValue(x,y);
			
			int count=Sources.Length-1;
			
			if(count==0){
				return control;
			}else if(count==1){
				return Sources[1].GetValue(x,y);
			}
			
			// Reduce count (such that it ranges from 0->count-1):
			count--;
			
			// Map to being relative to the points:
			control *= (double)count;
			
			int index=(int)control;
			
			if(index<0){
				
				// Sample it:
				return Sources[1].GetValue(x,y);
				
			}else if(index>=count){
				
				// Sample there:
				return Sources[count+1].GetValue(x,y);
				
			}else{
				
				// Make relative:
				control-=(double)index;
				
			}
			
			// Control is now 0-1 relative to Sources[index+1].
			
			// Get current:
			TextureNode current=Sources[index+1];
			
			if(control==0){
				// Just read it:
				return current.GetValue(x,y);
			}
			
			// Note that index is never equal to (actual count-1), therefore next is just:
			TextureNode next=Sources[index+2];
			
			// Blend:
			double a=current.GetValue(x,y);
			double b=next.GetValue(x,y);
			
			return a+( (b-a) * control );
			
        }
		
		public override double GetValue(double t){
			
			// Get control (0-1):
			double control=ControlModule.GetValue(t);
			
			int count=Sources.Length-1;
			
			if(count==0){
				return control;
			}else if(count==1){
				return Sources[1].GetValue(t);
			}
			
			// Reduce count (such that it ranges from 0->count-1):
			count--;
			
			// Map to being relative to the points:
			control *= (double)count;
			
			int index=(int)control;
			
			if(index<0){
				
				// Sample it:
				return Sources[1].GetValue(t);
				
			}else if(index>=count){
				
				// Sample there:
				return Sources[count+1].GetValue(t);
				
			}else{
				
				// Make relative:
				control-=(double)index;
				
			}
			
			// Control is now 0-1 relative to Points[index].
			
			// Get current:
			TextureNode current=Sources[index+1];
			
			if(control==0){
				// Just read it:
				return current.GetValue(t);
			}
			
			// Note that index is never equal to (actual count-1), therefore next is just:
			TextureNode next=Sources[index+2];
			
			// Blend:
			double a=current.GetValue(t);
			double b=next.GetValue(t);
			
			return a+( (b-a) * control );
			
		}
		
		public override int TypeID{
			get{
				return 93;
			}
		}
		
    }
	
}
