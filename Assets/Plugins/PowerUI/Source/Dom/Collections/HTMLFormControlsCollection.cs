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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;


namespace Dom{

	/// <summary>
	/// A collection of HTML form control elements.
	/// Very similar to HTMLCollection, only its namedItem method is slightly different.
	/// </summary>
	public partial class HTMLFormControlsCollection : HTMLCollection{
		
		public override Element namedItem(string name){
			// Get the named item:
			Element ne=base.namedItem(name);
			
			// If it's type='radio' then build and return a RadioNodeList.
			if(ne.getAttribute("type")=="radio"){
				
				// Create it, populating it at the same time:
				return new RadioNodeList(this,name);
				
			}
			
			return ne;
		}
		
	}
	
}