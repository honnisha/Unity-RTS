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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the animation-timing-function: css property.
	/// </summary>
	
	public class AnimationTimingFunction:CssProperty{
		
		public static AnimationTimingFunction GlobalProperty;
		
		public AnimationTimingFunction(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"animation-timing-function"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			style.AnimationInstance.TimingFunction=value;
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



