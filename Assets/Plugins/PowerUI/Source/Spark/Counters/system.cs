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
	/// Represents the system: css property. Used by @counter-style.
	/// </summary>
	
	public class System:CssProperty{
		
		public static System GlobalProperty;
		
		public System(){
			GlobalProperty=this;
			InitialValueText="symbolic";
		}
		
		public override string[] GetProperties(){
			return new string[]{"system"};
		}
		
	}
	
}



