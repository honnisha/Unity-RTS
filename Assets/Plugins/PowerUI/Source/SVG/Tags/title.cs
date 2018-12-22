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
	/// The SVG title element.
	/// </summary>
	[Dom.TagName("title")]
	public class SVGTitleElement:SVGElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		public override void BuildFilter(RenderContext ctx){
			
			// Nothing - children should not be rendered.
			
		}
		
	}
}