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
	/// Mirrors -moz-math-display.
	/// </summary>
	
	public class MathDisplay:CssProperty{
		
		public static MathDisplay GlobalProperty;
		
		
		public MathDisplay(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-math-display"};
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



