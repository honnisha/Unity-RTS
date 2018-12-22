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
	/// Represents the symbols: css property. Used by @counter-style.
	/// </summary>
	
	public class AdditiveSymbols:CssProperty{
		
		public static AdditiveSymbols GlobalProperty;
		
		public AdditiveSymbols(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"additive-symbols"};
		}
		
	}
	
}



