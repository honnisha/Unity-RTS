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
	/// Represents the range: css property. Used by @counter-style.
	/// </summary>
	
	public class RangeProperty:CssProperty{
		
		public static RangeProperty GlobalProperty;
		
		public RangeProperty(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"range"};
		}
		
	}
	
}



