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


namespace MathML{
	
	/// <summary>
	/// The Math namespace attribute as used by all MathML nodes.
	/// </summary>
	public class MathNamespace : XmlNamespace{
		
		public MathNamespace()
			:base("http://www.w3.org/1998/Math/MathML","mml","application/mathml+xml",typeof(MathDocument))
		{
			
		}
		
	}
	
}