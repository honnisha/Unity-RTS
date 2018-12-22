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
	/// Represents the on-atlas: css property. Specific to PowerUI. This tells particular images to render off the main atlas.
	/// </summary>
	
	public class OnAtlas:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"on-atlas"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			BackgroundImage image=GetBackground(style);
			
			if(value==null){
				image.ForcedIsolate=false;
			}else{
				image.ForcedIsolate=!value.GetBoolean(style.RenderData,this);
			}
			
			// Request a layout:
			image.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



