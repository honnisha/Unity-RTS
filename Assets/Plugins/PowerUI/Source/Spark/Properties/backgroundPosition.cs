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
	/// Represents the background-position: css property.
	/// </summary>
	
	public class BackgroundPosition:CssProperty{
		
		public BackgroundPosition(){
			InitialValueText="0% 0%";
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-position"};
		}
		
		public override void Aliases(){
			// A set of point aliases, e.g. background-position-x:
			PointAliases2D();
		}
		
		/// <summary>Gets the axis that the given word is on. E.g. 'left' is on the x axis.</summary>
		private void GetOrigin(string word,ref float x,ref float y,ref bool center,ref bool xAxis){
			
			switch(word){
				
				case "top":
					y=0f;
					xAxis=false;
					break;
				case "right":
					x=1f;
					xAxis=true;
					break;
				case "bottom":
					y=1f;
					xAxis=false;
					break;
				case "left":
					x=0f;
					xAxis=true;
					break;
				case "center":
					if(center){
						y=0.5f;
					}else{
						center=true;
						x=0.5f;
					}
					break;
					
			}
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			BackgroundImage image=GetBackground(style);
		
			Value offsetX=null;
			Value offsetY=null;
			float xOrigin=0f;
			float yOrigin=0f;
			
			if(value!=null){
				
				// Set true when center has been handled once before.
				bool center=false;
				bool xAxis=true;
				
				// Get the inner value count:
				int count=value.Count;
				
				if(count==1 || value is CssFunction){
					
					if(value.Type==ValueType.Text){
						string text=value.Text;
						
						// Occurs twice:
						GetOrigin(text,ref xOrigin,ref yOrigin,ref center,ref xAxis);
						GetOrigin(text,ref xOrigin,ref yOrigin,ref center,ref xAxis);
					}else{
						offsetX=offsetY=value;
					}
					
				}else{
					
					// Loops for e.g. right 10% top 5%
					for(int i=0;i<count;i++){
						
						// Grab the inner value:
						Value innerValue=value[i];
						
						// Is it text?
						if(innerValue.Type==ValueType.Text){
							
							// Yep!
							GetOrigin(innerValue.Text,ref xOrigin,ref yOrigin,ref center,ref xAxis);
							
						}else{
							
							// It's a %/ px/ some other fixed unit.
							if(xAxis){
								xAxis=false;
								offsetX=innerValue;
							}else{
								offsetY=innerValue;
							}
							
						}
						
					}
					
				}
				
			}
			
			image.OffsetOriginX=xOrigin;
			image.OffsetOriginY=yOrigin;
			image.OffsetX=offsetX;
			image.OffsetY=offsetY;
			
			// Request a layout:
			image.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}