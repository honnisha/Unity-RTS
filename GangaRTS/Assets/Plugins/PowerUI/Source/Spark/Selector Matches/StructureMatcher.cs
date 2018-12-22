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
	/// DOM structure matchers are things like a ~ b. They test if an element
	/// structurally matches the selector.
	/// </summary>
	
	public class StructureMatcher : SelectorMatcher{
		
		/// <summary>Moves e.CurrentNode through the DOM 
		/// ('backwards', e.g. from something to its parent).</summary>
		public virtual void MoveUpwards(CssEvent e){
			
		}
		
		/// <summary>True if this matcher should be repeatedly 
		/// moved until it potentially gets a hit.</summary>
		public virtual bool Repeat{
			get{
				return false;
			}
		}
		
	}
	
}