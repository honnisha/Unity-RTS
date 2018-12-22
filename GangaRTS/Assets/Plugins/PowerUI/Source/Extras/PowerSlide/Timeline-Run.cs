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
using Dom;
using PowerUI;
using Css;


namespace PowerSlide{
	
	/// <summary>
	/// Handles operating PowerSlide timelines.
	/// </summary>
	
	public partial class Timeline{
		
		/// <summary>The linked list of active timelines.</summary>
		public static Timeline first;
		/// <summary>The linked list of active timelines.</summary>
		public static Timeline last;
		/// <summary>Used to periodically call UpdateAll.
		/// Similar to a UITimer but instead this runs 
		/// on the Unity main thread and it's syncable with redraws.</summary>
		private static OnUpdateCallback updater;
		
		
		/// <summary>Stops all timelines running on the given style.</summary>
		public static void stopAll(ComputedStyle style){
			
			Timeline current=first;
			
			while(current!=null){
				
				if(current.style==style){
					// Interrupt it:
					current.stop(false);
				}
				
				current=current.after;
			}
			
		}
		
		/// <summary>Gets the current instance for the given element (null if none found).</summary>
		public static Timeline get(ComputedStyle style){
			
			Timeline current=first;
			
			while(current!=null){
				if(current.style==style){
					return current;
				}
				
				current=current.after;
			}
			
			return null;
			
		}
		
		/// <summary>Gets the current instance for the given widget (null if none found).</summary>
		public static Timeline get(Widgets.Widget widget){
			
			Timeline current=first;
			
			while(current!=null){
				
				if(current.currentWidget==widget){
					return current;
				}
				
				current=current.after;
			}
			
			return null;
			
		}
		
		/// <summary>Removes all active slides.</summary>
		public static void clear(){
			last=first=null;
		}
		
		/// <summary>Called at the UI update rate to progress the currently running slides.</summary>
		public static void updateAll(){
			
			// redraw frame?
			if(first==null || !UI.IsRedrawUpdate){
				return;
			}
			
			// Get the frame time:
			float frameTime=UI.CurrentFrameTime;
			
			Timeline current=first;
			
			while(current!=null){
				current.update(frameTime);
				current=current.after;	
			}
			
		}
		
		/// <summary>The duration of the whole thing. Overrides timeline.duration, if it has one.</summary>
		public float appliedDuration=float.MinValue;
		/// <summary>The "actual" duration - the one which considers timeline too.</summary>
		public float actualDuration;
		/// <summary>The current time in seconds that has passed since it started.</summary>
		public float currentTime;
		/// <summary>The sampler used when progressing.</summary>
		public Blaze.CurveSampler progressSampler;
		/// <summary>Current instances are stored in a linked list. This is the next one in the list.</summary>
		internal Timeline after;
		/// <summary>Current instances are stored in a linked list. This is the one before this in the list.</summary>
		internal Timeline before;
		/// <summary>The delay before this plays, in s.</summary>
		public float delay;
		/// <summary>Currently running backwards?</summary>
		public bool backwards;
		/// <summary>The amount this should repeat.</summary>
		public int repeatCount=1;
		/// <summary>The direction to run.</summary>
		public KeyframesAnimationDirection direction=KeyframesAnimationDirection.Forward;
		/// <summary>Started yet?</summary>
		public bool started;
		/// <summary>Is it paused?</summary>
		public bool paused;
		/// <summary>True if this instance is in the update queue.</summary>
		private bool enqueued;
		/// <summary>The first currently running slide (from any track).</summary>
		internal Slide firstRunning;
		/// <summary>The last currently running slide (from any track).</summary>
		internal Slide lastRunning;
		
		
		/// <summary>True if this instance is in the update queue.</summary>
		public bool isEnqueued{
			get{
				return enqueued;
			}
		}
		
		/// <summary>Gets all active slides of the given type.</summary>
		public List<T> getActive<T>() where T:Slide{
			
			// Create results:
			List<T> result=new List<T>();
			
			Slide current=firstRunning;
			
			while(current!=null){
				
				if(current is T){
					result.Add((T)current);
				}
				
				current=current.nextRunning;
			}
			
			return result;
		}
		
		/// <summary>True if the given slide is currently running.</summary>
		public bool isActive(Slide s){
			
			Slide current=firstRunning;
			
			while(current!=null){
				
				if(current==s){
					return true;
				}
				
				current=current.nextRunning;
			}
			
			return false;
		}
		
