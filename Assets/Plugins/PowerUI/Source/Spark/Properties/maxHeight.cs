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
	/// Represents the max-height: css property.
	/// </summary>
	
	public class MaxHeight:CssProperty{
		
		public static MaxHeight GlobalProperty;
		
		public MaxHeight(){
			
			// This is along the y axis:
			Axis=ValueAxis.Y;
			GlobalProperty=this;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"max-height"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



