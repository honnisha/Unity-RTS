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
	/// Represents a? in the specification.
	/// </summary>
	
	public class Optional : Spec.Value{
		
		/// <summary>The optional value</summary>
		public Spec.Value Value;
		
		
		public Optional(Spec.Value value){
			Value=value;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			if(!Value.OnReadValue(styleBlock,value,start,out size)){
				
				size=0;
				
			}
			
			// Ok either way:
			return true;
			
		}
		
	}
	
}