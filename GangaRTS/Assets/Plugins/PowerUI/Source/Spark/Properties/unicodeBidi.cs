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
using Dom;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the unicode-bidi: css property.
	/// </summary>
	
	public class UnicodeBidi:CssProperty{
		
		public static UnicodeBidi GlobalProperty;
		
		
		public UnicodeBidi(){
			
			GlobalProperty=this;
			InitialValueText="normal";
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"unicode-bidi"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



