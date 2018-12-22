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
using Css;
using PowerSlide;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the timeline-name: css property.
	/// </summary>
	
	public class TimelineName:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides-name","timeline-name"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// The name of the slides:
			string name=null;
			
			if(value!=null){
				
				// Get the name:
				name=value.GetText(style.RenderData,this);
				
			}
			
			if(name=="none" || name=="initial" || name=="normal"){
				name=null;
			}
			
			// Already running a slides instance?
			Timeline timeline=Timeline.get(style);
			
			if(name==null){
				// Clear:
				
				if(timeline!=null){
					// Stop without triggering done:
					timeline.stop(false);
				}
				
				// Reset matched styles
				style.ApplyMatchedStyles();
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(timeline!=null){
				
				/*
				This blocks interrupts - we allow it through
				Because the user must've got past the values being non-identical check.
				
				if(timeline.src==name){
					// Stop there - already running this timeline instance.
					return ApplyState.Ok;
				}
				*/
				
				// Stop it without done:
				timeline.stop(false);
				
				// Reset matched:
				style.ApplyMatchedStyles();
				
			}
			
			// Create timeline:
			timeline=new Timeline(style);
			timeline.src=name;
			
			// Enqueue it (adds to the update queue, but not the same as a start call):
			timeline.enqueue();
			
			// Reapply other values:
			Reapply(style,"timeline-duration");
			Reapply(style,"timeline-timing-function");
			Reapply(style,"timeline-delay");
			Reapply(style,"timeline-iteration-count");
			Reapply(style,"timeline-direction");
			//Reapply(style,"timeline-fill-mode");
			//Reapply(style,"timeline-play-state");
			
			Timeline.open(name,style.Element.document.basepath,timeline).then(delegate(object o){
				
				// Still enqueued?
				if(timeline.isEnqueued){
					
					// Start it now!
					timeline.start();
					
				}
				
			},delegate(object fail){
				
				Dom.Log.Add("Failed to start PowerSlide (via CSS): "+fail);
				
			});
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}