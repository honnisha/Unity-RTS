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
	/// Represents the flood-opacity: css property.
	/// </summary>
	
	public class FloodOpacity:CssProperty{
		
		public static FloodOpacity GlobalProperty;
		
		
		public FloodOpacity(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="1";
		}
		
		public override string[] GetProperties(){
			return new string[]{"flood-opacity"};
		}
		
	}
	
}



