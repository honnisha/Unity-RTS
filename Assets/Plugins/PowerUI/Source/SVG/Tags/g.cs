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
	/// The SVG g(roup) tag.
	/// </summary>
	
	[Dom.TagName("g")]
	public class SVGGElement:SVGElement{
		
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