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
	/// An SVG ellipse tag.
	/// </summary>
	
	[Dom.TagName("ellipse")]
	public class SVGEllipseElement:SVGGeometryElement{
		
		/// <summary>Center x.</summary>
		public SVGAnimatedLength cx{
			get{
				return new SVGAnimatedLength(CenterX,this,"cx");
			}
		}
		
		/// <summary>Center y.</summary>
		public SVGAnimatedLength cy{
			get{
				return new SVGAnimatedLength(CenterY,this,"cy");
			}
		}
		
		/// <summary>Radius x.</summary>
		public SVGAnimatedLength rx{
			get{
				return new SVGAnimatedLength(RadiusX,this,"rx");
			}
		}
		
		/// <summary>Radius y.</summary>
		public SVGAnimatedLength ry{
			get{
				return new SVGAnimatedLength(RadiusY,this,"ry");
			}
		}
		
		public SVGEllipseElement(){
			Shape=new EllipseProvider();
		}
		
		/// <summary>The hosting shape as an ellipse.</summary>
		private EllipseProvider Ellipse{
			get{
				return Shape as EllipseProvider;
			}
		}
		
		public Css.Value RadiusX{
			
			get{
				return Ellipse.RadiusX;
			}
			set{
				Ellipse.RadiusX=value;
				RebuildPath();
			}
		}
		
		public Css.Value RadiusY{
			
			get{
				return Ellipse.RadiusY;
			}
			set{
				Ellipse.RadiusY=value;
				RebuildPath();
			}
		}
		
		public Css.Value CenterX{
			
			get{
				return Ellipse.CenterX;
			}
			set{
				Ellipse.CenterX=value;
				RebuildPath();
			}
		}
		
		public Css.Value CenterY{
			
			get{
				return Ellipse.CenterY;
			}
			set{
				Ellipse.CenterY=value;
				RebuildPath();
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="cx"){
				
				CenterX=Css.Value.Load(getAttribute("cx"));
				
			}else if(property=="cy"){
				
				CenterY=Css.Value.Load(getAttribute("cy"));
				
			}else if(property=="rx"){
				
				RadiusX=Css.Value.Load(getAttribute("rx"));
				
			}else if(property=="ry"){
				
				RadiusY=Css.Value.Load(getAttribute("ry"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}