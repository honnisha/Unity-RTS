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
	/// A circle.
	/// </summary>
	
	public class CircleProvider:ShapeProvider{
		
		/// <summary>The C value for a cubic bezier curve in order to closely approx a circle.</summary>
		public const float BezierC=0.55191f;
		
		private Css.Value _radius=Css.Value.Empty;
		private Css.Value _centerX=Css.Value.Empty;
		private Css.Value _centerY=Css.Value.Empty;
		
		
		public Css.Value Radius{
			
			get{
				return _radius;
			}
			set{
				
				if(_radius.Equals(value)){
					return;
				}
				
				_radius = value;
				ClearCache();
			}
		}
		
		public Css.Value CenterX{
			
			get{
				return _centerX;
			}
			set{
				
				if(_centerX.Equals(value)){
					return;
				}
				
				_centerX = value;
				ClearCache();
			}
		}
		
		public Css.Value CenterY{
			
			get{
				return _centerY;
			}
			set{
				
				if(_centerY.Equals(value)){
					return;
				}
				
				_centerY = value;
				ClearCache();
			}
		}
		
		/// <summary>
		/// Gets the path representing this element.
		/// </summary>
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			Css.RenderableData rd=context.RenderData;
			
			// Don't build the path if there's no radius:
			float radius = Radius.GetDecimal(rd,ViewportAxis.None);
			
			if(radius<=0){
				return null;
			}
			
			if(_Path==null){
				
				// Don't need to consider stroke width.
				
				_Path = new VectorPath();
				
				float centerX = CenterX.GetDecimal(rd,ViewportAxis.X);
				float centerY = CenterX.GetDecimal(rd,ViewportAxis.Y);
				
				// Get the C values:
				float cX=BezierC * radius;
				float cY=cX;
				
				// Offset to match the center:
				cX+=centerX;
				cY+=centerY;
				
				float radiusX=centerX+radius;
				float radiusY=centerY+radius;
				
				float nRadiusX=centerX-radius;
				float nRadiusY=centerY-radius;
				
				_Path.MoveTo(centerX,radiusY);
				
				// First quadrant (top right, going clockwise):
				_Path.CurveTo(cX,radiusY,radiusX,cY,radiusX,centerY);
				
				// Bottom right:
				_Path.CurveTo(radiusX,-cY,cX,nRadiusY,centerX,nRadiusY);
				
				// Bottom left:
				_Path.CurveTo(-cX,nRadiusY,nRadiusX,-cY,nRadiusX,centerY);
				
				// Top left:
				_Path.CurveTo(nRadiusX,cY,-cX,radiusY,centerX,radiusY);
				
				// Mark as closed:
				_Path.LatestPathNode.IsClose=true;
				
			}
			
			return _Path;
		}
		
	}
	
}