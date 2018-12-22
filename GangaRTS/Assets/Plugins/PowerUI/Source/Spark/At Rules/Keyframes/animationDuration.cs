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
	/// Represents the animation-duration: css property.
	/// </summary>
	
	public class AnimationDuration:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-duration"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null){
				style.AnimationInstance.Duration=0f;
			}else{
				style.AnimationInstance.Duration=value.GetDecimal(style.RenderData,this);
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



