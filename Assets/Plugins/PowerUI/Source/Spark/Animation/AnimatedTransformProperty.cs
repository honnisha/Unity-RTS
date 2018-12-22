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
using UnityEngine;


namespace PowerUI{

	/// <summary>
	/// Helps with animating the CSS transform property as it requires custom handling.
	/// </summary>
	
	public sealed class AnimatedTransformProperty : AnimatedProperty{
		
		/// <summary>The set of one or more functions (write out to this).</summary>
		public Css.Value ToFunctions;
		/// <summary>The set of one or more functions.</summary>
		public Css.Value FromFunctions;
		/// <summary>The actual transitional value.</summary>
		public Css.Value ActiveFunctions;
		/// <summary>The cached matrix.</summary>
		private Css.Units.TransformValue RawTransformValue;
		
		
		/// <summary>Creates a new animated property.</summary>
		/// <param name="animation">The animation that this property is a part of.</param>
		/// <param name="property">The property being animated.</param>
		public AnimatedTransformProperty(UIAnimation animation)
			:base(animation,Css.Properties.TransformProperty.GlobalProperty){
		}
		
		/// <summary>Figures out what kind of lerp is required with the given from/to values.</summary>
		public void ApplyValues(Css.Value from,Css.Value to){
			
			// Get computed:
			if(from!=null){
				from=from.Computed;
				
				if(from.IsType(typeof(Css.Keywords.None))){
					from=null;
				}
			}
			
			if(to!=null){
				to=to.Computed;
				
				if(to.IsType(typeof(Css.Keywords.None))){
					to=null;
				}
			}
			
			// If both are null, do nothing.
			if(to==null && from==null){
				return;
			}
			
			// If either is a cached transformValue, pull the origin:
			if(to!=null && to is Css.Units.TransformValue){
				
				Css.Units.TransformValue rawTo=to as Css.Units.TransformValue;
				to=rawTo.Origin;
				
			}
			
			if(from!=null && from is Css.Units.TransformValue){
				
				Css.Units.TransformValue rawFrom=from as Css.Units.TransformValue;
				from=rawFrom.Origin;
				
			}
			
			// If one or the other is null then it acts like the identity of the other.
			if(to!=null && from!=null){
				
				// Check for functional equiv:
				if(!to.FunctionalEquals(from)){
					
					// Matrix interpolation :'(
					
					// Bake both matrices:
					RenderableData rd=Animation.ComputedStyle.RenderData;
					
					Matrix4x4 fromMatrix=Css.Properties.TransformProperty.Compute(from,rd);
					Matrix4x4 toMatrix=Css.Properties.TransformProperty.Compute(to,rd);
					
					// If either is 3D then they both are treated as 3D values:
					if(Css.Properties.TransformProperty.Is3D(from) || Css.Properties.TransformProperty.Is3D(to) ){
						
						// 3D!
						
						// Write out into this value (no prep required here):
						FromFunctions=new InterpolationMatrix3D(fromMatrix);
						ToFunctions=new InterpolationMatrix3D(toMatrix);
						
					}else{
					
						// 2D!
					
						// Write out into this value:
						InterpolationMatrix fromInt=new InterpolationMatrix(fromMatrix);
						InterpolationMatrix toInt=new InterpolationMatrix(toMatrix);
						
						// Prep for interpolation:
						fromInt.PrepareForInterpolate(toInt);
						
						ToFunctions=toInt;
						FromFunctions=fromInt;
						
					}
					
					// Copy the from value:
					ActiveFunctions=FromFunctions.Copy();
					
					// Write it back out:
					WriteOut();
					return;
					
				}
				
			}
			
			// Interpolate every parameter of each function (if the other is null, use 0)
			// Note! The same function can appear multiple times.
			ToFunctions=to;
			FromFunctions=from;
			
			if(FromFunctions==null){
				// Copy to (going from none) and set it all as zero:
				ActiveFunctions=ToFunctions.Copy();
				
				// Set it all to 'zero' (it's e.g. 1 for scale though!)
				SetDefaults(ActiveFunctions);
				
			}else{
				// Copy from:
				ActiveFunctions=FromFunctions.Copy();
			}
			
			// Write it back out:
			WriteOut();
			
		}
		
		/// <summary>Sets the defaults for one or more transform functions.</summary>
		private void SetDefaults(Css.Value set){
			
			// Get as a function:
			Css.Functions.Transformation tf=set as Css.Functions.Transformation;
			
			if(tf!=null){
				tf.SetDefaults();
				return;
			}
			
			// Should be a set:
			if(set is Css.ValueSet){
				
				for(int i=0;i<set.Count;i++){
					
					// Get as a function:
					tf=set[i] as Css.Functions.Transformation;
					
					if(tf!=null){
						tf.SetDefaults();
					}
					
				}
				
			}
			
		}
		
