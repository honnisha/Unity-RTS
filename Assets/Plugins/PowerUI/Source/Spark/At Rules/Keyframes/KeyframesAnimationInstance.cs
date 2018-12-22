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


namespace Css{
	
	/// <summary>
	/// An instance of an animation, running on a particular element.
	/// </summary>
	
	public class KeyframesAnimationInstance{
		
		/// <summary>The delay before this animation plays, in ms.</summary>
		public int Delay;
		/// <summary>Currently running backwards?</summary>
		public bool Backwards;
		/// <summary>The amount this animation should repeat.</summary>
		public int RepeatCount=1;
		/// <summary>The style of the element this is on.</summary>
		public ComputedStyle Style;
		/// <summary>The PowerUI animation.</summary>
		public UIAnimation Animation;
		/// <summary>Started yet?</summary>
		public bool Started;
		/// <summary>The duration of this animation, in s.</summary>
		public float Duration;
		/// <summary>Is this animation paused?</summary>
		public bool Paused;
		/// <summary>The fillmode for this animation.</summary>
		public KeyframesFill FillMode=KeyframesFill.None;
		/// <summary>The timing function to use.</summary>
		public Css.Value TimingFunction;
		/// <summary>The raw animation info.</summary>
		public KeyframesRule RawAnimation;
		/// <summary>The direction to run this animation in.</summary>
		public KeyframesAnimationDirection Direction=KeyframesAnimationDirection.Forward;
		/// <summary>The current frame ID.</summary>
		public int CurrentFrame;
		
		
		public KeyframesAnimationInstance(ComputedStyle style,KeyframesRule anim){
			
			Style=style;
			RawAnimation=anim;
			
		}
		
		/// <summary>Starts this instance.</summary>
		public void Start(){
			
			if(Paused || Started || RawAnimation==null){
				// Block start request.
				return;
			}
			
			Started=true;
			
			// Start running backwards?
			Backwards=( ((int)Direction & 1) == 1);
			
			// Set the current frame:
			if(Backwards){
				CurrentFrame=RawAnimation.FrameCount-1;
			}else{
				CurrentFrame=0;
			}
			
			AnimationEvent ae=new AnimationEvent("animationstart");
			ae.animationName=RawAnimation.Name;
			ae.SetTrusted(false);
			Style.Element.dispatchEvent(ae);
			
			Run();
			
		}
		
		/// <summary>Runs the current frame.</summary>
		private void Run(){
			
			// Get the frame:
			KeyframesKeyframe frame=RawAnimation.GetFrame(CurrentFrame);
			
			float duration;
			
			if(Backwards){
				duration=Duration * frame.AfterDelay;
			}else{
				duration=Duration * frame.BeforeDelay;
			}
			
			// The function to use:
			Blaze.VectorPath path=null;
			
			if(TimingFunction!=null){
				
				// Get the defined vector path:
				path=TimingFunction.GetPath(
					Style.RenderData,
					Css.Properties.AnimationTimingFunction.GlobalProperty
				);
			
			}
			
			if(path==null){
				
				// Use ease:
				path=Css.Keywords.Ease.SharedPath;
				
			}
			
			// Create the UI animation:
			Animation=new UIAnimation(Style.Element,frame.Style,duration,path);
			
			// Add ondone callback:
			Animation.OnDone(Advance);
			
		}
		
		/// <summary>Called when a cycle is completed.</summary>
		private void CompletedCycle(){
			
			// Can we repeat? -1 is infinite.
			if(RepeatCount!=-1){
				
				RepeatCount--;
				
				// Got to stop?
				if(RepeatCount==0){
					Stop(true);
					return;
				}
			}
			
			AnimationEvent ae=new AnimationEvent("animationiteration");
			ae.animationName=RawAnimation.Name;
			ae.elapsedTime=Duration * RepeatCount;
			ae.SetTrusted(false);
			Style.Element.dispatchEvent(ae);
			
			// If alternate, flip the direction:
			if( ((int)Direction & 2) == 2){
				Backwards=!Backwards;
			}else{
				// Start running backwards?
				Backwards=( ((int)Direction & 1) == 1);
			}
			
			// Set the current frame:
			if(Backwards){
				CurrentFrame=RawAnimation.FrameCount-1;
			}else{
				CurrentFrame=0;
			}
			
			Run();
		}
		
		/// <summary>Go to the next frame.</summary>
		public void Advance(UIAnimation anim){
			
			if(Backwards){
				CurrentFrame--;
				
				if(CurrentFrame<0){
					CompletedCycle();
					return;
				}
				
			}else{
				CurrentFrame++;
				
				if(CurrentFrame>=RawAnimation.FrameCount){
					CompletedCycle();
					return;
				}
			}
			
			Run();
			
		}
		
		/// <summary>Halts this instance.</summary>
		public void Stop(bool runEvent){
			
			if(!Started){
				return;
			}
			
			if(runEvent){
				
				AnimationEvent ae=new AnimationEvent("animationend");
				ae.animationName=RawAnimation.Name;
				ae.elapsedTime=Duration * RepeatCount;
				ae.SetTrusted(false);
				Style.Element.dispatchEvent(ae);
				
			}
			
			// Clear host:
			if(Style.AnimationInstance==this){
				Style.AnimationInstance=null;
			}
			
			Started=false;
			
			if(Animation!=null){
				// Halt the animation (never call OnDone here):
				Animation.Stop(false);
				Animation=null;
			}
			
		}
		
		/// <summary>Changes the paused state of this animation.</summary>
		public void SetPause(bool value){
			Paused=value;
			
			if(Animation!=null){
				Animation.Paused=value;
			}
			
			if(!value){
				Start();
			}
		}
		
	}
	
}