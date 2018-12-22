//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Css;


namespace Css.Spec{
	
	/// <summary>
	/// Represents a{1,4}, a* and a+ in the CSS specification. A value is repeated x times.
	/// </summary>
	
	public class Repeated : Spec.Value{
		
		/// <summary>The host value spacer. Either , or ' '</summary>
		public bool CommaSpacer;
		/// <summary>The value being repeated.</summary>
		public Spec.Value ToRepeat;
		/// <summary>Min repetition times.</summary>
		public int Minimum;
		/// <summary>Max repetition times. -1 is unlimited.</summary>
		public int Maximum=-1;
		
		
		/// <summary>Same as a*.</summary>
		public Repeated(Spec.Value toRepeat){
			ToRepeat=toRepeat;
		}
		
		/// <summary>Usually used for a+, where min is 1.</summary>
		public Repeated(Spec.Value toRepeat,int min){
			ToRepeat=toRepeat;
			Minimum=min;
		}
		
		/// <summary>Usually used for a+, where min is 1.</summary>
		public Repeated(Spec.Value toRepeat,int min,bool comma){
			ToRepeat=toRepeat;
			Minimum=min;
			CommaSpacer=comma;
		}
		
		/// <summary>Used for a{min,max}.</summary>
		public Repeated(Spec.Value toRepeat,int min,int max){
			ToRepeat=toRepeat;
			Minimum=min;
			Maximum=max;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			/*
			ValueSet set=(value as Css.ValueSet);
			
			if(set!=null && (CommaSpacer && set.Spacer==" ")){
				size=0;
				return false;
			}
			*/
			
			size=0;
			int repeatCount=0;
			
			while(true){
				
				Css.Value toCheck;
				int checkStart;
				
				if(CommaSpacer){
					// Must pull a child value from 'value'.
					toCheck=value[repeatCount];
					checkStart=0;
				}else{
					toCheck=value;
					checkStart=start;
				}
				
				int currentSize;
				if(ToRepeat.OnReadValue(styleBlock,toCheck,checkStart,out currentSize)){
					
					// Match! Bump up rep count:
					repeatCount++;
					
					// Move start and size along:
					start+=currentSize;
					size+=currentSize;
					
					if(repeatCount==Maximum){
						// Max number of reps - halt.
						break;
					}
					
				}else{
					// Stop there.
					break;
				}
				
			}
			
			// Valid if repeat count is within range (we know it's already within max):
			if(repeatCount>=Minimum){
				return true;
			}
			
			size=0;
			return false;
			
		}
		
	}
	
}