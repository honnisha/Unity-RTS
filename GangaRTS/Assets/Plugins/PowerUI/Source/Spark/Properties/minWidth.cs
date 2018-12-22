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
	/// Represents the min-width: css property.
	/// </summary>
	
	public class MinWidth:CssProperty{
		
		public static MinWidth GlobalProperty;
		
		
		public MinWidth(){
			
			// This is along the x axis:
			Axis=ValueAxis.X;
			GlobalProperty=this;
			InitialValue=ZERO;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"min-width"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



