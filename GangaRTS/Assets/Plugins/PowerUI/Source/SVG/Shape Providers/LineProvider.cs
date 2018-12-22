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


namespace Svg{
	
	/// <summary>
	/// An SVG line.
	/// </summary>
	
	public class LineProvider:ShapeProvider{
		
		private Css.Value _startX=Css.Value.Empty;
		private Css.Value _startY=Css.Value.Empty;
		private Css.Value _endX=Css.Value.Empty;
		private Css.Value _endY=Css.Value.Empty;
		
		
		public Css.Value StartX{
			
			get{
				return _startX;
			}
			set{
				
				if(_startX.Equals(value)){
					return;
				}
				
				_startX = value;
				ClearCache();
			}
		}
		
		public Css.Value StartY{
			
			get{
				return _startY;
			}
			set{
				
				if(_startY.Equals(value)){
					return;
				}
				
				_startY = value;
				ClearCache();
			}
		}
		
		public Css.Value EndX{
			
			get{
				return _endX;
			}
			set{
				
				if(_endX.Equals(value)){
					return;
				}
				
				_endX = value;
				ClearCache();
			}
		}
		
		public Css.Value EndY{
			
			get{
				return _endY;
			}
			set{
				
				if(_endY.Equals(value)){
					return;
				}
				
				_endY = value;
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
				
				float startX=StartX.GetDecimal(rd,ViewportAxis.X);
				float startY=StartY.GetDecimal(rd,ViewportAxis.Y);
				
				float endX=EndX.GetDecimal(rd,ViewportAxis.X);
				float endY=EndY.GetDecimal(rd,ViewportAxis.Y);
				
				_Path.MoveTo(startX,startY);
				_Path.LineTo(endX,endY);
				
			}
			
			return _Path;
			
		}
		
	}
	
}