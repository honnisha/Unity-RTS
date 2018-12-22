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
using System.Collections;
using Dom;


namespace Svg{
	
	/// <summary>
	/// An animated SVG length.
	/// </summary>
	
	public class SVGAnimatedLength : SVGSerializable{
		
		private SVGLength baseValue_;
		
		
		public SVGAnimatedLength(Css.Value rawValue,Node node,string attr){
			HostNode=node;
			AttributeName=attr;
			baseValue_=new SVGLength(true,this,rawValue);
		}
		
		/// <summary>The reflected base value.</summary>
		public SVGLength baseVal{
			get{
				return baseValue_;
			}
		}
		
		/// <summary>The reflected animatable value.</summary>
		public SVGLength animVal{
			get{
				return baseValue_;
			}
		}
		
		public override void Reserialize(){
			
			if(HostNode==null || baseValue_==null){
				return;
			}
			
			// Just flush out the length:
			HostNode.setAttribute(AttributeName, baseValue_.valueAsString);
			
		}
		
	}
	
}