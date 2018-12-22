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
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the background-image: css property.
	/// </summary>
	
	public class BackgroundImageProperty:CssProperty{
		
		public static BackgroundImageProperty GlobalProperty;
		
		public BackgroundImageProperty(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-image"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			Css.BackgroundImage image=GetBackground(style);
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				
				if(image.Image!=null){
					image.Image.GoingOffDisplay();
					image.Image=null;
				}
				
				// Reverse any isolation:
				image.Include();
				
			}else if(value.Type==Css.ValueType.Image){
				
				// Pull the image from the CSS value and create the package now:
				image.Image=new ImagePackage(value.GetImage(style.RenderData,GlobalProperty));
				
				// Instantly call ready:
				image.ImageReady(image.Image);
				
			}else{
				
				// Load it now!
				image.Image=new ImagePackage(value.Text,image.RenderData.Document.basepath);
				
				image.Image.onload=delegate(UIEvent e){
					
					// Call image ready now:
					image.ImageReady(image.Image);
					
				};
				
				// Send:
				image.Image.send();
				
			}
			
			// Request a layout:
			image.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



