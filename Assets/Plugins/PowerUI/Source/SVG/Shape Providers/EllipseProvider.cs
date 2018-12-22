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
	/// An SVG ellipse.
	/// </summary>
	
	public class EllipseProvider:ShapeProvider{
		
		private Css.Value _radiusX=Css.Value.Empty;
		private Css.Value _radiusY=Css.Value.Empty;
		private Css.Value _centerX=Css.Value.Empty;
		private Css.Value _centerY=Css.Value.Empty;
		
		
		public Css.Value RadiusX{
			
			get{
				return _radiusX;
			}
			set{
				
				if(_radiusX.Equals(value)){
					return;
				}
				
				_radiusX = value;
				ClearCache();
			}
		}
		
		public Css.Value RadiusY{
			
			get{
				return _radiusY;
			}
			set{
				
				if(_radiusY.Equals(value)){
					return;
				}
				
				_radiusY = value;
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
			float radiusX = RadiusX.GetDecimal(rd,ViewportAxis.X);
			float radiusY = RadiusY.GetDecimal(rd,ViewportAxis.Y);
			
			if(radiusX<=0f || radiusY<=0f){
				return null;
			}
			
			if(_Path==null){
				
				// Don't need to consider stroke width.
				
				_Path = new VectorPath();
				
				float centerX = CenterX.GetDecimal(rd,ViewportAxis.X);
				float centerY = CenterX.GetDecimal(rd,ViewportAxis.Y);
				
				// Get the C values:
				float cX=centerX + (CircleProvider.BezierC * radiusX);
				float cY=centerY + (CircleProvider.BezierC * radiusY);
				
				float nRadiusX=centerX-radiusX;
				float nRadiusY=centerY-radiusY;
				
				// Offset radius:
				radiusX+=centerX;
				radiusY+=centerY;
				
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