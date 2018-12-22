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
	/// Represents the stroke-miterlimit: css property.
	/// </summary>
	
	public class StrokeMiterLimit:CssProperty{
		
		public static StrokeMiterLimit GlobalProperty;
		
		public StrokeMiterLimit(){
			GlobalProperty=this;
			NamespaceName="svg";
			Inherits=true;
			InitialValueText="4";
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-miterlimit"};
		}
		
	}
	
}