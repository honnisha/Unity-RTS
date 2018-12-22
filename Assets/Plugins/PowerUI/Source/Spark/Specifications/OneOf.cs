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
	/// Represents a|b in the CSS specification.
	/// </summary>
	
	public class OneOf : Spec.Value{
		
		/// <summary>The underlying set of values.</summary>
		public Spec.Value[] Set;
		
		
		public OneOf(params Spec.Value[] set){
			Set=set;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			for(int i=0;i<Set.Length;i++){
				
				if(Set[i].OnReadValue(styleBlock,value,start,out size)){
					return true;
				}
				
			}
			
			size=0;
			return false;
			
		}
		
	}
	
}