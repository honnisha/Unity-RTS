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
	/// Represents the font-smoothing: css property.
	/// </summary>
	
	public class FontSmoothing:CssProperty{
		
		
		public FontSmoothing(){
			IsTextual=true;
			Inherits=true;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"font-smoothing"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			bool auto=(value==null || value.IsAuto || value.Text=="anti-alias");
			
			if(auto){
				text.Alias=float.MaxValue;
			}else{
				text.Alias=value.GetDecimal(style.RenderData,this);
			}
			
			// Clear dimensions so they get re-computed:
			text.ClearDimensions();
			
			// Request a redraw:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



