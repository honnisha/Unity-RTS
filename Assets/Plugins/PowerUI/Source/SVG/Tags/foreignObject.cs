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
using System.Text;
using Css;


namespace Svg{
	
	/// <summary>
	/// The SVG foreignObject tag.
	/// </summary>
	
	[Dom.TagName("foreignobject")]
	public class SVGForeignObjectElement:SVGElement{
		
		/*
		/// <summary>
		/// Gets the <see cref="GraphicsPath"/> for this element.
		/// </summary>
		/// <value></value>
		public override System.Drawing.Drawing2D.GraphicsPath Path(ISvgRenderer renderer)
		{
			return GetPaths(this, renderer);
		}
		*/
		
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
		
		/// <summary>
		/// Gets the bounds of the element.
		/// </summary>
		/// <value>The bounds.</value>
		public override BoxRegion Bounds
		{
			get 
			{
				return GroupBounds;
			}
		}

	}
}
