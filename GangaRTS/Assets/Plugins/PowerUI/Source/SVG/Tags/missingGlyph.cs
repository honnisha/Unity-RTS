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
	/// The SVG metadata element.
	/// </summary>
	[Dom.TagName("missing-glyph")]
	public class SVGMissingGlyphElement:SVGGlyphElement{
		
		public override string GlyphName{
			get{
				string name=base.GlyphName;
				
				if(name==null){
					return "__MISSING_GLYPH__";
				}
				
				return name;
			}
			set{
				base.GlyphName=value;
			}
		}
		
	}
}