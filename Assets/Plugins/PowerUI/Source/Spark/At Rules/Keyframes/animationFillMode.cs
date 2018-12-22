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
	/// Represents the animation-fill-mode: css property.
	/// </summary>
	
	public class AnimationFillMode:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-fill-mode"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			KeyframesFill fill;
			
			if(value==null){
				fill=KeyframesFill.None;
			}else{
				
				string text=value.GetText(style.RenderData,this);
				
				switch(text){
					default:
						fill=KeyframesFill.None;
					break;
					case "forward":
						fill=KeyframesFill.Forward;
					break;
					case "backward":
						fill=KeyframesFill.Backward;
					break;
					case "both":
						fill=KeyframesFill.Both;
					break;
				}
				
			}
			
			style.AnimationInstance.FillMode=fill;
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



