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
	/// Represents the text-align: css property.
	/// </summary>
	
	public class TextAlign:CssProperty{
		
		public static TextAlign GlobalProperty;
		
		public TextAlign(){
			GlobalProperty=this;
			Inherits=true;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-align"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.ReloadValue;
			
		}
		
	}
	
}



