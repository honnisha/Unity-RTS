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
	/// Represents the border-collapse: css property.
	/// </summary>
	
	public class BorderColllapseProperty:CssProperty{
		
		public BorderColllapseProperty(){
			Inherits=true;
			InitialValueText="separate";
		}
		
		public override string[] GetProperties(){
			return new string[]{"border-collapse"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



