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
	/// Represents the stroke-opacity: css property.
	/// </summary>
	
	public class StrokeOpacity:CssProperty{
		
		public static StrokeOpacity GlobalProperty;
		
		
		public StrokeOpacity(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="1";
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-opacity"};
		}
		
	}
	
}



