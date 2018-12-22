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

using Dom;
using Blaze;
using UnityEngine;


namespace Svg{
	
	/// <summary>
	/// The SVG font tag.
	/// </summary>
	
	[Dom.TagName("font")]
	public class SVGFontElement:SVGElement{
		
        public float HorizAdvX;
		
        public float HorizOriginX;
		
        public float HorizOriginY;
		
        public float VertAdvY=float.MaxValue; // If not set, defaults to UnitsPerEm
		
        public float VertOriginX=float.MaxValue; // If not set, defaults to HorizAdvX/2
		
        public float VertOriginY=float.MaxValue; // Default is the 'ascent' (it tried to read the attrib, then read the .Ascent property).
		
		
		public override bool OnAttributeChange(string property){
			
			if(property=="horiz-adv-x"){
				
				HorizAdvX=GetFloatAttribute("horiz-adv-x",0f);
				
			}else if(property=="horiz-origin-x"){
				
				HorizOriginX=GetFloatAttribute("horiz-origin-x",0f);
				
			}else if(property=="horiz-origin-y"){
				
				HorizOriginY=GetFloatAttribute("horiz-origin-y",0f);
				
			}else if(property=="vert-adv-y"){
				
				VertAdvY=GetFloatAttribute("vert-adv-y",float.MaxValue);
				
			}else if(property=="vert-origin-x"){
				
				VertOriginX=GetFloatAttribute("vert-origin-x",float.MaxValue);
				
			}else if(property=="vert-origin-y"){
				
				VertOriginY=GetFloatAttribute("vert-origin-y",float.MaxValue);
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
    }
}
