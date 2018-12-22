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
using Css;


namespace Svg{
	
	/// <summary>
	/// An SVG rect tag.
	/// </summary>
	
	[Dom.TagName("rect")]
	public class SVGRectangleElement:SVGGeometryElement{
		
		public SVGRectangleElement(){
			Shape=new RectangleProvider();
		}
		
		/// <summary>The hosting shape as a rectangle.</summary>
		private RectangleProvider Rectangle{
			get{
				return Shape as RectangleProvider;
			}
		}
		
		public Css.Value X{
			
			get{
				return Rectangle.X;
			}
			set{
				Rectangle.X=value;
				RebuildPath();
			}
		}
		
		public Css.Value Y{
			
			get{
				return Rectangle.Y;
			}
			set{
				Rectangle.Y=value;
				RebuildPath();
			}
		}
		
		public Css.Value CornerRadiusX{
			
			get{
				return Rectangle.CornerRadiusX;
			}
			set{
				Rectangle.CornerRadiusX=value;
				RebuildPath();
			}
		}
		
		public Css.Value CornerRadiusY{
			
			get{
				return Rectangle.CornerRadiusY;
			}
			set{
				Rectangle.CornerRadiusY=value;
				RebuildPath();
			}
		}
		
		private Css.Value Width{
			
			get{
				return Style.Computed[Css.Properties.Width.GlobalProperty];
			}
			
		}
		
		private Css.Value Height{
			
			get{
				return Style.Computed[Css.Properties.Height.GlobalProperty];
			}
			
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			RectangleProvider rect=Rectangle;
			
			// Check to see if w/h was updated via CSS:
			Css.Value newWidth=Width;
			Css.Value newHeight=Height;
			
			if(rect.Width!=newWidth || rect.Height!=newHeight){
				
				rect.Width=newWidth;
				rect.Height=newHeight;
				RebuildPath();
				
			}
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="x"){
				
				X=Css.Value.Load(getAttribute("x"));
				
			}else if(property=="y"){
				
				Y=Css.Value.Load(getAttribute("y"));
				
			}else if(property=="rx"){
				
				CornerRadiusX=Css.Value.Load(getAttribute("rx"));
				
			}else if(property=="ry"){
				
				CornerRadiusY=Css.Value.Load(getAttribute("ry"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}