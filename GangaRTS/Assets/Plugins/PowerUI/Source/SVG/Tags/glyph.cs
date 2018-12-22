//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using Dom;
using Blaze;
using UnityEngine;
using InfiniText;
using Css;


namespace Svg{
	
	/// <summary>
	/// The SVG glyph tag.
	/// </summary>
	
	[Dom.TagName("glyph")]
	public class SVGGlyphElement:SVGElement{
		
		/// <summary>The infinitext glyph.</summary>
		private Glyph Glyph=new Glyph();
		
		public virtual string GlyphName{
			get{
				return getAttribute("glyph-name");
			}
			set{
				setAttribute("glyph-name", value);
			}
		}
		
		public string Unicode{
			get{
				return getAttribute("unicode");
			}
			set{
				setAttribute("unicode", value);
			}
		}
		
		public float HorizAdvX=float.MaxValue;
		public float VertAdvY=float.MaxValue;
		public float VertOriginX=float.MaxValue;
		public float VertOriginY=float.MaxValue;
		
		
		public override bool OnAttributeChange(string property){
			
			if(property=="d"){
				
				// Path data here!
				Glyph.Clear();
				
				PathString.Load(getAttribute("d"),Glyph);
				
			}else if(property=="glyph-name" || property=="unicode"){
				
				// Just ignore this one (we want it so we can return true).
				
			}else if(property=="horiz-adv-x"){
				
				HorizAdvX=GetFloatAttribute("horiz-adv-x",float.MaxValue);
				
				if(HorizAdvX==float.MaxValue){
					
					// Pull from parent font:
					SVGFontElement parentFont=ParentFont;
					
					HorizAdvX=(parentFont==null)?0:parentFont.HorizAdvX;
					
				}
				
			}else if(property=="vert-adv-y"){
				
				VertAdvY=GetFloatAttribute("vert-adv-y",float.MaxValue);
				
				if(VertAdvY==float.MaxValue){
					
					// Pull from parent font:
					SVGFontElement parentFont=ParentFont;
					
					VertAdvY=(parentFont==null)?0:parentFont.VertAdvY;
					
				}
				
			}else if(property=="vert-origin-x"){
				
				VertOriginX=GetFloatAttribute("vert-origin-x",float.MaxValue);
				
				if(VertOriginX==float.MaxValue){
					
					// Pull from parent font:
					SVGFontElement parentFont=ParentFont;
					
					VertOriginX=(parentFont==null)?0:parentFont.VertOriginX;
					
				}
				
			}else if(property=="vert-origin-y"){
				
				VertOriginY=GetFloatAttribute("vert-origin-y",float.MaxValue);
				
				if(VertOriginY==float.MaxValue){
					
					// Pull from parent font:
					SVGFontElement parentFont=ParentFont;
					
					VertOriginY=(parentFont==null)?0:parentFont.VertOriginY;
					
				}
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
			
		}
		
		public override void OnChildrenLoaded(){
			// Pull defaults if needed.
			
			SVGFontElement parentFont=ParentFont;
			
			bool noParent=(parentFont==null);
			
			if(HorizAdvX==float.MaxValue){
				
				HorizAdvX=noParent?0:parentFont.VertAdvY;
				
			}
			
			if(VertAdvY==float.MaxValue){
				
				VertAdvY=noParent?0:parentFont.VertAdvY;
				
			}
			
			if(VertOriginX==float.MaxValue){
				
				VertOriginX=noParent?0:parentFont.VertOriginX;
				
			}
			
			if(VertOriginY==float.MaxValue){
				
				VertOriginY=noParent?0:parentFont.VertOriginY;
				
			}
			
			// Base:
			base.OnChildrenLoaded();
			
		}
		
		/*
		/// <summary>
		/// Gets the <see cref="GraphicsPath"/> for this element.
		/// </summary>
		public override GraphicsPath Path(ISvgRenderer renderer)
		{
			if (this._path == null || this.IsPathDirty)
			{
				_path = new GraphicsPath();

				foreach (SvgPathSegment segment in this.PathData)
				{
					segment.AddToPath(_path);
				}

				this.IsPathDirty = false;
			}
			return _path;
		}
		*/
		
		/// <summary>
		/// Gets the bounds of the element.
		/// </summary>
		/// <value>The bounds.</value>
		public override BoxRegion Bounds
		{
			get {
				
				return new BoxRegion(Glyph.MinX,Glyph.MinY,Glyph.Width,Glyph.Height);
				
			}
		}
		
	}
}
