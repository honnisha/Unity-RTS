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
	/// A slide. Contains e.g. the text spoken or the style to apply.
	/// A 'track' is a list of these slides.
	/// </summary>
	
	public partial class Slide : EventTarget{
		
		/// <summary>A global slide ID. Used to obtain a slide from a click event.</summary>
		private static int UniqueID_=1;
		
		/// <summary>Use this and partial class extensions to add custom info loaded from JSON.</summary>
		public static event Action<SlideEvent> OnLoad;
		
		/// <summary>
		/// A unique ID (locally). Used to obtain a slide from e.g. a click event.
		/// </summary>
		internal int uniqueID;
		/// <summary>
		/// True if this slide should be entirely ignored.</summary>
		public bool ignore;
		/// <summary>The json the slide originated from.</summary>
		public JSObject rawJson;
		/// <summary>
		/// The index of this slide in its track. Note that these aren't unique.
		/// </summary>
		public int index;
		/// <summary>
		/// The track this slide belongs to.
		/// </summary>
		public Track track;
		/// <summary>
		/// The timing leader for this slide (if it has one).
		/// </summary>
		public ITimingLeader timing;
		/// <summary>Specified start value.</summary>
		public Css.Value startValue;
		/// <summary>Specified duration value.</summary>
		public Css.Value durationValue;
		/// <summary>The computed start value.</summary>
		public float computedStart=0f;
		/// <summary>The computed duration.</summary>
		public float computedDuration=0f;
		/// <summary>
		/// Actions to trigger.
		/// </summary>
		public Action[] actions;
		/// <summary>The linked list of running slides.</summary>
		internal Slide nextRunning;
		/// <summary>The linked list of running slides.</summary>
		internal Slide previousRunning;
		
		
		/// <summary>The computed end value.</summary>
		public float computedEnd{
			get{
				return (computedStart + computedDuration);
			}
		}
		
		/// <summary>The document that this target belongs to.</summary>
		internal override Document eventTargetDocument{
			get{
				return track.eventTargetDocument;
			}
		}
		
		public Slide(){
			uniqueID=UniqueID_++;
		}
		
		/// <summary>Gets a slide by a unique ID.</summary>
		public virtual Slide getSlideByID(int uniqueID){
			
			if(this.uniqueID==uniqueID){
				return this;
			}
			
			return null;
			
		}
		
		/// <summary>Ends this slides timing lead.</summary>
		public void endTimingLead(){
			
			if(track.timeline.timingLeader==this){
				
				// Clear the leader:
				track.timeline.timingLeader=null;
				
			}
			
			if(timing!=null){
				timing.Stop();
				timing=null;
			}
			
		}
		
		/// <summary>The timeline will now have its timing lead by the given leader.</summary>
		public void timingLeadBy(ITimingLeader leader){
		
			// Set the leader:
			timing=leader;
			
			// Apply this slide to the timeline:
			track.timeline.timingLeader=this;
			
		}
		
		/// <summary>This slide is now done.</summary>
		internal virtual void end(){
			
			// Dispatch a "slideend" event.
			SlideEvent se=createEvent("end");
			
			// Dispatch here:
			dispatchEvent(se);
			
			// Dispatch to the element too:
			EventTarget et=eventTarget;
			
			if(et!=null){
				et.dispatchEvent(se);
			}
			
			// Quit timing lead:
			endTimingLead();
			
		}
		
		/// <summary>The event target to use.</summary>
		public EventTarget eventTarget{
			get{
				Element e=element;
				
				if(e==null){
					return timeline.document;
				}
				
				return e;
			}
		}
		
		/// <summary>The element which the timeline is running on.</summary>
		public Element element{
			get{
				if(timeline.style==null){
					return null;
				}
				
				return timeline.style.Element as Element;
			}
		}
		
		/// <summary>This slide is now starting.</summary>
		internal virtual void start(){
			
			// Dispatch a "timelinestart" event.
			SlideEvent se=createEvent("start");
			
			// Dispatch here:
			dispatchEvent(se);
			
			// Dispatch to the element too:
			EventTarget et=eventTarget;
			
			if(et!=null){
				et.dispatchEvent(se);
			}
			
		}
		
		/// <summary>The ID of the track that this slide belongs to.</summary>
		public int trackID{
			get{
				return track.id;
			}
		}
		
		/// <summary>Cues this slide right now.</summary>
		public void cue(){
			
			// Create the cue event:
			SlideEvent cue=createEvent("cue");
			
			// Run it now:
			if(dispatchEvent(cue)){
				
				// Cue the text for each speaker.
				
				
				// Cue all actions (always after text):
				if(actions!=null){
					
					// Param set:
					object[] arr=new object[]{cue};
					
					// Cue! Run each action now:
					for(int i=0;i<actions.Length;i++){
						
						// Get it:
						Action a=actions[i];
						
						// Update event:
						cue.action=a;
						
						// Invoke it:
						a.Method.Invoke(null,arr);
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Ends this slide if it's done.</summary>
		internal void endIfDone(bool backwards,float progress){
		
			float endAt=backwards ? (1f-computedStart) : computedEnd;
			
			if(endAt<=progress){
				
				// Remove from running:
				if(nextRunning==null){
					timeline.lastRunning=previousRunning;
				}else{
					nextRunning.previousRunning=previousRunning;
				}
				
				if(previousRunning==null){
					timeline.firstRunning=nextRunning;
				}else{
					previousRunning.nextRunning=nextRunning;
				}
				
				// Done! This can "pause" to make it wait for a cue.
				end();
				
			}
			
		}
		
		/// <summary>The timeline that this slide is in.</summary>
		public Timeline timeline{
			get{
				return track.timeline;
			}
		}
		
		/// <summary>True if this slide has had start called but not end.
		/// I.e. it's actively running.</summary>
		public bool isActive{
			get{
				return timeline.isActive(this);
			}
		}
		
		/// <summary>Called when the timeline is paused/ resumed and this slide is running.</summary>
		internal virtual void setPause(bool paused){}
		
		/// <summary>Loads a slide from the given JSON.</summary>
		public virtual void load(JSObject json){
			
			// Start:
			string startText=json.String("start");
			
			if(startText!=null){
				// Load the start value:
				startValue=Css.Value.Load(startText);
			}
			
			// Duration:
			string durationText=json.String("duration");
			
			if(durationText!=null){
				// Load the duration value:
				durationValue=Css.Value.Load(durationText);
			}
			
			// Action:
			JSArray acts=json["actions"] as JSArray;
			
			if(acts!=null){
				
				if(acts.IsIndexed){
					
					// Multiple actions:
					
					// For each one..
					foreach(KeyValuePair<string,JSObject> kvp in acts.Values){
						
						// index is..
						int index;
						int.TryParse(kvp.Key,out index);
						
						// Set it up:
						loadAction(index,kvp.Value);
						
					}
					
				}else{
					
					// Should be an array but we'll also accept just one.
					loadAction(0,acts);
					
				}
				
			}else{
				
				// Check if they mis-named as just 'action':
				acts=json["action"] as JSArray;
				
				if(acts!=null){
					
					loadAction(0,acts);
					
				}
				
			}
			
			rawJson=json;
			
			if(OnLoad!=null){
				
				// Dispatch the load event which enables any custom info being added:
				SlideEvent de=createEvent("load");
				OnLoad(de);
				
			}
			
		}
		
		/// <summary>Sets up an action at the given index in the Actions set.</summary>
		internal void loadAction(int index,JSObject data){
			
			if(actions==null){
				actions=new Action[1];
			}
			
			// Create and add:
			Action a=new Action(this,data);
			actions[index]=a;
			
		}
		
		/// <summary>Creates an event relative to this slide.</summary>
		public SlideEvent createEvent(string type){
			
			SlideEvent de=track.createRawEvent("slide"+type);
			de.slide=this;
			return de;
			
		}
		
	}
	
}