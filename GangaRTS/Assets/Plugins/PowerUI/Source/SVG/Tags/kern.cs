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
	/// The SVG kern tag.
	/// </summary>
	
	public class SVGKernElement:SVGElement{
		
		public string Glyph1;
		
		public string Glyph2;
		
		public string Unicode1;
		
		public string Unicode2;
		
		public float Kerning;
		
		
		public override bool OnAttributeChange(string property){
			
			if(property=="g1"){
				
				Glyph1=getAttribute("g1");
				
			}else if(property=="g2"){
				
				Glyph2=getAttribute("g2");
				
			}else if(property=="u1"){
				
				Unicode1=getAttribute("g1");
				
			}else if(property=="u2"){
				
				Unicode2=getAttribute("g2");
				
			}else if(property=="k"){
				
				Kerning=GetFloatAttribute("k",0f);
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
			
		}
		
	}
	
	/// <summary>
	/// The SVG vertical kern tag.
	/// </summary>
	
	[Dom.TagName("vkern")]
	public class SVGVKernElement : SVGKernElement
	{
		
	}
	
	/// <summary>
	/// The SVG horizontal kern tag.
	/// </summary>
	
	[Dom.TagName("hkern")]
	public class SVGHKernElement : SVGKernElement
	{
		
	}
	
}
