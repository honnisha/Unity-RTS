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
	/// Represents the vertical-align: css property.
	/// </summary>
	
	public class VerticalAlign:CssProperty{
		
		public static VerticalAlign GlobalProperty;
		
		public VerticalAlign(){
			GlobalProperty=this;
			InitialValueText="baseline";
			RelativeTo=ValueRelativity.LineHeight;
		}
		
		public override string[] GetProperties(){
			return new string[]{"v-align","vertical-align"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Redraw:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



