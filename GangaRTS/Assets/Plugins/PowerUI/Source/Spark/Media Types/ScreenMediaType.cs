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
using System.Collections;
using System.Collections.Generic;
using Css.Units;


namespace Css{
	
	/// <summary>
	/// The standard 'screen' media type.
	/// </summary>
	
	public class ScreenMediaType : MediaType{
		
		public ScreenMediaType(ReflowDocument doc){
			
			// Load all the values now:
			Css.Value width=new Css.Units.PxUnit(Width);
			Css.Value height=new Css.Units.PxUnit(Height);
			Css.Value aspect=new Css.Units.DecimalUnit((float)Width / (float)Height);
			
			// Pull in the default values:
			// Note that if any are numeric and 0, it treats it as a null.
			// (which in turn makes the feature unavailable, as expected by the HasFeature property).
			this["width"]=width;
			this["height"]=height;
			this["device-width"]=width;
			this["device-height"]=height;
			this["aspect-ratio"]=aspect;
			this["device-aspect-ratio"]=aspect;
			this["orientation"]=new Css.Units.TextUnit(Landscape?"landscape":"portrait");
			this["resolution"]=new Css.Units.DecimalUnit(Resolution);
			this["color"]=new Css.Units.DecimalUnit(Color);
			this["color-index"]=new Css.Units.DecimalUnit(ColorIndex);
			this["monochrome"]=new Css.Units.DecimalUnit(Monochrome);
			this["scan"]=new Css.Units.TextUnit(Scan);
			this["grid"]=new Css.Units.DecimalUnit(Grid);
			
			// Ready!
			Ready(doc);
			
		}
		
		// Note that width etc are fine as default.
		
		/// <summary>True if this media type is suitable for the given name. For example
		/// a mobile media type returns true for at least 'handheld'. Note that 'all' is handled separately.</summary>
		public override bool Is(string name){
			return (name=="screen");
		}
		
	}
	
}