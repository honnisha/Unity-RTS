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
	/// Represents the cursor: css property.
	/// </summary>
	
	public class Cursor:CssProperty{
		
		public static Cursor GlobalProperty;
		
		
		public Cursor(){
			Inherits=true;
			InitialValue=AUTO;
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"cursor"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



