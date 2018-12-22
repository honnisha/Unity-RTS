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
using Css;


namespace Svg{
	
	/// <summary>
	/// The parent SVG tag.
	/// </summary>
	
	[Dom.TagName("svg")]
	public class SVGSVGElement:SVGElement{
		
		/// <summary>How this SVG deals with overflow.</summary>
		public Overflow Overflow=Overflow.Auto;
		/// <summary>The aspect of the SVG.</summary>
		public AspectRatio AspectRatio=new AspectRatio();
		/// <summary>The viewbox of the SVG.</summary>
		public BoxRegion Viewbox;
		
		
		public SVGSVGElement(){
			
			// Create the rendering context now:
			Context=new RenderContext(this);
			
		}
		
		/// <summary>
		/// Applies the required transforms to <see cref="RenderContext"/>.
		/// </summary>
		/// <param name="renderer">The <see cref="RenderContext"/> to be transformed.</param>
		protected override bool PushTransforms(RenderContext renderer){
			
			if(!base.PushTransforms(renderer)){
				return false;
			}
			
			if(Viewbox!=null){
				renderer.AddViewBoxTransform(Viewbox,AspectRatio,this);
			}
			
			return true;
		}
		
		public override void BuildFilter(RenderContext renderer){
			
			switch (Overflow)
			{
				case Overflow.Auto:
				case Overflow.Visible:
				case Overflow.Scroll:
					
					base.BuildFilter(renderer);
					break;
				default:
					ScreenRegion prevClip = renderer.ClipRegion;
					
					try{
						
						Css.ComputedStyle cs=Style.Computed;
						
						renderer.SetClip(new BoxRegion(cs.OffsetLeft,cs.OffsetTop,cs.PixelWidth,cs.PixelHeight), false);
						
						base.BuildFilter(renderer);
					}finally{
						renderer.SetClip(prevClip, true);
					}
					
					break;
			}
			
		}
		
		internal override void AddedToDOM(){
			
			// Create a virtual which tells the rendering engine 
			// that the kids of this element are handled here:
			
			if(RenderData.Virtuals==null){
				RenderData.Virtuals=new Css.VirtualElements();
			}
			
			// Selects are unusual in that they don't draw their own childnodes:
			RenderData.Virtuals.AllowDrawKids=false;
			
			base.AddedToDOM();
			
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			if(document is SVGDocument){
				return;
			}
			
			// Occurs on inline SVG's.
			
			// Set the size:
			Context.SetSize((int)box.InnerWidth,(int)box.InnerHeight);
			
			UnityEngine.Texture tex=Context.Texture;
			
			if(tex==null){
				return;
			}
			
			// Update the background raw image:
			BackgroundImage img=RenderData.BGImage;
			
			if(img==null){
				img=new Css.BackgroundImage(RenderData);
				RenderData.BGImage=img;
			}
			
			// Update the bg image:
			img.UpdateImage(tex);
			
		}
		
		public override bool OnAttributeChange(string property){
			
			// Note that base handles width, height, x, y etc.
			
			if(property=="viewbox"){
				
				// SVG viewbox
				Viewbox=ValueHelpers.GetViewbox(getAttribute("viewbox"));
				
			}else if(property=="overflow"){
				
				// Overflow
				Overflow=ValueHelpers.GetOverflow(getAttribute("overflow"));
				
			}else if(property=="preserveaspectratio"){
				
				// Aspect ratio
				AspectRatio=new AspectRatio(getAttribute("preserveaspectratio"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}