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
	/// Represents the prefix: css property. Used by @counter-style.
	/// </summary>
	
	public class Prefix:CssProperty{
		
		public static Prefix GlobalProperty;
		
		public Prefix(){
			GlobalProperty=this;
			InitialValueText="";
		}
		
		public override string[] GetProperties(){
			return new string[]{"prefix"};
		}
		
	}
	
}



