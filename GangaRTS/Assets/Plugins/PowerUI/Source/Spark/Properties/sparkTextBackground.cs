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
	/// Represents the -spark-text-background: css property.
	/// </summary>
	
	public class TextBackground:CssProperty{
		
		public static TextBackground GlobalProperty;
		
		public TextBackground(){
			Inherits=true; // Important for variables.
			GlobalProperty=this;
			IsTextual=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-text-background"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Apply the changes - doesn't change anything about the actual text, so we just want a layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>
		/// Applies to text nodes.
		/// </summary>
		public override void ApplyText(TextRenderingProperty trp,RenderableData data,ComputedStyle style,Value value){
			
			// Do nothing if the node is totally empty.
			Dom.TextNode tn=(data.Node as Dom.TextNode);
			
			if(tn!=null && tn.IsSpaces){
				return;
			}
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				
				if(trp.Background!=null && trp.Background.Image!=null){
					trp.Background.Image.GoingOffDisplay();
					trp.Background=null;
				}
				
				// Reverse any isolation:
				trp.Include();
				
			}else if(value.Type==Css.ValueType.Image){
				
				if(trp.Background==null){
					trp.Background=new BackgroundOverlay(trp);
				}
				
				// Pull the image from the CSS value and create the package now:
				ImageFormat fmt=value.GetImage(style.RenderData,GlobalProperty);
				trp.Background.Image=new ImagePackage(fmt);
				
				// Instantly call ready:
				trp.Background.ImageReady();
				
			}else{
				
				if(trp.Background==null){
					trp.Background=new BackgroundOverlay(trp);
				}
				
				// Load it now!
				trp.Background.Image=new ImagePackage(value.Text,trp.RenderData.Document.basepath);
				
				trp.Background.Image.onload=delegate(UIEvent e){
					
					// Call image ready now:
					if(trp.Background!=null){
						trp.Background.ImageReady();
					}
					
				};
				
				// Send:
				trp.Background.Image.send();
				
			}
			
		}
		
	}
	
}