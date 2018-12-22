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
	/// Represents the visibility: css property.
	/// </summary>
	
	public class Visibility:CssProperty{
		
		public static Visibility GlobalProperty;
		
		public Visibility(){
			GlobalProperty=this;
			InitialValueText="visible";
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"visibility"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



