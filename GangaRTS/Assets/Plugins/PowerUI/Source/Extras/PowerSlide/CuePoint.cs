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
	/// A cue point. These are ordinary slides on a CueTrack.
	/// They can also define a selector
	/// </summary>
	
	public class CuePoint : Slide{
		
		/// <summary>Zero (the default length).</summary>
		private static Css.Value ZERO=new Css.Units.DecimalUnit(0f);
		
		/// <summary>True if the selector is global.</summary>
		public bool global;
		/// <summary>Optional: Selects a UI element which will continue from the cue.</summary>
		public string selector;
		/// <summary>Optional: The name of the event that will continue from the cue.</summary>
		public string eventName;
		
		
		public override void load(JSObject json){
			
			// Selects one (or more) elements which will continue this cue:
			selector=json.String("cued-by");
			
			if(selector!=null){
				selector=selector.Trim();
				
				// Global selector?
				string globalSelector=json.String("global");
				
				global=(globalSelector!=null && globalSelector=="true");
				
				if(selector==""){
					selector=null;
				}
				
			}
			
			// The event that'll continue this cue (e.g. "mousedown"):
			eventName=json.String("event");
			
			// These use "wait-at" instead of "start" to uniquely identify them:
			string startText=json.String("wait-at");
			
			if(startText!=null){
				// Load the start value:
				startValue=Css.Value.Load(startText);
			}
			
			base.load(json);
			
			// Duration is always zero:
			durationValue=ZERO;
			
		}
		
		/// <summary>The cued-by elements. These will cue the slides (to make it continue).</summary>
		public HTMLCollection cuedBy{
			get{
				
				if(selector==null){
					return null;
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
		
		internal override void start(){
			
			// Pause it now!
			Timeline tl=track.timeline;
			tl.setPause(true);
			
			if(tl.backwards){
				tl.currentTime=1f-computedStart;
			}else{
				tl.currentTime=computedStart;
			}
			
			// Hook up cue elements now:
			HTMLCollection targs=cuedBy;
			
			if(targs!=null && eventName!=null){
				
				foreach(Element e in targs){
					
					Dom.EventListener<Dom.Event> d=new Dom.EventListener<Dom.Event>(delegate(Dom.Event ev){
						
						// Cued by the cue point!
						element.cue();
						
					});
					
					CueElementData ced=new CueElementData(eventName,d,e);
					
					// Add to a list on the timeline:
					if(track.timeline.cueElements==null){
						track.timeline.cueElements=new List<CueElementData>();
					}
					
					track.timeline.cueElements.Add(ced);
					
					// Add listener:
					e.addEventListener(eventName,d);
					
				}
				
			}
			
			base.start();
			
		}
		
	}
	
	internal class CueElementData{
		
		public string eventName;
		public Dom.EventListener<Dom.Event> eventListener;
		public Element target;
		
		
		internal CueElementData(string name,Dom.EventListener<Dom.Event> listener,Element target){
			
			eventName=name;
			eventListener=listener;
			this.target=target;
			
		}
		
	}
	
}