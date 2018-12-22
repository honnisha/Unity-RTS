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
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// The AnimationCompleted delegate is an alternative to using a Nitro callback when the animation is done.
	/// Used with the OnDone method.
	/// </summary>
	public delegate void AnimationCompleted(UIAnimation animation);
		
	/// <summary>
	/// Handles all actively animated css properties. Also keeps track of a single set
	/// of css properties as used in any animate method (<see cref="PowerUI.HtmlElement.animate"/>)
	/// and can be used to monitor the progress of an animation.
	/// </summary>
	
	public class UIAnimation : EventTarget{
		
		/// <summary>Actively animated properties are stored in a linked list. This is the tail of that list.</summary>
		public static AnimatedProperty LastProperty;
		/// <summary>Actively animated properties are stored in a linked list. This is the head of that list.</summary>
		public static AnimatedProperty FirstProperty;
		
		/// <summary>Removes all active animations.</summary>
		public static void Clear(){
			LastProperty=FirstProperty=null;
		}
		
		/// <summary>Called at the UI update rate to progress the currently animated properties.</summary>
		public static void Update(float frameTime){
			
			if(FirstProperty==null){
				return;
			}
			
			AnimatedProperty current=FirstProperty;
			
			while(current!=null){
				current.Update(frameTime);
				current=current.PropertyAfter;	
			}
			
		}
		
		/// <summary>Searches the current animated properties for the named property on the given element.</summary>
		/// <param name="animating">The element being animated.</param>
		/// <param name="property">The CSS property to look for. Note: Must not be a composite property such as color-overlay.
		/// Must be a full property such as color-overlay-r.</param>
		/// <returns>An AnimatedProperty if it was found; Null otherwise.</returns>
		public static AnimatedProperty GetAnimatedProperty(Node animating,string property){
			// Get the property:
			return GetAnimatedProperty(animating,CssProperties.Get(property));
		}
				
		/// <summary>Searches the current animated properties for the named property on the given element.</summary>
		/// <param name="animating">The element being animated.</param>
		/// <param name="property">The CSS property to look for.</param>
		/// <returns>An AnimatedProperty if it was found; Null otherwise.</returns>
		public static AnimatedProperty GetAnimatedProperty(Node animating,CssProperty property){
			
			if(FirstProperty==null){
				return null;
			}
			
			AnimatedProperty current=FirstProperty;
			
			while(current!=null){
				if(current.Animating==animating && current.InnerPropertyInfo==property){
					return current;
				}
				
				current=current.PropertyAfter;
			}
			
			return null;
		}
		
		/// <summary>A custom data object.</summary>
		public object Data;
		/// <summary>The total time in seconds that this animation lasts for.</summary>
		public float TotalTime{
			get{
				return Duration;
			}
		}
		/// <summary>The node being animated.</summary>
		public Node Animating;
		/// <summary>The CS for the element being animated.</summary>
		public ComputedStyle ComputedStyle;
		/// <summary>True if this animation has finished; false otherwise.</summary>
		private bool FinishedPlaying;
		/// <summary>The time in seconds that the animation will run for.</summary>
		public float Duration;
		/// <summary>True if this animation is paused.</summary>
		public bool Paused;
		/// <summary>A curve which describes the progression of the animation. 
		/// x is time, y is value. Usually (0,0) to (1,1).</summary>
		public Blaze.VectorPath TimeCurve;
		/// <summary>All current animations are stored in a linked list. This is the next one.</summary>
		public UIAnimation NextAnimation;
		/// <summary>A promise which fires when the animation finished (you can also add an event handler for onfinish).</summary>
		private Promise _finished;
		
		
		/// <summary>Creates a new UIAnimation for animating CSS and immediately animates it.
		/// See <see cref="PowerUI.HtmlElement.animate"/>.</summary>
		/// <param name="animating">The element being animated.</param>
		/// <param name="properties">The CSS property string. Each property should define the value it will be when the animation is done.</param>
		/// <param name="duration">How long this animation lasts for.</param>
		/// <param name="timeCurve">A curve used to describe animation progression.</param>
		public UIAnimation(Node animating,string properties,float duration,Blaze.VectorPath timeCurve){
			
			if(string.IsNullOrEmpty(properties)){
				Dom.Log.Add("No properties given to animate!");
				return;
			}
			
			Style style=null;
			
			if( !properties.Contains(":") ){
				
				// Targeting a selector, e.g. #fadedBox
				// First, get the selector style:
				Style selector=(animating.document as ReflowDocument).getStyleBySelector(properties);
				
				if(selector==null){
					return;
				}
				
				// Animate each property:
				style=selector;
				
			}else{
				
				// Create a holding style:
				style=Style.Create(properties,animating);
				
			}
			
			// Animate with it:
			Setup(animating,style,duration,timeCurve);
		}
		
		/// <summary>Creates an animation with the given style as the set of target properties.</summary>
		public UIAnimation(Node animating,Style properties,float duration,Blaze.VectorPath timeCurve){
			
			Setup(animating,properties,duration,timeCurve);
			
		}
		
		/// <summary>Sets up general information for a particular animation and starts running it.</summary>
		private void Setup(Node animating,Style style,float duration,Blaze.VectorPath timeCurve){
			
			Animating=animating;
			ComputedStyle=(animating as IRenderableNode).ComputedStyle;
			
			if(duration<=0f){
				duration=0f;
			}
			
			Duration=duration;
			TimeCurve=timeCurve;
			
			// Start animating the properties now.
			
			foreach(KeyValuePair<CssProperty,Css.Value> kvp in style.Properties){
				
				// Get the value:
				Css.Value value=kvp.Value;
				
				// Get the property:
				CssProperty property=kvp.Key;
				
				// Grab the type:
				Css.ValueType type=value.Type;
				
				// Animate it (note that we don't need to copy it):
				if(property==Css.Properties.TransformProperty.GlobalProperty){
					
					// Special case for animating transform.
					AnimateTransform(value);
					
				}else if(type==Css.ValueType.Set){
					
					int count=value.Count;
					
					for(int i=0;i<count;i++){
						
						// Get the correct aliased property and animate it:
						Animate(property.GetAliased(i,true),value[i],(i==(count-1)));
						
					}
					
				}else{
					Animate(property,value,true);
				}
				
			}
			
		}
		
		/// <summary>A promise which fires when the animation finished.</summary>
		public Promise finished{
			get{
				if(_finished == null){
					_finished = new Promise();
				}
				return _finished;
			}
		}
		
		/// <summary>Resumes the animation.</summary>
		public void play(){
			Paused = false;
		}
		
		/// <summary>pauses the animation.</summary>
		public void pause(){
			Paused = true;
		}
		
		/// <summary>Cancels the animation.</summary>
		public void cancel(){
			Stop(false);
		}
		
		/// <summary>Current time of the animation in ms.
		public float? currentTime{
			get{
				if(FirstProperty == null){
					return null;
				}
				return FirstProperty.CurrentTime * 1000f;				
			}
		}
		
		/// <summary>Special case when animating CSS transform.</summary>
		private void AnimateTransform(Css.Value value){
			
			// Check if this property is already animated - if so, interrupt it and override with our new values.
			// There won't be many actively animated properties, so looping through the update queue is fast anyway.
			CssProperty property=Css.Properties.TransformProperty.GlobalProperty;
			
			AnimatedTransformProperty animProperty=GetAnimatedProperty(Animating,property) as AnimatedTransformProperty;
			
			bool isNew=(animProperty==null);
			
			if(isNew){
				
				// Create it now:
				animProperty=new AnimatedTransformProperty(this);
				
				// Add to queue:
				animProperty.AddToQueue();
				
			}
			
			animProperty.Animate(this,value,TimeCurve,true);
			
		}
		
		/// <summary>Called when this animation is finished.</summary>
		public void Finished(){
			if(FinishedPlaying){
				return;
			}
			
			FinishedPlaying=true;
			
			try{
				
				AnimationEvent e = new AnimationEvent("finish");
				dispatchEvent(e);
				
				if(_finished!= null){
					_finished.resolve(this);
				}
				
			}catch(Exception e){
				Dom.Log.Add("Error running animation finish method: "+e);
			}
			
		}
		
		/// <summary>Starts animating the named property and target value.</summary>
		/// <param name="property">The property to update. May be an alias or composite.</param>
		/// <param name="value">The target value of the property.</param>
		/// <param name="updateCss">True if this property should update CSS/ the screen when it's progressed.</param>
		private void Animate(CssProperty property,Css.Value value,bool updateCss){
			
			// Check if this property is already animated - if so, interrupt it and override with our new values.
			// There won't be many actively animated properties, so looping through the update queue is fast anyway.
			AnimatedProperty animProperty=GetAnimatedProperty(Animating,property);
			
			if(animProperty!=null){
				animProperty.Animate(this,value,TimeCurve,updateCss);
				
			}else{
				// Otherwise we want to create one or more AnimatedProperties and stick them into the queue.
				
				// Get or create the initial value:
				Css.Value hostValue;
				Css.Value rawValue=property.GetOrCreateValue(Animating,ComputedStyle,false,out hostValue);
				
				if(rawValue is Css.ValueSet){
					
					// A special case is when animating a value set. Each one needs to actually animate separately.
					// E.g. padding:40px 20px; then animating to just padding:30px;
					
					for(int i=0;i<rawValue.Count;i++){
						
						// Create it now:
						animProperty=new AnimatedProperty(this,property.GetAliased(i,true));
						
						// Setup the value:
						animProperty.SetupValue(hostValue,rawValue[i]);
						
						// Get the target value:
						Css.Value target=(value is Css.ValueSet) ? value[i] : value;
						
						// Animate it now:
						animProperty.Animate(this,target,TimeCurve,updateCss);
						
						animProperty.AddToQueue();
						
					}
					
				}else{ 
					
					// Create it now:
					animProperty=new AnimatedProperty(this,property);
					
					// Setup the value:
					animProperty.SetupValue(hostValue,rawValue);
					
					// Animate it now:
					animProperty.Animate(this,value,TimeCurve,updateCss);
					
					animProperty.AddToQueue();
					
				}
				
			}
		}
		
		/// <summary>Animation finished event.</summary>
		public Action<AnimationEvent> onfinish{
			get{
				return GetFirstDelegate<Action<AnimationEvent>>("finish");
			}
			set{
				addEventListener("finish",new EventListener<AnimationEvent>(value));
			}
		}
		
		/// <summary>Animation cancelled event.</summary>
		public Action<AnimationEvent> oncancel{
			get{
				return GetFirstDelegate<Action<AnimationEvent>>("cancel");
			}
			set{
				addEventListener("cancel",new EventListener<AnimationEvent>(value));
			}
		}
		
		/// <summary>Calls the given delegate when the animation is over.</summary>
		public void OnDone(AnimationCompleted onComplete){
			addEventListener("finish", new EventListener<AnimationEvent>((AnimationEvent e) => onComplete(this) ));
		}
		
		/// <summary>Call this to halt the animation early. This internally will cause the finished event to run.</summary>
		public void Stop(){
			Stop(true);
		}
		
		/// <summary>Call this to halt the animation early.</summary>
		/// <param name="runEvent">Optionally run the finished event if there is one.</param>
		public void Stop(bool runEvent){
			
			if(FinishedPlaying){
				return;
			}
			
			// Fire the cancelled event:
			AnimationEvent e = new AnimationEvent("cancel");
			dispatchEvent(e);
			
			// Find all properties belonging to this animation:
			AnimatedProperty current=FirstProperty;
			
			while(current!=null){
				
				// Grab the next one, just incase stop gets called:
				AnimatedProperty next=current.PropertyAfter;
				
				if(current.Animation==this){
					current.Stop();
				}
				
				// Hop to the next one:
				current=next;
			}
			
			if(runEvent){
				// Call finished:
				Finished();
			}
			
		}
		
	}
	
}