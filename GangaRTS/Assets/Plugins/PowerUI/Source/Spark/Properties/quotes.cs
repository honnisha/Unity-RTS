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
	/// Represents the quotes: css property. Accessed with the open-quote/close-quote CSS keywords.
	/// </summary>
	
	public class Quotes:CssProperty{
		
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static Quotes GlobalProperty;
		
		
		public Quotes(){
			
			// This is along no axis:
			Axis=ValueAxis.None;
			
			// Grab a global reference:
			GlobalProperty=this;
			
			Inherits=true;
			
			// Based on language:
			InitialValue=AUTO;
			
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"quotes"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



