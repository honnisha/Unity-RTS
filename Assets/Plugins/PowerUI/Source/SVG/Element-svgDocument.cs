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
using Svg;
using Css;


namespace Dom{
	
	public partial class Element{
		
		/// <summary>Gets the SVG document from a background-image (or img elements).</summary>
		public SVGDocument svgDocument{
			get{
				var bg=(this as IRenderableNode).RenderData.BGImage;
				
				if(bg==null || bg.Image==null || bg.Image.Contents==null){
					return null;
				}
				
				// Get as an SVG format:
				SVGFormat contents=bg.Image.Contents as SVGFormat;
				
				if(contents==null || contents.Svg==null){
					return null;
				}
				
				return contents.Svg.document as SVGDocument;
			}
		}
		
	}
	
}