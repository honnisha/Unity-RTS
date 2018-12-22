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
using Css;


namespace Svg{
	
	/// <summary>
	/// An SVG switch tag.
	/// </summary>
	
	[Dom.TagName("switch")]
	public class SVGSwitchElement:SVGElement{
		
		/// <summary>
		/// Gets the bounds of the element.
		/// </summary>
		/// <value>The bounds.</value>
		public override BoxRegion Bounds{
			get{
				return GroupBounds;
			}
		}
		
		/// <summary>
		/// Gets the <see cref="GraphicsPath"/> for this element.
		/// </summary>
		/// <value></value>
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			return GetPaths(context,renderer);
		}
		
		/// <summary>
		/// Renders the <see cref="SVGElement"/> and contents to the specified <see cref="Graphics"/> object.
		/// </summary>
		/// <param name="renderer">The <see cref="Graphics"/> object to render to.</param>
		public override void BuildFilter(RenderContext renderer){
			
			if (Visibility==VisibilityMode.Hidden){
				return;
			}
			
			renderer.PushTransform(this);
			SetClip(renderer);
			BuildChildren(renderer);
			ResetClip(renderer);
			renderer.PopTransform(this);
			
		}
		
	}
}
