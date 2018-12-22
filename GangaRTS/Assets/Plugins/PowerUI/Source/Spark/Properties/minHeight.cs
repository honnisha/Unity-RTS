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
	/// Represents the min-height: css property.
	/// </summary>
	
	public class MinHeight:CssProperty{
		
		public static MinHeight GlobalProperty;
		
		public MinHeight(){
			
			// This is along the y axis:
			Axis=ValueAxis.Y;
			GlobalProperty=this;
			InitialValue=ZERO;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"min-height"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



