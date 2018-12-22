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
using Dom;
using Blaze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Css;


namespace Svg{
	
	/// <summary>
	/// The SVG namespace attribute as used by all SVG nodes.
	/// </summary>
	public class SVGNamespace : XmlNamespace{
		
		public SVGNamespace()
			:base("http://www.w3.org/2000/svg","svg","image/svg+xml",typeof(SVGDocument))
		{
			
		}
		
	}
	
}