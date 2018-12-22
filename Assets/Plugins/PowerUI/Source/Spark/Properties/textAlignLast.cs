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
	/// Represents the text-align-last: css property.
	/// </summary>
	
	public class TextAlignLast:CssProperty{
		
		public static TextAlignLast GlobalProperty;
		
		public TextAlignLast(){
			GlobalProperty=this;
			Inherits=true;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-align-last"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.ReloadValue;
			
		}
		
	}
	
}



