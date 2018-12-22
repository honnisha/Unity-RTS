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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the animation: composite css property.
	/// </summary>
	
	public class AnimationCompProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation"};
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			Css.Value name=null;
			Css.Value duration=null;
			Css.Value timing=null;
			Css.Value delay=null;
			Css.Value itCount=null;
			Css.Value direction=null;
			Css.Value fillMode=null;
			Css.Value playState=null;
			
			// Get the count:
			int count=value==null? 0 : value.Count;
			
			for(int i=0;i<count;i++){
				
				// Get the value:
				Css.Value innerValue=value[i];
				
				// If it's a time..
				if(innerValue is Css.Units.TimeUnit){
					
					// First or second time?
					if(duration==null){
						duration=innerValue;
						continue;
					}else if(delay==null){
						delay=innerValue;
						continue;
					}
					
				}
				
				// Just a number? Iteration count:
				if(itCount==null && innerValue is Css.Units.IntegerUnit){
					
					itCount=innerValue;
					continue;
					
				}
				
				// Function?
				if(timing==null && innerValue is Css.CssFunction){
					
					timing=innerValue;
					continue;
					
				}
				
				// Textual name:
				string text=innerValue.Text;
				
				// infinite etc.
				if(text=="normal" || text=="reverse" || text=="alternate" || text=="alternate-reverse"){
					
					direction=innerValue;
					continue;
					
				}
				
				if(text=="none" || text=="forwards" || text=="backwards" || text=="both"){
					
					fillMode=innerValue;
					continue;
					
				}
				
				if(text=="running" || text=="paused"){
					
					playState=innerValue;
					continue;
					
				}
				
				if(text=="infinite"){
					
					itCount=innerValue;
					continue;
					
				}
				
				// Timing function keywords:
				if(timing==null && innerValue is Css.CssKeyword){
					
					// Assume it's a timing function:
					timing=innerValue;
					continue;
					
				}
				
				if(name==null){
					name=innerValue;
				}
				
			}
			
			styleBlock.SetComposite("animation-duration",duration,value);
			styleBlock.SetComposite("animation-timing-function",timing,value);
			styleBlock.SetComposite("animation-delay",delay,value);
			styleBlock.SetComposite("animation-iteration-count",itCount,value);
			styleBlock.SetComposite("animation-direction",direction,value);
			styleBlock.SetComposite("animation-fill-mode",fillMode,value);
			styleBlock.SetComposite("animation-play-state",playState,value);
			
			// Declaring name last prevents weird jitters:
			styleBlock.SetComposite("animation-name",name,value);
			
		}
		
	}
	
}



