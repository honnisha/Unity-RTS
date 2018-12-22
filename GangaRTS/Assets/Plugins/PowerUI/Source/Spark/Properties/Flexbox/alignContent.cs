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
	/// Represents the align-content: css property.
	/// </summary>
	
	public class AlignContent:CssProperty{
		
		public static AlignContent GlobalProperty;
		
		
		public AlignContent(){
			GlobalProperty=this;
			InitialValueText="normal";
		}
		
		public override string[] GetProperties(){
			return new string[]{"align-content"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



