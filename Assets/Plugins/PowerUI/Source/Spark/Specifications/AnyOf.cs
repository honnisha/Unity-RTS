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
	/// Represents a || in the CSS specification. One or more in any order.
	/// </summary>
	
	public class AnyOf : Spec.Value{
		
		/// <summary>The underlying set of values.</summary>
		public Spec.Value[] Set;
		/// <summary>Tracks which in the set have been used up. Prevents using them twice.</summary>
		private bool[] Used;
		
		
		public AnyOf(params Spec.Value[] set){
			Set=set;
			Used=new bool[set.Length];
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			int setCount=Set.Length;
			int usedUp=0;
			size=0;
			
			// Unfortunately this is required to be worst-case n^2, but n is usually very small (typically just 2).
			bool somethingWasSpent=true;
			
			while(somethingWasSpent){
				
				// Clear the flag:
				somethingWasSpent=false;
				
				for(int b=0;b<setCount;b++){
					
					// If it has been 'spent' then ignore this one.
					if(Used[b]){
						continue;
					}
					
					int currentSize;
					if(Set[b].OnReadValue(styleBlock,value,start,out currentSize)){
						
						// 'b' has now been used up.
						Used[b]=true;
						
						// Move start and size along:
						start+=currentSize;
						size+=currentSize;
						
						// We now go again if there's any that have not been used up.
						somethingWasSpent=true;
						usedUp++;
						break;
						
					}
					
				}
				
				if(usedUp==setCount){
					// Used them all up.
					break;
				}
				
			}
			
			// Clear used:
			for(int i=0;i<setCount;i++){
				Used[i]=false;
			}
			
			// Valid if at least 1 matched.
			return (size!=0);
			
		}
		
	}
	
}