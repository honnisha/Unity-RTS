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
	/// Represents the align-items: css property.
	/// </summary>
	
	public class AlignItems:CssProperty{
		
		public static AlignItems GlobalProperty;
		
		
		public AlignItems(){
			GlobalProperty=this;
			InitialValueText="stretch";
		}
		
		public override string[] GetProperties(){
			return new string[]{"align-items"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



