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
	/// Represents the stroke-linecap: css property.
	/// </summary>
	
	public class StrokeLineCap:CssProperty{
		
		public static StrokeLineCap GlobalProperty;
		
		public StrokeLineCap(){
			GlobalProperty=this;
			NamespaceName="svg";
			Inherits=true;
			InitialValueText="butt";
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-linecap"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}