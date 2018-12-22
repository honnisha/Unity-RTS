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
	/// Represents the animation-iteration-count: css property.
	/// </summary>
	
	public class AnimationIterationCount:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"animation-iteration-count"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(style.AnimationInstance==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				style.AnimationInstance.RepeatCount=1;
			}else if(value.Type==ValueType.Text){
				// Infinite
				style.AnimationInstance.RepeatCount=-1;
			}else{
				style.AnimationInstance.RepeatCount=value.GetInteger(style.RenderData,this);
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



