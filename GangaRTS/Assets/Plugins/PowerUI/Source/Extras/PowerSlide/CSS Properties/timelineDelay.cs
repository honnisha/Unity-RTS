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
	/// Represents the timeline-delay: css property.
	/// </summary>
	
	public class TimelineDelay:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides-delay","timeline-delay"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			Timeline si=Timeline.get(style);
			
			if(si==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null || value.Type==ValueType.Text){
				si.delay=0;
			}else{
				si.delay=value.GetDecimal(style.RenderData,this);
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



