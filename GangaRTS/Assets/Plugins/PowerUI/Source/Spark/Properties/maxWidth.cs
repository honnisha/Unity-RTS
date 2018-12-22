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
	/// Represents the max-width: css property.
	/// </summary>
	
	public class MaxWidth:CssProperty{
		
		public static MaxWidth GlobalProperty;
		
		public MaxWidth(){
			
			// This is along the x axis:
			Axis=ValueAxis.X;
			GlobalProperty=this;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"max-width"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