		/// <summary>Starts this instance.</summary>
		public void start(){
			
			if(paused || started || tracks==null){
				// Block start request.
				return;
			}
			
			started=true;
			status_=TIMELINE_STARTED;
			
			if(!enqueued){
				enqueue();
			}
			
		}
		
		/// <summary>Adds this instance to the update queue.</summary>
		public void enqueue(){
			
			if(enqueued){
				return;
			}
			
			enqueued=true;
			after=null;
			before=last;
			
			if(first==null){
				first=last=this;
				
				// Enqueue in the update system:
				updater=OnUpdate.Add(updateAll);
				
			}else{
				last.after=this;
				last=this;
			}
			
		}
		
		/// <summary>Advances this instance by the given amount.</summary>
		public void update(float deltaTime){
			
			if(!started || paused){
				// Awaiting data, usually.
				return;
			}
			
			if(currentTime==0f){
				
				if(deltaTime>0.5f){
					// Block slow frames.
					// This is almost always only ever the very first one
					return;
				}
				
				// Clear running:
				firstRunning=null;
				lastRunning=null;
				
				// Establish duration:
				float rawDuration=appliedDuration;
				
				if(rawDuration<0f){
					
					if(duration==null){
						
						// Infer from max end time, or '1' if there is none.
						rawDuration=maxDefinedDuration;
						
						if(rawDuration==0f){
							rawDuration=1f;
						}
						
					}else{
						rawDuration=duration.GetDecimal(style==null? null : style.RenderData,null);
					}
					
				}
				
				actualDuration=rawDuration;
				
				// Starting backwards?
				backwards=( ((int)direction & 1) == 1);
				
				// Update durations for each track:
				for(int i=0;i<tracks.Length;i++){
					Track track=tracks[i];
					
					if(track==null){
						continue;
					}
					
					track.onStart();
					
					// Set start/duration:
					track.setStartAndDuration(rawDuration);
					
					// Reset current index:
					if(backwards){
						track.currentSlide=track.slides.Length;
					}else{
						track.currentSlide=-1;
					}
					
				}
				
				// Dispatch start:
				dispatch("start");
				
				// Instant?
				if(actualDuration==0f){
					
					stop(true);
					
					return;
				}
				
			}
			
			// If we have a timing leader, then current time is..
			if(timingLeader!=null){
				
				currentTime=timingLeader.computedStart;
				
				if(timingLeader.timing!=null){
					
					// Get the leaders duration (just in case it has expired):
					float duration=timingLeader.timing.GetDuration();
					float current=timingLeader.timing.GetCurrentTime();
					
					currentTime+=current;
					
					if(duration!=-1f && current>=duration){
						
						// It's finished! Quit the timing leader:
						// (This occurs if the lead time is shorter than the slide's duration).
						timingLeader.endTimingLead();
						
					}
					
				}
				
			}else{
				currentTime+=deltaTime;
			}
			
			if(style!=null && !style.Element.isRooted){
				
				// Immediately stop - the element was removed (don't call the finished event):
				stop(false);
				
				return;
				
			}
			
			// Set ActiveValue by sampling from the curve (if there is one):
			
			if(progressSampler!=null){
				
				// Map through the progression curve:
				progressSampler.Goto(currentTime / actualDuration,true);
				currentTime=progressSampler.CurrentValue * actualDuration;
				
			}
			
			// Advance all tracks to the frame at 'progress'.
			for(int i=0;i<tracks.Length;i++){
				
				// Get the track:
				Track track=tracks[i];
				
				if(track==null || track.slides==null || track.slides.Length==0){
					continue;
				}
				
				int length=track.slides.Length;
				
				int index=track.currentSlide;
				
				if(backwards){
					index--;
				}else{
					index++;
				}
				
				while( (backwards && index>=0) || (!backwards && index<length) ){
					
					// Get the slide:
					Slide slideToStart=track.slides[index];
					
					if(slideToStart.ignore){
						// Skip:
						if(backwards){
							index--;
						}else{
							index++;
						}
						
						continue;
						
					}
					
					// Startable?
					if(backwards){
						
						if((1f-slideToStart.computedEnd)>=currentTime){
							// Nope!
							break;
						}
						
					}else if(slideToStart.computedStart>=currentTime){
						// Nope!
						break;
					}
					
					// Add to queue:
					slideToStart.nextRunning=null;
					slideToStart.previousRunning=lastRunning;
					
					if(firstRunning==null){
						firstRunning=lastRunning=slideToStart;
					}else{
						lastRunning.nextRunning=slideToStart;
						lastRunning=slideToStart;
					}
					
					// Start it now:
					slideToStart.start();
					
					// Next:
					track.currentSlide=index;
					
					if(paused){
						return;
					}
					
					if(backwards){
						index--;
					}else{
						index++;
					}
					
				}
				
			}
			
			// Kill any slides which are now done:
			endDoneSlides();
			
			if(currentTime >= actualDuration){
				
				// Done!
				completedCycle();
				
			}
			
		}
		
