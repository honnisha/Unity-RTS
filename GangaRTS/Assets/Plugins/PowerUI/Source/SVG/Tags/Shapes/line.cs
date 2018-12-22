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
	/// An SVG line tag.
	/// </summary>
	
	[Dom.TagName("line")]
	public class SVGLineElement:SVGGeometryElement{
		
		/// <summary>Start x.</summary>
		public SVGAnimatedLength x1{
			get{
				return new SVGAnimatedLength(StartX,this,"x1");
			}
		}
		
		/// <summary>Start y.</summary>
		public SVGAnimatedLength y1{
			get{
				return new SVGAnimatedLength(StartY,this,"y1");
			}
		}
		
		/// <summary>Start x.</summary>
		public SVGAnimatedLength x2{
			get{
				return new SVGAnimatedLength(EndX,this,"x2");
			}
		}
		
		/// <summary>Start y.</summary>
		public SVGAnimatedLength y2{
			get{
				return new SVGAnimatedLength(EndY,this,"y2");
			}
		}
		
		public SVGLineElement(){
			Shape=new LineProvider();
		}
		
		/// <summary>The hosting shape as a line.</summary>
		private LineProvider Line{
			get{
				return Shape as LineProvider;
			}
		}
		
		public Css.Value StartX{
			
			get{
				return Line.StartX;
			}
			set{
				Line.StartX=value;
				RebuildPath();
			}
		}
		
		public Css.Value StartY{
			
			get{
				return Line.StartY;
			}
			set{
				Line.StartY=value;
				RebuildPath();
			}
		}
		
		public Css.Value EndX{
			
			get{
				return Line.EndX;
			}
			set{
				Line.EndX=value;
				RebuildPath();
			}
		}
		
		public Css.Value EndY{
			
			get{
				return Line.EndY;
			}
			set{
				Line.EndY=value;
				RebuildPath();
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="x1"){
				
				StartX=Css.Value.Load(getAttribute("x1"));
				
			}else if(property=="y1"){
				
				StartY=Css.Value.Load(getAttribute("y1"));
				
			}else if(property=="x2"){
				
				EndX=Css.Value.Load(getAttribute("x2"));
				
			}else if(property=="y2"){
				
				EndY=Css.Value.Load(getAttribute("y2"));
				
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