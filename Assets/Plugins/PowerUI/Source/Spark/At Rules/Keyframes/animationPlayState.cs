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
	/// Represents the animation-play-state: css property.
	/// </summary>
	
	public class AnimationPlayState:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-play-state"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			string text=null;
			
			if(value!=null){
				// Get the text:
				text=value.GetText(style.RenderData,this);
			}
			
			// Pause/ unpause:
			style.AnimationInstance.SetPause((text=="paused"));
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



