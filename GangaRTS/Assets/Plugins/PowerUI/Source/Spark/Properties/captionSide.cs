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
	/// Represents the caption-side: css property.
	/// </summary>
	
	public class CaptionSide:CssProperty{
		
		public static CaptionSide GlobalProperty;
		
		
		public CaptionSide(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="top";
		}
		
		public override string[] GetProperties(){
			return new string[]{"caption-side"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



