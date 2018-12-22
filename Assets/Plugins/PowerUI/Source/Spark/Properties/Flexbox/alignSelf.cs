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
	/// Represents the align-self: css property.
	/// </summary>
	
	public class AlignSelf:CssProperty{
		
		public static AlignSelf GlobalProperty;
		
		
		public AlignSelf(){
			GlobalProperty=this;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"align-self"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



