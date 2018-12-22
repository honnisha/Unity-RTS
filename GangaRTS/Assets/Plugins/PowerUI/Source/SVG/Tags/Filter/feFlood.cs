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
	
	[Dom.TagName("feFlood")]
	public class SVGFEFloodElement:SVGFilterPrimitiveStandardAttributes{
		
		/// <summary>Flood opacity.</summary>
		public float FloodOpacity{
			get{
				// Resolve:
				return ResolveDecimal(Css.Properties.FloodOpacity.GlobalProperty);
			}
		}
		
		/// <summary>Converts this SVG FX node to a Loonim node.</summary>
		protected override TextureNode ToLoonimNode(SurfaceTexture tex){
			
			// Get the flood-col and opacity:
			Color floodColour=GetFilterColour(Css.Properties.FloodColor.GlobalProperty,Color.black);
			
			// Apply opacity:
			floodColour.a*=FloodOpacity;
			
			// Create as a simple property:
			return new Property(floodColour);
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="flood-color"){
				
				// Apply to CSS:
				style.floodColor=getAttribute("flood-color");
				
			}else if(property=="flood-opacity"){
				
				// Apply to CSS:
				style.floodOpacity=getAttribute("flood-opacity");
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}
