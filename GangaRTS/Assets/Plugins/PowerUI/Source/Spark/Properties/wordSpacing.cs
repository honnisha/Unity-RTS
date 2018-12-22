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
using UnityEngine;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the word-spacing: css property.
	/// </summary>
	
	public class WordSpacing:CssProperty{
		
		public static WordSpacing GlobalProperty;
		
		public WordSpacing(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
			InitialValueText="normal"; // = -1
		}
		
		
		public override float GetNormalValue(RenderableData context){
			return -1f;
		}
		
		public override string[] GetProperties(){
			return new string[]{"word-spacing"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Apply:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}