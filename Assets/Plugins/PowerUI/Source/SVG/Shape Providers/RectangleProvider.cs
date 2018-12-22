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

using System;
using Dom;
using Blaze;
using UnityEngine;


namespace Svg{
	
	/// <summary>
	/// An SVG rectangle.
	/// </summary>
	
	public class RectangleProvider:ShapeProvider{
		
		private Css.Value _cornerRadiusX=Css.Value.Empty;
		private Css.Value _cornerRadiusY=Css.Value.Empty;
		private Css.Value _x=Css.Value.Empty;
		private Css.Value _y=Css.Value.Empty;
		private Css.Value _width=Css.Value.Empty;
		private Css.Value _height=Css.Value.Empty;
		
		
		public Css.Value CornerRadiusX{
			
			get{
				return _cornerRadiusX;
			}
			set{
				
				if(_cornerRadiusX.Equals(value)){
					return;
				}
				
				_cornerRadiusX = value;
				ClearCache();
			}
		}
		
		public Css.Value CornerRadiusY{
			
			get{
				return _cornerRadiusY;
			}
			set{
				
				if(_cornerRadiusY.Equals(value)){
					return;
				}
				
				_cornerRadiusY = value;
				ClearCache();
			}
		}
		
		public Css.Value X{
			
			get{
				return _x;
			}
			set{
				
				if(_x.Equals(value)){
					return;
				}
				
				_x = value;
				ClearCache();
			}
		}
		
		public Css.Value Y{
			
			get{
				return _y;
			}
			set{
				
				if(_y.Equals(value)){
					return;
				}
				
				_y = value;
				ClearCache();
			}
		}
		
		public Css.Value Width{
			
			get{
				return _width;
			}
			set{
				
				if(_width.Equals(value)){
					return;
				}
				
				_width = value;
				ClearCache();
			}
		}
		
		public Css.Value Height{
			
			get{
				return _height;
			}
			set{
				
				if(_height.Equals(value)){
					return;
				}
				
				_height = value;
				ClearCache();
			}
		}
		
		/// <summary>
		/// Gets the path representing this element.
		/// </summary>
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			Css.RenderableData rd=context.RenderData;
			
			// Get w/h:
			float width=Width.GetDecimal(rd,ViewportAxis.X);
			float height=Height.GetDecimal(rd,ViewportAxis.Y);
			
			if (width <= 0f && height > 0f){
				
				return null;
				
			}
			
			if(_Path==null){
				
				_Path = new VectorPath();
				
				// Get corner radius:
				float rx=CornerRadiusX.GetDecimal(rd,ViewportAxis.X);
				float ry=CornerRadiusY.GetDecimal(rd,ViewportAxis.Y);
				
				// Get x/y:
				float x=X.GetDecimal(rd,ViewportAxis.X);
				float y=Y.GetDecimal(rd,ViewportAxis.Y);
				
				// Note: This goes clockwise (like the other standard shapes).
				
				// If the corners aren't to be rounded just create a rectangle
				if (rx == 0f && ry == 0f){
					
					// Ordinary rectangle.
					_Path.MoveTo(x,y);
					_Path.LineTo(x,y+height);
					_Path.LineTo(x+width,y+height);
					_Path.LineTo(x+width,y);
					_Path.ClosePath();
					
				}else{
					
					// Clip the corner radius:
					rx = (float)Math.Min(rx * 2, width);
					ry = (float)Math.Min(ry * 2, height);
					
					// Get the C values (used to shape the 4 corners arcs - see CircleProvider for some clarity):
					float cx=(CircleProvider.BezierC * rx);
					float cy=(CircleProvider.BezierC * ry);
					
					float limit=x + width * 0.5f;
					
					// The start/ end of arcs from the left along x.
					float leftArcX=Math.Min(x + rx, limit);
					// The start/ end of arcs from the right along x.
					float rightArcX=Math.Max(x + width - rx, limit);
					
					limit=y + height * 0.5f;
					
					// The start/ end of arcs from the bottom along y.
					float bottomArcY=Math.Min(y + ry, limit);
					// The start/ end of arcs from the top along y.
					float topArcY=Math.Max(y + height - ry, limit);
					
					// Start from bottom left:
					_Path.MoveTo(
						leftArcX,
						y
					);
					
					// First arc (bottom left):
					_Path.CurveTo(
						leftArcX-cx, y,
						x, bottomArcY-cy,
						x, bottomArcY
					);
					
					// Up the left edge:
					_Path.LineTo(x,topArcY);
					
					// Top left arc:
					_Path.CurveTo(
						x, topArcY+cy,
						leftArcX-cx, y+height,
						leftArcX, y+height
					);
					
					// Along the top edge:
					_Path.LineTo(rightArcX,y);
					
					// Top right arc:
					_Path.CurveTo(
						rightArcX + cx, y,
						x+width, topArcY+cy,
						x+width, topArcY
					);
					
					// Down the right edge:
					_Path.LineTo(x+width,bottomArcY);
					
					// Bottom right arc:
					_Path.CurveTo(
						x+width, bottomArcY-cy,
						rightArcX+cx, y,
						rightArcX, y
					);
					
					// Line along the bottom!
					_Path.ClosePath();
					
				}
				
			}
			
			return _Path;
			
		}
		
	}
	
}