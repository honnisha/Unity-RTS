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
using Css;
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	/// <summary>
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use PowerUI.Document.innerHTML to set it's content.
	/// </summary>

	public partial class ReflowDocument{
		
		/// <summary>A fast lookup of the media rules on this document.</summary>
		public Dictionary<string,Css.AtRules.FontFeatureValuesRule> FontFeatures;
		
	}
	
}