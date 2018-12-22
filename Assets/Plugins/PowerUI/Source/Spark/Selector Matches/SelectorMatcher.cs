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
	/// The base class for all CSS selector chunks.
	/// Anything like :hover or #id or a ~ b are all different kinds of matchers.
	/// RootMatcher objects are generally the most important ones.
	/// </summary>
	
	public class SelectorMatcher{
		
		/// <summary>The host selector.</summary>
		public Selector Selector;
		
		/// <summary>Checks if the given node matches this root/local.</summary>
		public virtual bool TryMatch(Dom.Node context){
			
			return false;
			
		}
		
	}
	
}