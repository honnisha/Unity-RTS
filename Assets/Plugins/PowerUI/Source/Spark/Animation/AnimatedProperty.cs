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
using Css.Units;
using Dom;


namespace PowerUI{

	/// <summary>
	/// A single css property being animated. Note that composite properties such as colours or rotations must be broken down into
	/// their inner properties (e.g. rgba, xyz). This is done internally by <see cref="PowerUI.UIAnimation"/>.
	/// </summary>
	
	public class AnimatedProperty{
		
		/// <summary>True if this particular property should flush the output to the screen. This is required if
		/// there are multiple inner properties for a particular CSS property.</summary>
		public bool UpdateCss;
		/// <summary>The current time in seconds that has passed since the animation started.</summary>
		public float CurrentTime;
		/// <summary>The value that should be applied right now in terms of units.</summary>
		public float ActiveValue;
		/// <summary>TargetValue-StartValue.</summary>
		private float DeltaValue;
		/// <summary>The initial starting value.</summary>
		private float StartValue;
		/// <summary>The parent animation that this property belongs to.</summary>
		public UIAnimation Animation;
		/// <summary>The starting CSS value.</summary>
		public Css.Value RawStart;
		/// <summary>The CSS value object that the current value of this is applied to.</summary>
		public Css.Units.DecimalUnit ValueObject;
		/// <summary>The potentially composite property being animated. E.g. color.</summary>
		public CssProperty PropertyInfo;
		/// <summary>The "actual" property being animated, if there is one. E.g. color-r.</summary>
		public CssProperty InnerPropertyInfo;
		/// <summary>The CSS property that this value is a part of. For example, if this property currently being animated is 
		/// the red component of the colour overlay, the property value object is the colour overlay as a whole.</summary>
		public Css.Value PropertyValueObject;
		/// <summary>Currently animated properties are stored in a linked list. This is the next one in the list.</summary>
		public AnimatedProperty PropertyAfter;
		/// <summary>Currently animated properties are stored in a linked list. This is the one before this in the list.</summary>
		public AnimatedProperty PropertyBefore;
		/// <summary>The sampler used when progressing the animation.</summary>
		public Blaze.CurveSampler ProgressSampler;
		/// <summary>The raw target value.</summary>
		internal Css.Value RawTarget;
		
		
		/// <summary>Creates a new animated property.</summary>
		/// <param name="animation">The animation that this property is a part of.</param>
		/// <param name="property">The property being animated.</param>
		public AnimatedProperty(UIAnimation animation,CssProperty property){
			Animation=animation;
			InnerPropertyInfo=property;
			
			if(property.IsAlias){
				CssPropertyAlias alias=property as CssPropertyAlias;
				PropertyInfo=alias.Target;
			}else{
				PropertyInfo=property;
			}
			
		}
		
		/// <summary>Animates this property now.</summary>
		/// <param name="animation">The animation that this property is a part of.</param>
		/// <param name="targetValue">The parsed value that this property will be when the animation is over.</param>
		/// <param name="timeCurve">Optional curve used to describe the progression of the animation.
		/// X is time, Y is value. Null acts like linear (a line from 0,0 to 1,1).</param>
		/// <param name="updateCss">True if this particular property should flush its changes to css/the screen.</param>
		public void Animate(UIAnimation animation,Css.Value targetValue,Blaze.VectorPath timeCurve,bool updateCss){
			
			Animation=animation;
			CurrentTime=0f;
			UpdateCss=updateCss;
			RawTarget=targetValue;
			
			if(timeCurve==null){
				ProgressSampler=null;
			}else{
				
				if(!(timeCurve is Blaze.RasterVectorPath)){
					
					Blaze.RasterVectorPath rvp=new Blaze.RasterVectorPath();
					timeCurve.CopyInto(rvp);
					rvp.ToStraightLines();
					timeCurve=rvp;
					
				}
				
				ProgressSampler=new Blaze.CurveSampler(timeCurve);
				ProgressSampler.Reset();
			}
			
		}
		
		/// <summary>Sets up the initial value.</summary>
		public void SetupValue(Css.Value hostValue,Css.Value rawValue){
			
			RawStart=rawValue;
			ValueObject=rawValue as Css.Units.DecimalUnit;
			PropertyValueObject=hostValue;
			
		}
		
