//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using Dom;
using Blaze;
using UnityEngine;
using Loonim;


namespace Svg{
	
	/// <summary>
	/// The feBlend element.
	/// </summary>
	
	[Dom.TagName("feBlend")]
	public class SVGFEBlendElement:SVGFilterPrimitiveStandardAttributes{
		
		/// <summary>The blending mode to use.</summary>
		public BlendingMode Mode{
			get{
				
				string attrib=getAttribute("mode");
				
				if(attrib!=null){
					// Tidy it:
					attrib=attrib.Trim().ToLower();
				}
				
				switch(attrib){
					case "multiply":
						return BlendingMode.Multiply;
					case "screen":
						return BlendingMode.Screen;
					case "darken":
						return BlendingMode.Darken;
					case "lighten":
						return BlendingMode.Lighten;
					
				}
				
				// Normal otherwise:
				return BlendingMode.Normal;
			}
		}
		
		/// <summary>Converts this SVG FX node to a Loonim node.</summary>
		protected override TextureNode ToLoonimNode(SurfaceTexture tex){
			
			// Create:
			return new Blend(
				In1.GetLoonim(tex),
				In2.GetLoonim(tex),
				new Property((float)1f), // Weight (always 1)
				new Property((float)Mode)
			);
			
		}
		
	}
	
}