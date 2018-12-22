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
	/// Represents the text-transform: css property.
	/// </summary>
	
	public class TextTransform:CssProperty{
		
		public static TextTransform GlobalProperty;
		
		public TextTransform(){
			IsTextual=true;
			Inherits=true;
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-transform"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			// Apply the changes:
			text.ClearText();
			
			// Request a layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



