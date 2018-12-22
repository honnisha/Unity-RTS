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
	/// Represents the white-space: css property.
	/// </summary>
	
	public class WhiteSpace:CssProperty{
		
		public static WhiteSpace GlobalProperty;
		
		public WhiteSpace(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="normal";
		}
		
		/// <summary>The value of the 'normal' keyword.</summary>
		public override float GetNormalValue(RenderableData context){
			return WhiteSpaceMode.Normal;
		}
		
		public override string[] GetProperties(){
			return new string[]{"white-space"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



