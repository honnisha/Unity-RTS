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
	/// Represents the border-spacing: css property.
	/// </summary>
	
	public class BorderSpacingProperty:CssProperty{
		
		public BorderSpacingProperty(){
			Inherits=true;
			InitialValue=ZERO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"border-spacing"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



