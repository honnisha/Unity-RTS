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
	/// Represents the suffix: css property. Used by @counter-style.
	/// </summary>
	
	public class Suffix:CssProperty{
		
		public static Suffix GlobalProperty;
		
		public Suffix(){
			GlobalProperty=this;
			InitialValueText=". ";
		}
		
		public override string[] GetProperties(){
			return new string[]{"suffix"};
		}
		
	}
	
}



