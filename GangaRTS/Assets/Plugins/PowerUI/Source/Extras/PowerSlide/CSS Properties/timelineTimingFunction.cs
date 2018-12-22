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
	/// Represents the timeline-timing-function: css property
	/// </summary>
	
	public class TimelineTimingFunction:CssProperty{
		
		public static TimelineTimingFunction GlobalProperty;
		
		public TimelineTimingFunction(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"slides-timing-function","timeline-timing-function"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			Timeline si=Timeline.get(style);
			
			if(si==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			si.setTimingFunction(value);
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



