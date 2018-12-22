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
	/// Represents the width: css property.
	/// </summary>
	
	public class Width:CssProperty{
		
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static Width GlobalProperty;
		
		
		public Width(){
			
			// This is along the x axis:
			Axis=ValueAxis.X;
			
			// It's the width property:
			IsWidthOrHeight=true;
			
			// Grab a global reference:
			GlobalProperty=this;
			
			InitialValue=AUTO;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"width"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



