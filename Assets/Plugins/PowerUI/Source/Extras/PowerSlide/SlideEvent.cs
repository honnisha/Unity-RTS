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
using Dom;
using System.Collections;
using System.Collections.Generic;


namespace PowerSlide{
	
	/// <summary>
	/// Represents a PowerSlide event.
	/// </summary>
	
	public class SlideEvent : Dom.Event{
		
		/// <summary>The current timeline.</summary>
		public Timeline timeline_;
		/// <summary>The current track (group of slides).</summary>
		public Track track_;
		
		/// <summary>The current timeline.</summary>
		public Timeline timeline{
			get{
				if(timeline_!=null){
					return timeline_;
				}
				
				Track tr=track;
				if(tr!=null){
					return tr.timeline;
				}
				
				return null;
			}
			set{
				timeline_=value;
			}
		}
		
		/// <summary>The current track (group of slides).</summary>
		public Track track{
			get{
				if(track_!=null){
					return track_;
				}
				
				if(slide!=null){
					return slide.track;
				}
				
				return null;
			}
			set{
				track_=value;
			}
		}
		/// <summary>The current slide. Can be null if this is a track event.</summary>
		public Slide slide;
		/// <summary>The current action. Can be null if this is a track/slide event.</summary>
		public Action action;
		/// <summary>An optional set of globals to pass to the target.</summary>
		public Dictionary<string,object> globals;
		
		/// <summary>A convenience approach for getting/setting globals to pass through during a cue event.</summary>
		public object this[string global]{
			get{
				if(globals==null){
					return null;
				}
				
				object result;
				globals.TryGetValue(global,out result);
				return result;
			}
			set{
				if(globals==null){
					globals=new Dictionary<string,object>();
				}
				
				globals[global]=value;
			}
		}
		
		/// <summary>The HTML document that this dialogue event originated from.</summary>
		public PowerUI.HtmlDocument htmlDocument{
			get{
				Timeline tl=timeline;
				
				if(tl==null){
					return null;
				}
				
				return tl.document;
			}
		}
		
		/// <summary>A custom action ref.</summary>
		public string actionID{
			get{
				return action.ID;
			}
		}
		
		public SlideEvent(string type,object init):base(type,init){}
		
		/// <summary>Opens a widget with the given template and URL. Globals originate from this event.
		/// Convenience method for thisEvent.document.widgets.open(template,url,thisEvent.globals);</summary>
		public Widgets.Widget open(string template,string url){
			return htmlDocument.widgets.open(template,url,globals);
		}
		
		/// <summary>Opens a widget with the given template and URL and returns a promise. Globals originate from this event.
		/// Convenience method for thisEvent.document.widgets.open(template,url,thisEvent.globals);</summary>
		public PowerUI.Promise load(string template,string url){
			return htmlDocument.widgets.load(template,url,globals);
		}
		
	}
	
}