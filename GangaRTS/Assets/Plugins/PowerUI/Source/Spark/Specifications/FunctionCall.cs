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
	/// Represents a function call in the specification.
	/// </summary>
	
	public class FunctionCall : Spec.Value{
		
		/// <summary>The function name.</summary>
		public string Name;
		
		
		public FunctionCall(string name){
			Name=name;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			Css.CssFunction func=value[start] as Css.CssFunction;
			
			if(func!=null && func.Name==Name){
				size=1;
				return true;
			}
			
			size=0;
			return false;
			
		}
		
	}
	
}