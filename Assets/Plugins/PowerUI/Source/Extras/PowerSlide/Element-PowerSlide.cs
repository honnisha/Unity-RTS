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
using PowerSlide;
using Css;
using PowerUI;


namespace Dom{
	
	public partial class Element{
		
		/// <summary>Runs a PowerSlide sequence on this element. This is the same as runTimeline.
		/// The returned promise triggers when it's done.</summary>
		public Promise slide(string name,float duration){
			return runTimeline(name,duration,true);
		}
		
		/// <summary>Runs a PowerSlide sequence on this element. This is the same as runTimeline.
		/// The returned promise triggers when it's done.</summary>
		public Promise slide(string name,float duration,bool killRunning){
			return runTimeline(name,duration,killRunning);
		}
		
		/// <summary>Runs a PowerSlide sequence on this element.
		/// The returned promise triggers when it's done.</summary>
		public Promise runTimeline(string name,float duration){
			return runTimeline(name,duration,true);
		}
		
		/// <summary>Runs a PowerSlide sequence on this element.
		/// The returned promise triggers when it's done.</summary>
		/// <param name='killRunning'>Kills all running slides on the style before starting a new one.</param>
		public Promise runTimeline(string name,float duration,bool killRunning){
			
			// Create a promise:
			Promise p=new Promise();
			
			ComputedStyle cs=style.Computed;
			
			if(killRunning){
				
				// Stop all slides on the style:
				Timeline.stopAll(cs);
				
			}
			
			// Create timeline:
			Timeline timeline=new Timeline(cs);
			timeline.src=name;
			
			// Enqueue it (adds to the update queue, but not the same as a start call):
			timeline.enqueue();
			
			// Reapply other values:
			timeline.appliedDuration=duration;
			
			// Load it:
			Timeline.open(name,document.basepath,timeline).then(delegate(object o){
				
				// Still enqueued?
				if(!timeline.isEnqueued){
					p.reject(timeline);
					return;
				}
				
				// Catch the slides end event:
				timeline.addEventListener("timelineend",delegate(PowerSlide.SlideEvent se){
					
					// Resolve it now:
					p.resolve(timeline);
					
				});
				
				// Catch the slides cancel event (called when it was quit early):
				timeline.addEventListener("timelinecancel",delegate(PowerSlide.SlideEvent se){
					
					// Resolve it now:
					p.reject(timeline);
					
				});
				
				// Start it now!
				timeline.start();
				
			},p);
			
			return p;
		}
		
		/// <summary>Cues the element to advance any paused PowerSlide slides.</summary>
		public bool cue(){
			
			// Get the computedStyle (without requiring a HtmlElement):
			ComputedStyle cs=(this as IRenderableNode).ComputedStyle;
			
			// Get the PowerSlide (if any) for that CS:
			Timeline slides=Timeline.get(cs);
			
			if(slides!=null){
				// Great - cue it!
				return slides.cue();
			}
			
			return false;
			
		}
		
	}
	
}