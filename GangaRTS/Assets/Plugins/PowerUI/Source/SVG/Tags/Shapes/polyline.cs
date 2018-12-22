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


namespace Svg{
	
	/// <summary>
	/// An SVG polyline tag.
	/// </summary>
	
	[Dom.TagName("polyline")]
	public class SVGPolylineElement:SVGGeometryElement, SVGAnimatedPoints{
		
		/// <summary>A list of points.</summary>
		public SVGPointList points{
			get{
				return new SVGPointList(false,Polygon.Points,this,"points");
			}
		}
		
		/// <summary>A list of animated points.</summary>
		public SVGPointList animatedPoints{
			get{
				return new SVGPointList(false,Polygon.Points,this,"points");
			}
		}
		
		public SVGPolylineElement(){
			Shape=new PolygonProvider();
			// Don't "close" the path:
			Polygon.Close=false;
			
		}
		
		/// <summary>The hosting shape as a polygon.</summary>
		protected PolygonProvider Polygon{
			get{
				return Shape as PolygonProvider;
			}
		}
		
		public Css.Value PointsValue{
			
			get{
				return Polygon.Points;
			}
			set{
				Polygon.Points=value;
				RebuildPath();
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="points"){
				
				PointsValue=Css.Value.Load(getAttribute("points"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
		/// <summary>
		/// Renders the stroke of the <see cref="SvgVisualElement"/> to the specified <see cref="ISvgRenderer"/>
		/// </summary>
		/// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
		protected internal override bool BuildStroke(VectorPath path,RenderContext renderer){
			
			return BuildStrokeMarkers(path,renderer);
			
		}
		
	}
	
}