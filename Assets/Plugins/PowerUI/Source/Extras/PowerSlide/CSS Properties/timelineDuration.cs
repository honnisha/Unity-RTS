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
using PowerSlide;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the timeline-duration: css property.
	/// </summary>
	
	public class TimelineDuration:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides-duration","timeline-direction"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			Timeline si=Timeline.get(style);
			
			if(si==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null){
				si.appliedDuration=float.MinValue;
			}else{
				si.appliedDuration=value.GetDecimal(style.RenderData,this);
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



