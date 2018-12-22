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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the all: css property.
	/// </summary>
	
	public class All:CssProperty{
		
		public static All GlobalProperty;
		
		
		public All(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"all"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Reset all CSS properties except for direction and unicode-bidi to whatever value we've got specified.
			// initial | inherit | unset | revert
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



