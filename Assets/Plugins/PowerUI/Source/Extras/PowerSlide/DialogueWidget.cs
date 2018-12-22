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


namespace Widgets{
	
	/// <summary>
	/// A widget which manages dialogue (speech). Used by PowerSlide.
	/// They typically display dialogue, but doing so isn't required.
	/// I.e. they can manage a collection of WorldUI's instead.
	/// </summary>
	
	public class DialogueWidget : Widget{
		
		/// <summary>The timeline in use</summary>
		public PowerSlide.Timeline timeline;
		
		
		/// <summary>A click to continue helper. This entirely ignores the event if an option is on the UI.
		/// Otherwise, it simply acts the same as Widgets.Widget.Cue</summary>
		[Values.Preserve]
		public static void ClickToContinue(PowerUI.MouseEvent e){
			
			// The widget:
			Widgets.Widget widget=e.htmlTarget.widget;
			
			// Is it a dialogue widget?
			DialogueWidget dw=widget as DialogueWidget;
			
			PowerSlide.Timeline tl;
			
			if(dw!=null){
				
				// Get the timeline:
				tl=dw.timeline;
				
				// Is there an option on the UI at the moment?
				if(dw.hasActiveOption){
					// Yes - ignore:
					return;
				}
				
			}else{
				
				// Get the timeline:
				tl=PowerSlide.Timeline.get(widget);
				
			}
			
			// Cue it:
			if(tl!=null){
				tl.cue();
			}
			
		}
		
		/// <summary>Occurs when an option is clicked.</summary>
		[Values.Preserve]
		public static void RunOption(PowerUI.MouseEvent e){
			
			Dom.Node targetNode=(e.target as Dom.Node);
			
			// Get the unique ID:
			int uniqueId;
			if(!int.TryParse(targetNode.getAttribute("unique-id"),out uniqueId)){
				Dom.Log.Add("A dialogue option did not have an unique-id attribute.");
				return;
			}
			
			// Get the widget:
			DialogueWidget dw=e.targetWidget as DialogueWidget;
			
			if(dw==null){
				Dom.Log.Add("A dialogue option tried to run but it's not inside a DialogueWidget.");
				return;
			}
			
			// Great ok - we can now grab the active options entry.
			// Select the active slide with that unique ID:
			PowerSlide.DialogueSlide slide=dw.getSlide(uniqueId) as PowerSlide.DialogueSlide;
			
			if(slide==null){
				Dom.Log.Add("Unable to resolve a DialogueSlide from a unique-id.");
				return;
			}
			
			// Get the GOTO:
			string gotoUrl=slide.slidesToGoTo;
			
			if(string.IsNullOrEmpty(gotoUrl)){
				
				// Acts just like a continue does.
				// Just cue it:
				dw.timeline.cue();
				return;
				
			}
			
			// Load it now (into the existing timeline):
			PowerSlide.Timeline.open(gotoUrl,PowerSlide.Dialogue.basePath,dw.timeline).then(delegate(object o){
				
				// Successfully opened it! Should already be running, but just incase:
				PowerSlide.Timeline timeline=o as PowerSlide.Timeline;
				
				// Start:
				timeline.start();
				
			},delegate(object failure){
				
				// Failed!
				Dom.Log.Add("Failed to load a timeline from an option URI: "+failure);
				
			});
			
			// dw.timeline.document.startDialogue(gotoUrl,dw.Timeline.template);
			
			// Kill the event:
			e.stopPropagation();
			
		}
		
		/// <summary>True if there is an option actively on this dialogue widget.</summary>
		public bool hasActiveOption{
			get{
				
				if(timeline==null){
					return false;
				}
				
				// Check running slides:
				PowerSlide.Slide current=timeline.firstRunning;
				
				while(current!=null){
					
					PowerSlide.DialogueSlide ds=current as PowerSlide.DialogueSlide;
					
					if(ds!=null){
						
						if(ds.isOptions){
							return true;
						}
						
					}
					
					current=current.nextRunning;
				}
				
				return false;
			}
		}
		
		/// <summary>A convenience function for setting up onmousedown and unique-id attributes.
		/// onmousedown points at Widgets.DialogueWidget.RunOption.</summary>
		protected string OptionMouseDown(PowerSlide.Slide option){
			
			return "onmousedown='Widgets.DialogueWidget.RunOption' unique-id='"+option.uniqueID+"'";
			
		}
		
		/// <summary>Gets an active slide by its unique ID.</summary>
		public PowerSlide.Slide getSlide(int uniqueID){
			return timeline.getSlide(uniqueID);
		}
		
		/// <summary>Gets all dialogue slides which are currently active.</summary>
		public List<PowerSlide.DialogueSlide> allActive{
			get{
				return timeline.getActive<PowerSlide.DialogueSlide>();
			}
		}
		
		/// <summary>Called when the template is ready.</summary>
		public override void Goto(string url,Dictionary<string,object> globals){
			
			// Just get the timeline:
			object timelineObj;
			if(globals.TryGetValue("timeline",out timelineObj)){
				
				// Get the timeline:
				timeline=timelineObj as PowerSlide.Timeline;
				
			}
			
		}
		
		/// <summary>The depth that this type of widget lives at.</summary>
		public override int Depth{
			get{
				return 1100;
			}
		}
		
		/// <summary>Handles events on the widget itself.</summary>
		protected override void OnEvent(Dom.Event e){
			
			// Catch PowerSlide dialogue events and convert them to our virtual methods:
			PowerSlide.SlideEvent se;
			
			if(e.type=="dialoguestart"){
				
				// It's a slide event:
				se=e as PowerSlide.SlideEvent;
				
				// Get the slide:
				Show(se.slide as PowerSlide.DialogueSlide);
				
			}else if(e.type=="dialogueend"){
				
				// It's a slide event:
				se=e as PowerSlide.SlideEvent;
				
				// Hide:
				Hide(se.slide as PowerSlide.DialogueSlide);
				
			}else if(e.type=="timelinepause"){
				
				// The slides just paused (and are now waiting for a cue)
				se=e as PowerSlide.SlideEvent;
				
				// Waiting for a cue:
				WaitForCue(se);
				
			}else if(e.type=="timelineplay"){
				
				// The slides just cued (and are now continuing).
				se=e as PowerSlide.SlideEvent;
				
				// Cue received:
				Cued(se);
				
			}
			
			base.OnEvent(e);
			
		}
		
		/// <summary>Called when the dialogue is now waiting for a cue event.</summary>
		protected virtual void WaitForCue(PowerSlide.SlideEvent e){
			
		}
		
		/// <summary>Called when the dialogue got cued.</summary>
		protected virtual void Cued(PowerSlide.SlideEvent e){
			
		}
		
		/// <summary>Called when the given slide requested to display.
		/// Note that multiple slides may request to be visible at the same time.</summary>
		protected virtual void Show(PowerSlide.DialogueSlide dialogue){
			
		}
		
		/// <summary>Called when the given slide requested to hide.
		/// Note that there may be multiple visible slides.</summary>
		protected virtual void Hide(PowerSlide.DialogueSlide dialogue){
			
		}
		
	}
	
	public partial class Widget{
		
		/// <summary>Cues dialogue within a widget.</summary>
		[Values.Preserve]
		public static void Cue(PowerUI.MouseEvent me){
			
			// The widget:
			Widgets.Widget widget=me.htmlTarget.widget;
			
			// Get the timeline:
			PowerSlide.Timeline tl=PowerSlide.Timeline.get(widget);
			
			// Cue it:
			if(tl!=null){
				tl.cue();
			}
			
		}
		
	}
	
}