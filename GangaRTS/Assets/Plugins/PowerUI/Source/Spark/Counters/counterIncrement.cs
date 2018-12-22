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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the counter-increment: css property.
	/// </summary>
	
	public class CounterIncrement:CssProperty{
		
		public static CounterIncrement GlobalProperty;
		
		
		public CounterIncrement(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"counter-increment"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the counter property:
			Css.CounterProperty cp=style.RenderData.GetProperty(typeof(Css.CounterProperty)) as Css.CounterProperty;
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				
				if(cp!=null){
					// Remove:
					cp.Remove();
				}
				
			}else{
				
				if(cp==null){
					
					// Create it:
					cp=new Css.CounterProperty(style.RenderData);
					
					// Add it now:
					style.RenderData.AddOrReplaceProperty(cp,typeof(Css.CounterProperty));
					
				}
				
				// Set as increments:
				cp.SetIncrements(value);
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}