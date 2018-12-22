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
	/// Represents the color: css property.
	/// </summary>
	
	public class ColorProperty:CssProperty{
		
		public static ColorProperty GlobalProperty;
		
		public ColorProperty(){
			IsTextual=true;
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="black";
		}
		
		public override string[] GetProperties(){
			return new string[]{"color"};
		}
		
		public override void Aliases(){
			// e.g. color-a:
			ColourAliases();
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a paint:
			style.RequestPaintAll();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



