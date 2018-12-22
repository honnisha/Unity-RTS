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
	/// Represents the animation-name: css property.
	/// </summary>
	
	public class AnimationName:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-name"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// The name of the animation:
			string name=null;
			
			if(value!=null){
				
				// Get the name:
				name=value.GetText(style.RenderData,this);
				
			}
			
			if(name=="none" || name=="initial" || name=="normal"){
				name=null;
			}
			
			// Get the animation:
			KeyframesRule animation=style.document.GetAnimation(name);
			
			if(animation==null){
				// Clear:
				
				if(style.AnimationInstance!=null){
					// Stop without triggering OnDone:
					style.AnimationInstance.Stop(false);
				}
				
				// Reset matched styles
				style.ApplyMatchedStyles();
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(style.AnimationInstance!=null){
				
				if(style.AnimationInstance.RawAnimation==animation){
					// Stop there.
					return ApplyState.Ok;
				}else{
					// Stop it:
					style.AnimationInstance.Stop(false);
					
					// Reset matched:
					style.ApplyMatchedStyles();
				}
				
			}
			
			// Create an instance:
			style.AnimationInstance=new KeyframesAnimationInstance(style,animation);
			
			// Reapply other values:
			Reapply(style,"animation-duration");
			Reapply(style,"animation-timing-function");
			Reapply(style,"animation-delay");
			Reapply(style,"animation-iteration-count");
			Reapply(style,"animation-direction");
			Reapply(style,"animation-fill-mode");
			Reapply(style,"animation-play-state");
			
			// Start it:
			style.AnimationInstance.Start();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}