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
	/// Represents the stroke-dashoffset: css property.
	/// </summary>
	
	public class StrokeDashOffset:CssProperty{
		
		public static StrokeDashOffset GlobalProperty;
		
		public StrokeDashOffset(){
			GlobalProperty=this;
			NamespaceName="svg";
			Inherits=true;
			InitialValue=ZERO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-dashoffset"};
		}
		
	}
	
}