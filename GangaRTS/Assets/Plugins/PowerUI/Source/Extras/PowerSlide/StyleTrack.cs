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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerUI;
using Dom;
using Json;


namespace PowerSlide{
	
	/// <summary>
	/// A style track. They're simply a list of slides which define a series of style changes.
	/// In this sense, they're similar to @keyframes but can apply to a multitude of elements
	/// and in a series.
	/// </summary>
	
	public partial class StyleTrack : Track{
		
		public StyleTrack(){}
		
		/// <summary>The name of this type of track. "style" and "dialogue" are common examples.</summary>
		public override string tagName{
			get{
				return "style";
			}
		}
		
		internal override void onStart(){
			
			if(slides==null){
				return;
			}
			
			// Start each style slide:
			for(int i=0;i<slides.Length;i++){
				
				StyleSlide slide=slides[i] as StyleSlide;
				
				if(slide==null){
					continue;
				}
				
				// Start:
				slide.onStart();
				
			}
			
		}
		
		/// <summary>Creates a slide of the correct type for this track.</summary>
		public override Slide createSlide(){
			return new StyleSlide();
		}
		
		/// <summary>Loads a track from some JSON data with an optional header.</summary>
		public override void load(JSObject json,JSObject header){
			
			// Slides:
			JSArray slides=json as JSArray;
			
			if(slides==null){
				
				// Never null:
				this.slides=new Slide[0];
				return;
				
			}
			
			if(!slides.IsIndexed){
				throw new Exception("Incorrect PowerSlide track: 'slides' must be an indexed array.");
			}
			
			// Create array now:
			this.slides=new Slide[slides.length];
			
			// For each one..
			foreach(KeyValuePair<string,JSObject> kvp in slides.Values){
				
				// index is..
				int index;
				int.TryParse(kvp.Key,out index);
				
				// Create and setup the slide now:
				Slide c=new Slide();
				c.track=this;
				c.index=index;
				
				// Load the info:
				c.load(kvp.Value);
				
				// Apply:
				this.slides[index]=c;
				
			}
			
			base.load(json,header);
			
		}
		
	}
	
}	