		private void Complete(){
			
			// Remove from the update queue:
			Stop();
			
			// Set target to the result.
			
			Css.Value target=RawTarget.Copy();
			
			// Write it straight out to the computed style:
			// (Internally handles any aliases for us)
			Animation.ComputedStyle[InnerPropertyInfo]=target;
			
			// If it's not an alias, update hostValue:
			if(!InnerPropertyInfo.IsAlias){
				PropertyValueObject=RawTarget;
			}
			
			if(UpdateCss){
				
				// If we're the main animation (updateCss is true), tell the style it changed:
				// Note that in grouped properties, only the last one actually runs the update.
				Animation.ComputedStyle.ChangeProperty(PropertyInfo,PropertyValueObject);
				
				// And call the done function:
				Animation.Finished();
				
			}
			
		}
		
		public virtual void Update(float deltaTime){
			
			if(Animation==null || Animation.Paused){
				return;
			}
			
			if(CurrentTime==0f){
				
				if(deltaTime>0.5f){
					// Block slow frames.
					// This is almost always only ever the very first one
					return;
				}
				
				// Make sure they're the same units.
				if(ValueObject!=null && RawStart.GetType()==RawTarget.GetType()){
					
					// Raw start/end values:
					StartValue=RawStart.GetRawDecimal();
					DeltaValue=RawTarget.GetRawDecimal();
					
					// Update delta:
					DeltaValue-=StartValue;
					
				}else{
					
					// Different units or are non-numeric! Also applies to e.g. "none" -> "x%".
					// We'll be using pixel values here, but we must use GetDecimal every frame of the animation.
					
					// Force it to be a raw decimal:
					ValueObject=new Css.Units.DecimalUnit(0f);
					
					// Write it straight out to the computed style:
					// (Internally handles any aliases for us)
					Animation.ComputedStyle[InnerPropertyInfo]=ValueObject;
					
					// If it's not an alias, update hostValue:
					if(!InnerPropertyInfo.IsAlias){
						PropertyValueObject=ValueObject;
					}
					
				}
				
				// Setup targets etc.
				
				// Instant?
				if(Animation.Duration==0f){
					
					Complete();
					
					return;
				}
				
			}
			
			CurrentTime+=deltaTime;
			
			if(!Animation.Animating.isRooted){
				
				// Immediately stop - the element was removed (don't call the finished event):
				Stop();
				
				return;
				
			}
			
			// Get the style:
			ComputedStyle computed=Animation.ComputedStyle;
			
			float progress=CurrentTime / Animation.TotalTime;
			
			if(progress >= 1f){
				
				// Done!
				progress=1f;
				
				// Make sure we are exactly the right value.
				Complete();
				
				// Stop there:
				return;
				
			}
			
			// Set ActiveValue by sampling from the curve (if there is one):
			
			if(ProgressSampler!=null){
				
				// Map through the progression curve:
				ProgressSampler.Goto(progress,true);
				progress=ProgressSampler.CurrentValue;
				
			}
			
			if(RawStart!=ValueObject){
				
				// Get the current from/to values:
				StartValue=RawStart.GetDecimal(computed.RenderData,InnerPropertyInfo);
				DeltaValue=RawTarget.GetDecimal(computed.RenderData,InnerPropertyInfo);
				
				// Update delta:
				DeltaValue-=StartValue;
				
			}
			
			// Read the value:
			ActiveValue=StartValue + (DeltaValue * progress);
			
			// Write it out to the CSS value:
			ValueObject.RawValue=ActiveValue;
			
			if(UpdateCss){
				
				// And Tell the style it changed:
				// Note that in grouped properties, only the last one actually runs the update.
				computed.ChangeProperty(PropertyInfo,PropertyValueObject);
				
			}
			
		}
		
		public void AddToQueue(){
			// Don't call if it's known to already be in the update queue.
			if(UIAnimation.FirstProperty==null){
				UIAnimation.FirstProperty=UIAnimation.LastProperty=this;
			}else{
				PropertyBefore=UIAnimation.LastProperty;
				UIAnimation.LastProperty = UIAnimation.LastProperty.PropertyAfter = this;
			}
		}
		
		public void Stop(){
			if(PropertyBefore==null){
				UIAnimation.FirstProperty=PropertyAfter;
			}else{
				PropertyBefore.PropertyAfter=PropertyAfter;
			}
			
			if(PropertyAfter==null){
				UIAnimation.LastProperty=PropertyBefore;
			}else{
				PropertyAfter.PropertyBefore=PropertyBefore;
			}
			
		}
		
		/// <summary>The prime property being animated.</summary>
		public string Property{
			get{
				return PropertyInfo.Name;
			}
		}
		
		/// <summary>The node being animated.</summary>
		public Node Animating{
			get{
				return Animation.Animating;
			}
		}
		
	}
	
}