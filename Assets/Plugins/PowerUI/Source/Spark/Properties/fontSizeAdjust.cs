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
	/// Represents the font-size-adjust: css property.
	/// </summary>
	
	public class FontSizeAdjust:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontSizeAdjust GlobalProperty;
		
		
		public FontSizeAdjust(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-size-adjust"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			float adjust=-1f;
			
			if(value!=null && value.Text!="none"){
				
				adjust=(float)value.GetDecimal(style.RenderData,this);
			
			}
			
			text.FontSizeAdjust=adjust;
			
			// Clear dimensions so they get re-computed:
			text.ClearDimensions();
			
			// Request a redraw:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



