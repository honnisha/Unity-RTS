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
	/// The SVG symbol tag.
	/// </summary>
	
	[Dom.TagName("symbol")]
	public class SVGSymbolElement:SVGElement{
		
		/// <summary>The aspect of the symbol.</summary>
		public AspectRatio AspectRatio=new AspectRatio();
		/// <summary>The viewbox of the symbol.</summary>
		public BoxRegion Viewbox;
		
		
		public override bool OnAttributeChange(string property){
			
			// Note that base handles width, height, x, y etc.
			
			if(property=="viewbox"){
				
				// SVG viewbox
				Viewbox=ValueHelpers.GetViewbox(getAttribute("viewbox"));
				
			}else if(property=="preserveaspectratio"){
				
				// Aspect ratio
				AspectRatio=new AspectRatio(getAttribute("preserveaspectratio"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
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
		/// Applies the required transforms to <see cref="ISvgRenderer"/>.
		/// </summary>
		/// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
		protected override bool PushTransforms(RenderContext renderer){
			
			if(!base.PushTransforms(renderer)){
				return false;
			}
			
			renderer.AddViewBoxTransform(Viewbox,AspectRatio,null);
			
			return true;
			
		}
		
		/// <summary>
		/// Applies the required transforms to <see cref="ISvgRenderer"/>.
		/// </summary>
		/// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
		protected override void PopTransforms(RenderContext renderer){
			
			// Pop viewbox:
			renderer.PopTransform();
			
			// Pop element:
			base.PopTransforms(renderer);
			
		}
		
		// Only builds if the parent is set to a Use element
		public override void BuildFilter(RenderContext renderer){
			
			if (parentElement is SVGUseElement){
				base.BuildFilter(renderer);
			}
			
		}
		
	}
}
