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


namespace Svg{
	
	/// <summary>
	/// The SVG font-face tag.
	/// </summary>
	
	[Dom.TagName("font-face")]
	public class SVGFontFaceElement:SVGElement{
		
		public float AccentHeight;
		public float Ideographic;
		public float Hanging;
		public float VIdeographic;
		public float VAlphabetic;
		public float VMathematical;
		public float VHanging;
		public float UnderlinePosition;
		public float UnderlineThickness;
		public float StrikethroughPosition;
		public float StrikethroughThickness;
		public float OverlinePosition;
		public float OverlineThickness;
		
		
		// Note: many of the attributes here are handled by CSS.
		
		public override bool OnAttributeChange(string property){
			
			if(property=="accent-height"){
				
				AccentHeight=GetFloatAttribute("accent-height",0f);
				
			}else if(property=="ideographic"){
				
				Ideographic=GetFloatAttribute("ideographic",0f);
				
			}else if(property=="alphabetic"){
				
				// Same as CSS 'baseline':
				style["baseline"]=Css.Value.Load(getAttribute("alphabetic"));
				
			}else if(property=="mathematical"){
				
				// Same as CSS 'mathline':
				style["mathline"]=Css.Value.Load(getAttribute("mathematical"));
				
			}else if(property=="hanging"){
				
				Hanging=GetFloatAttribute("hanging",0f);
				
			}else if(property=="v-ideographic"){
				
				VIdeographic=GetFloatAttribute("v-ideographic",0f);
				
			}else if(property=="v-alphabetic"){
				
				VAlphabetic=GetFloatAttribute("v-alphabetic",0f);
				
			}else if(property=="v-mathematical"){
				
				VMathematical=GetFloatAttribute("v-mathematical",0f);
				
			}else if(property=="v-hanging"){
				
				VHanging=GetFloatAttribute("v-hanging",0f);
				
			}else if(property=="underline-position"){
				
				UnderlinePosition=GetFloatAttribute("underline-position",0f);
			
			}else if(property=="underline-thickness"){
				
				UnderlineThickness=GetFloatAttribute("underline-thickness",0f);
			
			}else if(property=="strikethrough-position"){
				
				StrikethroughPosition=GetFloatAttribute("strikethrough-position",0f);
			
			}else if(property=="strikethrough-thickness"){
				
				StrikethroughThickness=GetFloatAttribute("strikethrough-thickness",0f);
			
			}else if(property=="overline-position"){
				
				OverlinePosition=GetFloatAttribute("overline-position",0f);
			
			}else if(property=="overline-thickness"){
				
				OverlineThickness=GetFloatAttribute("overline-thickness",0f);
			
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
}
