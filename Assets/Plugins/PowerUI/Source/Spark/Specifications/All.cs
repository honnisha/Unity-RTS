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
	/// Represents 'a b' in the CSS specification.
	/// </summary>
	
	public class All : Spec.Value{
		
		/// <summary>The underlying set of values.</summary>
		public Spec.Value[] Set;
		
		
		public All(params Spec.Value[] set){
			Set=set;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			int setCount=Set.Length;
			size=0;
			
			for(int i=0;i<setCount;i++){
				
				int currentSize;
				if(Set[i].OnReadValue(styleBlock,value,start,out currentSize)){
					
					start+=currentSize;
					size+=currentSize;
					
				}else{
					
					size=0;
					return false;
					
				}
				
			}
			
			// All ok!
			return true;
			
		}
		
	}
	
}