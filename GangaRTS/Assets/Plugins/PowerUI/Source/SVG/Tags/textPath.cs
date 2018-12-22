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

namespace Svg{
	
	/// <summary>
	/// The parent SVG tag.
	/// </summary>
	
	[Dom.TagName("textpath")]
	public class SVGTextPathElement:SVGTextContentElement{
		
		public string ReferencedPath;
		
		public TextPathMethod Method=TextPathMethod.Align;
		
		public TextPathSpacing Spacing=TextPathSpacing.Exact;
		
		
		public override bool OnAttributeChange(string property){
			
			// Note that base handles width, height, x, y etc.
			
			if(property=="href"){
				
				ReferencedPath=getAttribute("href");
				
			}else if(property=="startoffset"){
				
				StartOffset=Css.Value.Load(getAttribute("startoffset"));
				
			}else if(property=="method"){
				
				Method=ValueHelpers.GetPathMethod(getAttribute("method"));
				
			}else if(property=="spacing"){
				
				Spacing=ValueHelpers.GetPathSpacing(getAttribute("spacing"));
				
			}else if(!base.OnAttributeChange(property)){
				
				return false;
				
			}
			
			
			return true;
		}
		
		public override Css.ValueSet Dx{
			get{
				return null;
			}
			set{}
		}
		
		public virtual Css.Value StartOffset{
			
			get{
				return (_dx.Count < 1 ? Css.Value.Empty : _dx[0]);
			}
			set{
				
				if(_dx.Count < 1){
					
					_dx.Add(value);
					
				}else{
					
					_dx[0] = value;
					
				}
				
			}
			
		}
		
	}
	
}
