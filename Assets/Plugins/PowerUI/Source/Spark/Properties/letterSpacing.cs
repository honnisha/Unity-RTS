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
	/// Represents the letter-spacing: css property.
	/// </summary>
	
	public class LetterSpacing:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static LetterSpacing GlobalProperty;
		
		public LetterSpacing(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
			InitialValueText="normal"; // 0
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"letter-spacing"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}