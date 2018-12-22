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
	/// Represents the clip: css property.
	/// </summary>
	
	public class Clip:CssProperty{
		
		public static Clip GlobalProperty;
		
		
		public Clip(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"clip"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



