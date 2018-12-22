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

using System;
using System.Collections.Generic;
using System.Text;

namespace Svg{
	
	/// <summary>
	/// The SVG text span element.
	/// </summary>
	[Dom.TagName("tspan")]
	public class SVGTSpanElement : SVGTextPositioningElement{
		
		public override void BuildFilter(RenderContext ctx){
			
			// Nothing - children should not be rendered.
			
		}
		
	}
}