		private void WriteOut(){
			
			// Cache transform now:
			Css.Units.TransformValue existing=Animation.ComputedStyle[InnerPropertyInfo] as Css.Units.TransformValue;
			
			if(existing==null){
				
				// Build and cache it now:
				existing=new Css.Units.TransformValue( Animation.ComputedStyle,ActiveFunctions );
				existing.Changed=false;
				
				// Cache it now:
				Animation.ComputedStyle[InnerPropertyInfo]=existing;
				
				// Full layout required (so it gets adopted by kids):
				Animation.ComputedStyle.RequestLayout();
				
			}else{
				
				// Update its origin:
				existing.Origin=ActiveFunctions;
				existing.Changed=false;
				
			}
			
			RawTransformValue=existing;
			
		}
		
		public override void Update(float deltaTime){
			
			if(Animation==null || Animation.Paused){
				return;
			}
			
			if(CurrentTime==0f){
				
				if(deltaTime>0.5f){
					// Block slow frames.
					// This is almost always only ever the very first one
					return;
				}
				
				// Setup targets etc.
				
				// Get or create the initial value:
				Css.Value hostValue;
				Css.Value rawValue=InnerPropertyInfo.GetOrCreateValue(Animation.Animating,Animation.ComputedStyle,false,out hostValue);
				
				// Setup values:
				ApplyValues(rawValue,RawTarget);
				
				if(Animation.Duration==0f){
					
					// Make sure we are exactly the right value:
					WriteProgress(1f);
					
					// Remove from update queue:
					Stop();
					
					// Tell the style it changed:
					// Note that in grouped properties, only the last one actually runs the update.
					RawTransformValue.Recalculate(Animation.ComputedStyle);
					Animation.ComputedStyle.RequestPaintAll();
					
					// And call the done function:
					Animation.Finished();
					
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
				
				// Remove from the update queue:
				Stop();
				
				// Make sure we are exactly the right value.
				
				if(ProgressSampler!=null){
					
					// Map through the progression curve:
					ProgressSampler.Goto(progress,true);
					progress=ProgressSampler.CurrentValue;
					
				}
				
				// Write it out to the CSS value:
				WriteProgress(progress);
				
				// If we're the main animation (updateCss is true), tell the style it changed:
				// Note that in grouped properties, only the last one actually runs the update.
				RawTransformValue.Recalculate(computed);
				computed.RequestPaintAll();
				
				// And call the done function:
				Animation.Finished();
				
				// Stop there:
				return;
				
			}else{
				
				// Set ActiveValue by sampling from the curve (if there is one):
				
				if(ProgressSampler!=null){
					
					// Map through the progression curve:
					ProgressSampler.Goto(progress,true);
					progress=ProgressSampler.CurrentValue;
					
				}
				
				// Read the value:
				WriteProgress(progress);
				
			}
			
			// And Tell the style it changed:
			// Note that in grouped properties, only the last one actually runs the update.
			RawTransformValue.Recalculate(computed);
			computed.RequestPaintAll();
			
		}
		
		/// <summary>Writes out to TransformValueObject now using the given 0-1 progress value.</summary>
		public void WriteProgress(float progress){
			
			if(ActiveFunctions is Css.CssFunction){
				
				// For each arg of activeFunction..
				int count=ActiveFunctions.Count;
				
				for(int i=0;i<count;i++){
					
					// Get to/from values:
					float to=(ToFunctions==null) ? 0f : ToFunctions[i].GetRawDecimal();
					float from=(FromFunctions==null) ? 0f : FromFunctions[i].GetRawDecimal();
					
					// Progress it:
					ActiveFunctions[i].SetRawDecimal( from+((to-from)*progress) );
					
				}
				
				// If it's a 3D interpol matrix, also slerp quaternions:
				if(ActiveFunctions is InterpolationMatrix3D){
					
					InterpolationMatrix3D active3D=ActiveFunctions as InterpolationMatrix3D;
					InterpolationMatrix3D to3D=ToFunctions as InterpolationMatrix3D;
					InterpolationMatrix3D from3D=FromFunctions as InterpolationMatrix3D;
					
					// Slerp it (sets Changed for us too):
					active3D.Rotation=Quaternion.Slerp(from3D.Rotation,to3D.Rotation,progress);
					
				}else if(ActiveFunctions is InterpolationMatrix){
					
					// Set changed:
					InterpolationMatrix activeMatrix=ActiveFunctions as InterpolationMatrix;
					activeMatrix.Changed=true;
					
				}
				
			}else{
				
				// For each function..
				int count=ActiveFunctions.Count;
				
				for(int i=0;i<count;i++){
					
					// Get the function:
					Css.Value fromHost=(FromFunctions==null) ? null : FromFunctions[i];
					Css.Value toHost=(ToFunctions==null) ? null : ToFunctions[i];
					Css.Value active=ActiveFunctions[i];
					
					int pCount=active.Count;
					
					// For each parameter:
					for(int p=0;p<pCount;p++){
						
						// Get to/from values:
						float to=(toHost==null)? 0f : toHost[p].GetRawDecimal();
						float from=(fromHost==null)? 0f : fromHost[p].GetRawDecimal();
						
						// Progress it:
						active[p].SetRawDecimal( from+((to-from)*progress) );
						
					}
					
				}
				
			}
			
		}
		
	}
	
}