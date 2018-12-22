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
	/// Represents the stroke-linejoin: css property.
	/// </summary>
	
	public class StrokeLineJoin:CssProperty{
		
		public static StrokeLineJoin GlobalProperty;
		
		public StrokeLineJoin(){
			GlobalProperty=this;
			NamespaceName="svg";
			Inherits=true;
			InitialValueText="miter";
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-linejoin"};
		}
		
	}
	
}