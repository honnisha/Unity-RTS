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
	/// Represents the empty-cells: css property.
	/// </summary>
	
	public class EmptyCells:CssProperty{
		
		public static EmptyCells GlobalProperty;
		
		
		public EmptyCells(){
			Inherits=true;
			InitialValueText="show";
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"empty-cells"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



