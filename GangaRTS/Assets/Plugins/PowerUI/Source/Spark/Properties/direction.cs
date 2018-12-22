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
	/// Represents the direction: css property.
	/// </summary>
	
	public class Direction:CssProperty{
		
		public static Direction GlobalProperty;
		
		
		public Direction(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="ltr";
		}
		
		public override string[] GetProperties(){
			return new string[]{"direction"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// If we've got a writing system property, reset it:
			SparkWritingSystem.GlobalProperty.UpdateMap(style);
			
			// Request a layout now:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



