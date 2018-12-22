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
	/// Represents the stroke-width: css property.
	/// </summary>
	
	public class StrokeWidth:CssProperty{
		
		public static StrokeWidth GlobalProperty;
		
		public StrokeWidth(){
			GlobalProperty=this;
			NamespaceName="svg";
			Inherits=true;
			InitialValueText="1";
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke-width"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



