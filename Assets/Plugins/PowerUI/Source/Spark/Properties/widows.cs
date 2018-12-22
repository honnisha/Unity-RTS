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
	/// Represents the widows: css property.
	/// </summary>
	
	public class Widows:CssProperty{
		
		public static Widows GlobalProperty;
		
		
		public Widows(){
			Inherits=true;
			InitialValueText="2";
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"widows"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



