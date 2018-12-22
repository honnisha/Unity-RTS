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
	/// Represents the background-repeat: css property.
	/// </summary>
	
	public class BackgroundRepeat:CssProperty{
		
		public BackgroundRepeat(){
			InitialValueText="repeat";
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-repeat"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			BackgroundImage image=GetBackground(style);
			
			if(value==null || value.Text=="" || value.Text=="repeat"){
				// Repeat by default:
				
				image.RepeatX=image.RepeatY=true;
				
			}else{
			
				if(value.Text=="repeat-x"){
					// X only.
					
					image.RepeatX=true;
					image.RepeatY=false;
				}else if(value.Text=="repeat-y"){
					// Y only.
					
					image.RepeatY=true;
					image.RepeatX=false;
				}else{
					// No repeat.
					
					image.RepeatX=false;
					image.RepeatY=false;
				}
				
			}
			
			// Request a layout:
			image.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



