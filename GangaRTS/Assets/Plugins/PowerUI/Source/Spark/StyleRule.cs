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


namespace Css{
	
	/// <summary>
	/// Used in a stylesheet to direct selection requests.
	/// A selector describes e.g. the parent node etc.
	/// <summary>

	public class StyleRule : Rule{
		
		/// <summary>The style block to apply if this selector matches in full.</summary>
		public Style Style;
		/// <summary>The selector that this is a style for.</summary>
		public Selector Selector;
		/// <summary>This styles parent style sheet (if there is one).</summary>
		public Css.StyleSheet ParentStyleSheet;
		
		
		public StyleRule(Style style){
			Style=style;
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 1;
			}
		}
		
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet parentStyleSheet{
			get{
				return ParentStyleSheet;
			}
		}
		
		public string cssText{
			get{
				
				// Get the selector text:
				return ToString();
				
			}
			set{
				throw new NotImplementedException("cssText is read-only on style rules. Set a whole sheet instead.");
			}
		}
		
		/// <summary>Adds this rule to its parent document caches.
		/// This is used by the default style sheet when the document gets cleared.</summary>
		public void AddToDocument(ReflowDocument document){
			
			// Add the selector (as it may need to update attribute caches):
			Selector.AddToDocument(this,document,ParentStyleSheet);
			
			if(document.documentElement!=null){
				// Is this selector already in use?
				
				// Recurse through the DOM looking for it:
				(document.documentElement as IRenderableNode).RenderData.RuleAdded(this);
			}
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
			// Remove the selector (as it may need to update attribute caches):
			Selector.RemoveFromDocument(this,document,ParentStyleSheet);
			
			if(document.documentElement!=null){
				
				// Find all elements that matched this rule.
				(document.documentElement as IRenderableNode).RenderData.RuleRemoved(this);
				
			}
			
		}
		
		/// <summary>The rules style.</summary>
		public Css.Style style{
			get{
				return Style;
			}
		}
		
		/// <summary>Gets the selector name.</summary>
		public override string ToString(){
			
			string result=Selector.ToString();
			
			return result+"{ "+Style.ToString()+" }";
			
		}
		
	}
	
}