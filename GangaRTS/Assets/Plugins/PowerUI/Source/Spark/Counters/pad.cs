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
	/// Represents the pad: css property. Used by @counter-style.
	/// </summary>
	
	public class Pad:CssProperty{
		
		public static Pad GlobalProperty;
		
		public Pad(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"pad"};
		}
		
	}
	
}



