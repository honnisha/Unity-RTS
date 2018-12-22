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
using System.Collections;
using System.Collections.Generic;


namespace Css.Properties{
	
	/// <summary>
	/// Mirrors -moz-math-variant.
	/// </summary>
	
	public class MathVariant:CssProperty{
		
		public static MathVariant GlobalProperty;
		
		
		public MathVariant(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-math-variant"};
		}
		
		public override bool Internal{
			get{
				return true;
			}
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



