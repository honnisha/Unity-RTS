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
	/// Represents the baseline-shift: css property.
	/// </summary>
	
	public class BaselineShift:CssProperty{
		
		public static BaselineShift GlobalProperty;
		
		
		public BaselineShift(){
			GlobalProperty=this;
			InitialValue=ZERO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"baseline-shift"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



