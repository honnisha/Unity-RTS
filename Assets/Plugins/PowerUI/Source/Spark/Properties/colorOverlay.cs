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
	/// Represents the color-overlay: css property.
	/// Specific to PowerUI. This overlays the given colour over any element.
	/// </summary>
	
	public class ColorOverlay:CssProperty{
		
		public static ColorOverlay GlobalProperty;
		
		public ColorOverlay(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="#ffffffff";
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override string[] GetProperties(){
			return new string[]{"color-overlay"};
		}
		
		public override void Aliases(){
			
			// Add e.g. color-overlay-a:
			ColourAliases();
			
			// Opacity maps to color-overlay-a, aka index 3 of color-overlay:
			Alias("opacity",ValueAxis.None,3);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a repaint:
			style.RequestPaintAll();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



