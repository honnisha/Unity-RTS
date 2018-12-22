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
using InfiniText;


namespace Css{
	
	/// <summary>
	/// Extends the CSS Value object with a function for obtaining OpenType features.
	/// </summary>
	
	public partial class Value{
		
		public virtual void GetOpenTypeFeatures(TextRenderingProperty trp,List<OpenTypeFeature> features){
			
		}
		
	}
	
}