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
using Blaze;
using Dom;
using Css;


namespace Svg{

	/// <summary>
	/// Represents the use tag.
	/// </summary>
	[Dom.TagName("use")]
	public class SVGUseElement:SVGElement{
		
		/// <summary>The element targeted with the href.</summary>
		private SVGElement _targetElement;
		
		/// <summary>The element targeted with the href.</summary>
		public SVGElement Target{
			get{
				
				if(_targetElement==null){
					_targetElement=TryResolveHref();
				}
				
				return _targetElement;
			}
		}
		
		/// <summary>The URL of the ref'd element.</summary>
		public string ReferencedElement{
			get{
				return getAttribute("href");
			}
			set{
				setAttribute("href", value);
			}
		}
		
		public override BoxRegion Bounds{
			get{
				return new BoxRegion(0,0,0,0);
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="x"){
				
				
				
			}else if(property=="y"){
				
				
				
			}else if(property=="href"){
				
				// Try resolve now:
				_targetElement=TryResolveHref();
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>
		/// Applies the required transforms to <see cref="ISvgRenderer"/>.
		/// </summary>
		/// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
		protected override bool PushTransforms(RenderContext renderer){
			
			if(!base.PushTransforms(renderer)){
				return false;
			}
			
			RenderableData rd=RenderData;
			
			renderer.PushMatrix(
				renderer.TranslateMatrix(
					rd.OffsetLeft,
					rd.OffsetTop
				)
			);
			
			return true;
		}
		
		/// <summary>
		/// Applies the required transforms to <see cref="ISvgRenderer"/>.
		/// </summary>
		/// <param name="renderer">The <see cref="ISvgRenderer"/> to be transformed.</param>
		protected override void PopTransforms(RenderContext renderer){
			
			// Pop transform:
			renderer.PopTransform();
			
			// Pop element:
			base.PopTransforms(renderer);
			
		}
		
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			SVGElement element = Target;
			return (element != null) ? (element as SVGElement).GetPath(element,renderer) : null;
			
		}
		
		public override void BuildFilter(RenderContext renderer){
			
			if (Visibility==VisibilityMode.Hidden){
				return;
			}
			
			PushTransforms(renderer);
			
			SetClip(renderer);
			
			SVGElement element = Target;
			
			if(element!=null){
				
				Node origParent = element.parentNode;
				element.parentNode_ = this;
				element.BuildFilter(renderer);
				element.parentNode_ = origParent;
				
			}

			ResetClip(renderer);
			PopTransforms(renderer);
			
		}
		
	}
}