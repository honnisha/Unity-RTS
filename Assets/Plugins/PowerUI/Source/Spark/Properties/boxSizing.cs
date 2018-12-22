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
	/// Represents the box-sizing: css property.
	/// </summary>
	
	public class BoxSizing:CssProperty{
		
		public static BoxSizing GlobalProperty;
		
		
		public BoxSizing(){
			
			GlobalProperty=this;
			InitialValueText="content-box";
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"box-sizing"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



