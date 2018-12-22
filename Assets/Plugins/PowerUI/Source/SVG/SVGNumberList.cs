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
	/// A list of SVG numbers.
	/// </summary>
	
	public class SVGNumberList:SVGListInterface<SVGNumber>{
		
		public SVGNumberList(bool readOnly,Css.Value init,Node host,string attr):base(readOnly,init,host,attr){}
		
		/// <summary>Creates an instance of the listed type from the given information in a CSS value.</summary>
		protected override SVGNumber Create(Css.Value from){
			return new SVGNumber(IsReadOnly,from.GetInteger(null,null));
		}
		
	}
	
}