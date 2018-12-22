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
	/// Represents the animation-delay: css property.
	/// </summary>
	
	public class AnimationDelay:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-delay"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null || value.Type==ValueType.Text){
				style.AnimationInstance.Delay=0;
			}else{
				style.AnimationInstance.Delay=value.GetInteger(style.RenderData,this);
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



