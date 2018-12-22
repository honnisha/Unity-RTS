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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using Blaze;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Information for overlaying a background over text.
	/// </summary>
	public class BackgroundOverlay{
		
		/// <summary>The graphic to display.</summary>
		public ImagePackage Image;
		/// <summary>The location of the image on an atlas if it's on one.</summary>
		public AtlasLocation ImageLocation;
		/// <summary>The filter mode to display the image with.</summary>
		public FilterMode Filtering=FilterMode.Point;
		/// <summary>The text property this is for.</summary>
		public TextRenderingProperty Text;
		
		
		// -> It's for overlaying images over text.
		public BackgroundOverlay(TextRenderingProperty text){
			Text=text;
		}
		
		/// <summary>A callback used when the graphic has been loaded and is ready for display.</summary>
		public void ImageReady(){
			if(Image==null || !Image.Loaded){
				return;
			}
			
			Text.RequestLayout();
			
			if(Image!=null && Filtering!=FilterMode.Point){
				Image.Contents.FilterMode=Filtering;
			}
		}
		
		/// <summary>Lays out this background.</summary>
		internal void Layout(LayoutBox box,Renderman renderer){
			
			if(Image==null || !Image.Loaded){
				// Reject the visibility state change.
				ImageLocation=null;
				Text.Include();
				return;
			}
			
			// Tell the image that the box has likely changed - this allows it to redraw (e.g. SVGs):
			float trueImageWidth;
			float trueImageHeight;
			Image.Contents.OnLayout(Text.RenderData,box,out trueImageWidth,out trueImageHeight);
			
			if(ImageLocation==null){
				
				// Tell it that it's going on screen:
				Image.GoingOnDisplay(Text.RenderData);
				
				// Create an isolated location:
				ImageLocation=new AtlasLocation(trueImageWidth,trueImageHeight);
				
			}
			
			// Isolate the text:
			Text.Isolate();
			
		}
		
	}

}