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


namespace Css{
	
	/// <summary>
	/// A CSS namespace. Used by the at namespace CSS rule.
	/// </summary>
	
	public class CssNamespace{
		
		/// <summary>The main name of this function. Originates from the first result returned by GetNames.</summary>
		public string Name;
		
		
		public CssNamespace(string name){
			Name=name;
		}
		
	}
	
}