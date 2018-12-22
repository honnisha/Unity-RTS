using System;
using UnityEngine;

namespace Loonim{
	
    /// <summary>
    /// Repeats an image. Different from scaling input as this clips the input coords to being always in the 0-1 range.
    /// </summary>
	public class Repeat: TextureNode{
		
		/// <summary># of repetitions.</summary>
		public TextureNode Repetition{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>Should it "mirror"? Either 1 or 0.</summary>
		public TextureNode Mirror{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public Repeat():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Stretch out t by times to repeat:
			double rep=SourceModule2.GetValue(x,y);
			bool mirror=(SourceModule3.GetValue(x,y)>0.5);
			
			x*=rep;
			y*=rep;
			
			// Get the current repetition:
			int baseRepetition=(int)x;
			
			// Shift:
			x-=baseRepetition;
			
			// X is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				x=1.0-x;
			}
			
			// Get the current repetition:
			baseRepetition=(int)y;
			
			// Shift:
			y-=baseRepetition;
			
			// Y is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				y=1.0-y;
			}
			
			// Read source:
			return SourceModule1.GetColour(x,y);
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			// Stretch out t by times to repeat:
			double rep=SourceModule2.GetWrapped(x,y,wrap);
			bool mirror=(SourceModule3.GetWrapped(x,y,wrap)>0.5);
			
			x*=rep;
			y*=rep;
			
			// Get the current repetition:
			int baseRepetition=(int)x;
			
			// Shift:
			x-=baseRepetition;
			
			// X is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				x=1.0-x;
			}
			
			// Get the current repetition:
			baseRepetition=(int)y;
			
			// Shift:
			y-=baseRepetition;
			
			// Y is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				y=1.0-y;
			}
			
			// Read source:
			return SourceModule1.GetWrapped(x,y,wrap);
			
		}
		
		public override double GetValue(double x, double y, double z){
			
			// Stretch out t by times to repeat:
			double rep=SourceModule2.GetValue(x,y,z);
			bool mirror=(SourceModule3.GetValue(x,y,z)>0.5);
			
			x*=rep;
			y*=rep;
			z*=rep;
			
			// Get the current repetition:
			int baseRepetition=(int)x;
			
			// Shift:
			x-=baseRepetition;
			
			// X is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				x=1.0-x;
			}
			
			// Get the current repetition:
			baseRepetition=(int)y;
			
			// Shift:
			y-=baseRepetition;
			
			// Y is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				y=1.0-y;
			}
			
			// Get the current repetition:
			baseRepetition=(int)z;
			
			// Shift:
			z-=baseRepetition;
			
			// Z is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				z=1.0-z;
			}
			
			// Read source:
			return SourceModule1.GetValue(x,y,z);
			
		}
		
		public override double GetValue(double x, double y){
			
			// Stretch out t by times to repeat:
			double rep=SourceModule2.GetValue(x,y);
			bool mirror=(SourceModule3.GetValue(x,y)>0.5);
			
			x*=rep;
			y*=rep;
			
			// Get the current repetition:
			int baseRepetition=(int)x;
			
			// Shift:
			x-=baseRepetition;
			
			// X is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				x=1.0-x;
			}
			
			// Get the current repetition:
			baseRepetition=(int)y;
			
			// Shift:
			y-=baseRepetition;
			
			// Y is now in the 0-1 range.
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				y=1.0-y;
			}
			
			// Read source:
			return SourceModule1.GetValue(x,y);
			
		}
		
		public override double GetValue(double t){
			
			// Stretch out t by times to repeat:
			t*=SourceModule2.GetValue(t);
			
			// Get the current repetition:
			int baseRepetition=(int)t;
			
			// Shift t:
			t-=baseRepetition;
			
			// T is now in the 0-1 range.
			bool mirror=(SourceModule3.GetValue(t)>0.5);
			
			if(mirror && (baseRepetition & 1)==1){
				// "odd" repetition - flip t:
				t=1.0-t;
			}
			
			// Read source:
			return SourceModule1.GetValue(t);
			
		}
		
		public override int TypeID{
			get{
				return 19;
			}
		}
		
	}
}
