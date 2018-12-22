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
	/// Represents the word-wrap and overflow-wrap: css properties.
	/// </summary>
	
	public class OverflowWrap:CssProperty{
		
		public static OverflowWrap GlobalProperty;
		
		public OverflowWrap(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="normal";
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"overflow-wrap","word-wrap"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Apply:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}