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


namespace Css{
	
	/// <summary>
	/// Describes if an element is scope
	/// <summary>

	sealed class IsScope:CssKeyword{
		
		public override string Name{
			get{
				return "scope";
			}
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			return new ScopeMatcher();
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for :scope.
	/// </summary>
	
	sealed class ScopeMatcher:LocalMatcher{
		
		/// <summary>The scope of this selector.</summary>
		public Node Scope;
		
		public override void AddToDocument(ReflowDocument document,StyleSheet sheet){
			
			// Apply scope:
			Scope=sheet.ownerNode;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			return (node.parentNode_==Scope);
		}
		
	}
	
}