		internal void endDoneSlides(){
			
			// Kill any slides which are now done:
			Slide current=firstRunning;
			
			while(current!=null){
				
				current.endIfDone(backwards,currentTime);
				
				current=current.nextRunning;
			}
			
		}
		
		/// <summary>Called when a cycle is completed.</summary>
		private void completedCycle(){
			
			// Can we repeat? -1 is infinite.
			if(repeatCount!=-1){
				
				repeatCount--;
				
				// Got to stop?
				if(repeatCount==0){
					stop(true);
					return;
				}
				
			}
			
			dispatch("iteration");
			
			// If alternate, flip the direction:
			if( ((int)direction & 2) == 2){
				backwards=!backwards;
			}else{
				// Start running backwards?
				backwards=( ((int)direction & 1) == 1);
			}
			
			// Set the current frame:
			for(int i=0;i<tracks.Length;i++){
				
				Track track=tracks[i];
				
				if(track==null){
					return;
				}
				
				if(backwards){
					track.currentSlide=track.slides.Length;
				}else{
					track.currentSlide=-1;
				}
				
			}
			
			// Run again:
			currentTime=0f;
			
		}
		
		/// <summary>Cues this timeline (unpauses it).</summary>
		public bool cue(){
			return setPause(false);
		}
		
		/// <summary>Changes the paused state of this animation.</summary>
		public bool setPause(bool value){
			
			if(paused==value){
				return false;
			}
			
			if(value){
				dispatch("pause");
			}else{
				dispatch("play");
			}
			
			paused=value;
			
			Slide s=firstRunning;
			
			while(s!=null){
				
				// Pause each slide:
				s.setPause(value);
				s=s.nextRunning;
				
			}
			
			// Quit any slides that are now done:
			if(!value){
				
				// Kill any slides which are now done:
				endDoneSlides();
				
			}
			
			// Clear cues:
			clearCues();
			
			if(!value){
				start();
			}
			
			return true;
		}
		
		/// <summary>
		/// Sets a time curve which progresses the overall slides animation.
		/// Almost always linear but change it for advanced effects.</summary>
		public void setTimingFunction(Css.Value timingFunc){
			
			// The function to use:
			Blaze.VectorPath path=null;
			
			if(timingFunc!=null){
				
				// Get the defined vector path:
				path=timingFunc.GetPath(
					style==null? null : style.RenderData,
					Css.Properties.TimelineTimingFunction.GlobalProperty
				);
				
			}
			
			if(path==null){
				progressSampler=null;
			}else{
				
				if(!(path is Blaze.RasterVectorPath)){
					
					Blaze.RasterVectorPath rvp=new Blaze.RasterVectorPath();
					path.CopyInto(rvp);
					rvp.ToStraightLines();
					path=rvp;
					
				}
				
				progressSampler=new Blaze.CurveSampler(path);
				progressSampler.Reset();
			}
			
		}
		
		/// <summary>Dispatches an event of the given name.</summary>
		public void dispatch(string name){
			SlideEvent se=new SlideEvent("timeline"+name,null);
			se.timeline=this;
			se.SetTrusted(true);
			dispatchEvent(se);
		}
		
		/// <summary>Dispatch an event.</summary>
		protected override bool HandleLocalEvent(Event e,bool bubblePhase){
			
			if(style!=null){
				style.Element.dispatchEvent(e);
			}else if(document!=null){
				document.dispatchEvent(e);
			}
			
			if(currentWidget!=null){
				currentWidget.dispatchEvent(e);
			}
			
			return base.HandleLocalEvent(e,bubblePhase);
			
		}
		
		/// <summary>Stops this instance, optionally firing an event.</summary>
		public void stop(bool fireEvent){
			
			if(!enqueued){
				return;
			}
			
			enqueued=false;
			
			if(before==null){
				first=after;
			}else{
				before.after=after;
			}
			
			if(after==null){
				last=before;
			}else{
				after.before=before;
			}
			
			// Reset:
			reset();
			
			if(fireEvent){
				status_=TIMELINE_ENDED;
				dispatch("end");
			}else{
				status_=TIMELINE_CANCELLED;
				dispatch("cancel");
			}
			
		}
		
	}
	
}