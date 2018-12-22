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
	/// Represents the fill-opacity: css property.
	/// </summary>
	
	public class FillOpacity:CssProperty{
		
		public static FillOpacity GlobalProperty;
		
		
		public FillOpacity(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="1";
		}
		
		public override string[] GetProperties(){
			return new string[]{"fill-opacity"};
		}
		
	}
	
}



