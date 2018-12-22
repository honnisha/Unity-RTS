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
using UnityEngine;
using Css;


namespace Svg{
	
	/// <summary>
	/// Shared functionality for SVG geometry elements (path, circle etc).
	/// </summary>
	
	public class SVGGeometryElement:SVGGraphicsElement{
		
		/// <summary>The loaded path.</summary>
		public ShapeProvider Shape;
		
		
		public void RebuildPath(){
			
			// Request a new redraw now!
			
		}
		
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			return Shape.GetPath(context,renderer);
			
		}
		
		/*
		
		public override void RenderStroke(VectorPath path,RenderContext ctx){
			
			#warning stroke!
			
			/*
			var result = base.RenderStroke(renderer);
            
            if (this.MarkerStart != null)
            {
                SvgMarker marker = this.OwnerDocument.GetElementById<SvgMarker>(this.MarkerStart.ToString());
                marker.RenderMarker(renderer, this, path.PathPoints[0], path.PathPoints[0], path.PathPoints[1]);
            }

            if (this.MarkerMid != null)
            {
                SvgMarker marker = this.OwnerDocument.GetElementById<SvgMarker>(this.MarkerMid.ToString());
                for (int i = 1; i <= path.PathPoints.Length - 2; i++)
                    marker.RenderMarker(renderer, this, path.PathPoints[i], path.PathPoints[i - 1], path.PathPoints[i], path.PathPoints[i + 1]);
            }

            if (this.MarkerEnd != null)
            {
                SvgMarker marker = this.OwnerDocument.GetElementById<SvgMarker>(this.MarkerEnd.ToString());
                marker.RenderMarker(renderer, this, path.PathPoints[path.PathPoints.Length - 1], path.PathPoints[path.PathPoints.Length - 2], path.PathPoints[path.PathPoints.Length - 1]);
            }
			
		}
		*/
		
		/// <summary>
		/// Gets the bounds of the element.
		/// </summary>
		/// <value>The bounds.</value>
		public override BoxRegion Bounds{
			get{
				
				VectorPath path=Shape.Path;
				
				if(path==null){
					// None!
					return BoxRegion.Empty;
				}
				
				if(path.Width==0f){
					path.RecalculateBounds();
				}
				
				return new BoxRegion(path.MinX,path.MinY,path.Width,path.Height);
				
			}
		}
		
	}
	
}