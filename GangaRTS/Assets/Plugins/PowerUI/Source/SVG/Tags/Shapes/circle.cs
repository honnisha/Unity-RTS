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
	/// An SVG circle tag.
	/// </summary>
	
	[Dom.TagName("circle")]
	public class SVGCircleElement:SVGGeometryElement{
		
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
		
		/// <summary>Radius.</summary>
		public SVGAnimatedLength r{
			get{
				return new SVGAnimatedLength(Radius,this,"r");
			}
		}
		
		public SVGCircleElement(){
			Shape=new CircleProvider();
		}
		
		/// <summary>The hosting shape as a circle.</summary>
		private CircleProvider Circle{
			get{
				return Shape as CircleProvider;
			}
		}
		
		public Css.Value Radius{
			
			get{
				return Circle.Radius;
			}
			set{
				Circle.Radius=value;
				RebuildPath();
			}
		}
		
		public Css.Value CenterX{
			
			get{
				return Circle.CenterX;
			}
			set{
				Circle.CenterX=value;
				RebuildPath();
			}
		}
		
		public Css.Value CenterY{
			
			get{
				return Circle.CenterY;
			}
			set{
				Circle.CenterY=value;
				RebuildPath();
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="cx"){
				
				CenterX=Css.Value.Load(getAttribute("cx"));
				
			}else if(property=="cy"){
				
				CenterY=Css.Value.Load(getAttribute("cy"));
				
			}else if(property=="r"){
				
				Radius=Css.Value.Load(getAttribute("r"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}