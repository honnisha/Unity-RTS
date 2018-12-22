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
	/// Represents the animation-direction: css property.
	/// </summary>
	
	public class AnimationDirection:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-direction"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
		
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			KeyframesAnimationDirection direction;
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				direction=KeyframesAnimationDirection.Forward;
			}else{
				
				string text=value.GetText(style.RenderData,this);
				
				switch(text){
					
					default:
						direction=KeyframesAnimationDirection.Forward;
					break;
					case "reverse":
						direction=KeyframesAnimationDirection.Backward;
					break;
					case "alternate-reverse":
						direction=KeyframesAnimationDirection.AlternateBackward;
					break;
					case "alternate":
						direction=KeyframesAnimationDirection.AlternateForward;
					break;
				}
				
			}
			
			style.AnimationInstance.Direction=direction;
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



