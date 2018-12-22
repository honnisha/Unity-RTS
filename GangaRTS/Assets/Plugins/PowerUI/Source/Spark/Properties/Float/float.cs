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
	/// Represents the float: css property.
	/// </summary>
	
	public class Float:CssProperty{
		
		public static Float GlobalProperty;
		
		
		public Float(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"float"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}