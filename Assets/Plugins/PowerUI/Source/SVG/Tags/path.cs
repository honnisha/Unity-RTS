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
	/// The SVG path tag.
	/// </summary>
	
	[Dom.TagName("path")]
	public class SVGPathElement:SVGGeometryElement{
		
		public SVGPathElement(){
			Shape=new ShapeProvider();
			Shape.SetPath(new VectorPath());
		}
		
		public override bool OnAttributeChange(string property){
			
			// Test width/height first:
			if(property=="d"){
				
				// Path data; load into an actual path:
				Shape.Path.Clear();
				
				PathString.Load(getAttribute("d"),Shape.Path);
				
			}else if(property=="pathlength"){
				
			}else if(property=="marker-start"){
				
			}else if(property=="marker-mid"){
				
			}else if(property=="marker-end"){
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
		/// <summary>Directly sets a vector path.</summary>
		public void setPath(VectorPath path){
			Shape.SetPath(path);
		}
		
		/// <summary>DOM API - get the total length of this path.</summary>
		public float getTotalLength(){
			
			return Shape.Path.Length();
			
		}
		
	}
	
}