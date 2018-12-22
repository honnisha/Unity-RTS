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
	/// Represents the clear: css property.
	/// </summary>
	
	public class Clear:CssProperty{
		
		public static Clear GlobalProperty;
		
		
		public Clear(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"clear"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}