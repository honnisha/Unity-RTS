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
	/// Represents the fallback: css property. Used by @counter-style.
	/// </summary>
	
	public class Fallback:CssProperty{
		
		public static Fallback GlobalProperty;
		
		public Fallback(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"fallback"};
		}
		
	}
	
}



