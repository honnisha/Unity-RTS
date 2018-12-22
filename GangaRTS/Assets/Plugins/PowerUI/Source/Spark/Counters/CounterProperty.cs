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


namespace Css{
	
	/// <summary>
	/// Handles incrementing and resetting counters.
	/// </summary>
	public class CounterProperty : Css.DisplayableProperty{
		
		/// <summary>The counters to reset if any.</summary>
		public List<string> Resets;
		/// <summary>The counters to increment (the name and amount to increment by).</summary>
		public List<CssCounter> Increments;
		
		
		public CounterProperty(RenderableData data):base(data){}
		
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			// Any to increment?
			if(Increments!=null){
				
				for(int i=0;i<Increments.Count;i++){
					
					// Increment it:
					renderer.IncrementCounter(Increments[i].Name,Increments[i].Count);
					
				}
				
			}
			
			// Any to 'reset'?
			if(Resets!=null){
				
				for(int i=0;i<Resets.Count;i++){
					
					// Reset it:
					renderer.ResetCounter(Resets[i]);
					
				}
				
			}
			
		}
		
		public override bool Render(bool first,LayoutBox box,Renderman renderer){
			
			base.Render(first,box,renderer);
			
			// PostProcess required if this has Resets (i.e. after the element has done all of its kids):
			return (Resets!=null);
		}
		
		/// <summary>Transforms all the blocks that this property has allocated. Note that transformations are a post process.
		/// Special case for borders as it may also internally transform its corners.</summary>
		/// <param name="topTransform">The transform that should be applied to this property.</param>
		public override void PostProcess(LayoutBox box,Renderman renderer){
			
			if(Resets!=null){
				
				// Pop all of the counters:
				for(int i=0;i<Resets.Count;i++){
					
					// Pop it:
					renderer.PopCounter(Resets[i]);
					
				}
				
			}
			
		}
		
		/// <summary>Sets up the counters to reset.</summary>
		public void SetResets(Css.Value set){
			
			if(Resets==null){
				Resets=new List<string>();
			}else{
				Resets.Clear();
			}
			
			// For each one..
			for(int i=0;i<set.Count;i++){
				
				Resets.Add(set[i].Text);
				
			}
			
		}
		
		/// <summary>Sets up the values to increment.</summary>
		public void SetIncrements(Css.Value set){
			
			if(Increments==null){
				Increments=new List<CssCounter>();
			}else{
				Increments.Clear();
			}
			
			if(set is Css.ValueSet){
				
				// For each one..
				for(int i=0;i<set.Count;i++){
					
					// If this is a number then it's the count for the previous increment.
					Css.Value value=set[i];
					
					if(value is Css.Units.DecimalUnit && Increments.Count>0){
						
						// Number to increment by (for prev counter):
						CssCounter counter=Increments[Increments.Count-1];
						counter.Count=value.GetInteger(null,null);
						Increments[Increments.Count-1]=counter;
						
					}else{
						
						// Another counter:
						Increments.Add(new CssCounter(value.Text,1));
						
					}
					
				}
				
			}else{
				
				// Just one:
				Increments.Add(new CssCounter(set.Text,1));
				
			}
			
		}
		
	}
	
}