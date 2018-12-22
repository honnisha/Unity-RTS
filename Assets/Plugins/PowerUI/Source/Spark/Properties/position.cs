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
	/// Represents the position: css property.
	/// </summary>
	
	public class Position:CssProperty{
		
		public static Position GlobalProperty;
		
		
		public Position(){
			GlobalProperty=this;
			InitialValueText="static";
		}
		
		public override string[] GetProperties(){
			return new string[]{"position"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Just request a layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.ReloadValue;
			
		}
		
	}
	
}



