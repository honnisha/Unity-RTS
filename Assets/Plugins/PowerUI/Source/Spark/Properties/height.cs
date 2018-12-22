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
	/// Represents the height: css property.
	/// </summary>
	
	public class Height:CssProperty{
		
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static Height GlobalProperty;
		
		
		public Height(){
			
			// This is along the y axis:
			Axis=ValueAxis.Y;
			
			// It's the height property:
			IsWidthOrHeight=true;
			
			// Grab a global reference:
			GlobalProperty=this;
			
			InitialValue=AUTO;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"height"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



