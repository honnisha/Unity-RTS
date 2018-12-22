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
	/// Represents the stroke-dasharray: css property.
	/// </summary>
	
	public class StrokeDashArray:CssProperty{
		
		public static StrokeDashArray GlobalProperty;
		
		public StrokeDashArray(){
			GlobalProperty=this;
			NamespaceName="svg";
			Inherits=true;
			// None
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-dasharray"};
		}
		
	}
	
}