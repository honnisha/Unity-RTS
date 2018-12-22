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
	/// Mirrors -moz-script-level.
	/// </summary>
	
	public class ScriptLevel:CssProperty{
		
		public static ScriptLevel GlobalProperty;
		
		
		public ScriptLevel(){
			// Note that it is not IsTextual - this is correct!
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-script-level"};
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



