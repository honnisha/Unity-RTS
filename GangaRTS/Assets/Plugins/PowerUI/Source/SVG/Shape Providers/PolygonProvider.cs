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
	/// An SVG polygon.
	/// </summary>
	
	public class PolygonProvider:ShapeProvider{
		
		/// <summary>True if the shape should be closed.</summary>
		public bool Close=true;
		private Css.Value _points=Css.Value.Empty;
		
		
		public Css.Value Points{
			
			get{
				return _points;
			}
			set{
				
				if(_points.Equals(value)){
					return;
				}
				
				_points = value;
				ClearCache();
			}
		}
		
		/// <summary>
		/// Gets the path representing this element.
		/// </summary>
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			Css.RenderableData rd=context.RenderData;
			
			if(_Path==null){
				
				_Path = new VectorPath();
				
				try{
					
					Css.Value points = Points;
					int count=points.Count;
					
					for (int i = 2; (i + 1) < count; i += 2){
						
						float endPointX=points[i].GetDecimal(rd,ViewportAxis.X);
						float endPointY=points[i+1].GetDecimal(rd,ViewportAxis.Y);
						
						if(Close){
							
							//first line
							if (_Path.FirstPathNode==null){
								
								// Wrap around:
								float startPointX=points[i - 2].GetDecimal(rd,ViewportAxis.X);
								float startPointY=points[i - 1].GetDecimal(rd,ViewportAxis.Y);
								
								_Path.MoveTo(startPointX,startPointY);
							}
							
							_Path.LineTo(endPointX,endPointY);
							
						}else{
							
							// It's a polyline
							
							//first line
							if (_Path.FirstPathNode==null){
								_Path.MoveTo(endPointX,endPointY);
							}else{
								_Path.LineTo(endPointX,endPointY);
							}
							
						}
						
					}
					
				}
				catch{
					Dom.Log.Add("Warning: Failed to parse a set of points for a polygon definition in either an SVG or your CSS.");
				}
				
			}
			
			return _Path;
			
		}
		
	}
	
}