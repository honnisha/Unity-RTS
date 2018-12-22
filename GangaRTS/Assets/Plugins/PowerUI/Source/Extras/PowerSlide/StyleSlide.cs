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
	/// A style slide.
	/// </summary>
	
	public class StyleSlide : Slide{
		
		/// <summary>True if the selector is global.</summary>
		public bool global;
		/// <summary>The selector. May be null.</summary>
		public string selector;
		/// <summary>The style to apply.</summary>
		public Css.Style styleToApply;
		/// <summary>The latest targets.</summary>
		internal HTMLCollection latestTargets;
		
		
		internal override void setPause(bool paused){
			
			if(latestTargets==null){
				return;
			}
			
			foreach(Element e in latestTargets){
				
				// Get its animation instance and pause/ unpause it:
				var ai=(e as Css.IRenderableNode).ComputedStyle.AnimationInstance;
				
				if(ai!=null){
					ai.SetPause(paused);
				}
				
			}
			
		}
		
		public override void load(JSObject json){
			
			// Get the selector (may be null):
			selector=json.String("selector");
			
			if(selector!=null){
				selector=selector.Trim();
				
				// Global selector?
				string globalSelector=json.String("global");
				
				global=(globalSelector!=null && globalSelector=="true");
				
				if(selector==""){
					selector=null;
				}
				
			}
			
			// Get the style and/or animation:
			string style=json.String("style");
			
			if(style!=null){
				style=style.Trim();
			}
			
			string anim=json.String("animation");
			
			if(!string.IsNullOrEmpty(anim)){
				
				// Shortform:
				if(style==null){
					style="";
				}else if(style!=""){
					style+=";";
				}
				
				// Set name:
				style+="animation-name:"+anim;
				
			}
			
			if(style!=null){
				// Load the style:
				styleToApply=new Css.Style(style,null);
			}else{
				Dom.Log.Add("PowerSlide Style slide with no style in it!");
			}
			
			base.load(json);
		}
		
		/// <summary>The targeted element(s).</summary>
		public HTMLCollection targets{
			get{
				
				if(selector==null){
					// Just the host:
					HTMLCollection c=new HTMLCollection();
					c.push(element);
					return c;
				}
				
				if(global){
					
					// Select from the doc:
					Css.ReflowDocument doc=(element.document as Css.ReflowDocument);
					
					Css.IRenderableNode node=(doc.documentElement as Css.IRenderableNode);
					
					return node.querySelectorAll(selector);
					
				}
				
				return (element as Css.IRenderableNode).querySelectorAll(selector);
				
			}
		}
		
		internal void onStart(){
			
			// Query the elements now!
			HTMLCollection allTargets=targets;
			latestTargets=allTargets;
			
			foreach(Element e in latestTargets){
				
				// Get as a HTML element:
				HtmlElement htmlE=(e as HtmlElement);
				
				if(htmlE==null){
					// Hello SVG
					continue;
				}
				
				// Clear animation-name (resets it to the default state):
				htmlE.style["animation-name"]=null;
				
			}
			
		}
		
		internal override void start(){
			
			// Apply anim-duration, anim-direction and the rest of the style
			// (style must be last, as it contains animation-name):
			
			string dir=track.timeline.backwards?"backwards" : "forwards";
			
			foreach(Element e in latestTargets){
				
				// Get as a HTML element:
				HtmlElement htmlE=(e as HtmlElement);
				
				if(htmlE==null){
					// Hello SVG
					continue;
				}
				
				// Set duration:
				htmlE.style["animation-duration"]=new Css.Units.DecimalUnit(computedDuration);
				
				// Set direction:
				htmlE.style["animation-direction"]=new Css.Units.TextUnit(dir);
				
				if(styleToApply!=null){
				
					// Handle the rest:
					foreach(KeyValuePair<Css.CssProperty,Css.Value> kvp in styleToApply.Properties){
						
						// Apply it directly:
						htmlE.style[kvp.Key]=kvp.Value.Copy();
						
					}
					
				}
				
			}
			
			base.start();
			
		}
		
	}
	
}