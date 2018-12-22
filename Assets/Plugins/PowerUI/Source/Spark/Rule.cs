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
	/// The base of all CSS rules.
	/// </summary>
	
	public interface Rule{
		
		/// <summary>The type of rule.</summary>
		int type{ get; }
		
		/// <summary>The parent style sheet.</summary>
		StyleSheet parentStyleSheet{ get; }
		
		/// <summary>The CSS text.</summary>
		string cssText{ get; set; }
		
		/// <summary>The parent rule of this rule.</summary>
		// Rule parentRule{ get; }
		
		/// <summary>Adds this rule to the lookups in the given document.</summary>
		void AddToDocument(ReflowDocument document);
		
		/// <summary>Removes this rule from the lookups in the given document.</summary>
		void RemoveFromDocument(ReflowDocument document);
		
	}

}