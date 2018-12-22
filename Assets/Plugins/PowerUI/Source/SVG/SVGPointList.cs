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
using System.Collections.Generic;
using Dom;


namespace Svg{
	
	/// <summary>
	/// A list of points.
	/// </summary>
	
	public class SVGPointList:SVGListInterface<DOMPoint>{
		
		public SVGPointList(bool readOnly,Css.Value init,Node host,string attr):base(readOnly,init,host,attr){}
		
		/// <summary>Creates an instance of the listed type from the given information in a CSS value.</summary>
		protected override DOMPoint Create(Css.Value from){
			return new DOMPoint(IsReadOnly,from[0].GetInteger(null,null),from[1].GetInteger(null,null));
		}
		
	}
	
}