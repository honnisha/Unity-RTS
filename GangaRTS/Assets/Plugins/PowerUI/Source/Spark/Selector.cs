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
using Dom;


namespace Css{
	
	/// <summary>
	/// Used in a stylesheet to direct selection requests.
	/// A selector describes e.g. the parent node etc.
	/// <summary>
	
	public class Selector{
		
		/// <summary>The computed specifity for this selector.</summary>
		public int Specifity;
		/// <summary>Same as Roots.Length. There's always at least one.</summary>
		public int RootCount;
		/// <summary>The rule this selector belongs to.</summary>
		public StyleRule Rule;
		/// <summary>The complete original CSS value.</summary>
		public Css.Value Value;
		/// <summary>The root that is considered the target of this selector. Its IsTarget flag is true.</summary>
		public RootMatcher Target;
		/// <summary>All the roots of this selector. There's always at least one.</summary>
		public RootMatcher[] Roots;
		/// <summary>Quick reference to roots[RootCount-1].</summary>
		public RootMatcher LastRoot;
		/// <summary>Quick reference to roots[0].</summary>
		public RootMatcher FirstRoot;
		/// <summary>The namespace, if any, of this selector.</summary>
		public CssNamespace Namespace;
		/// <summary>A final call which maps the selector into a pseudo element.</summary>
		public PseudoSelectorMatch PseudoElement;
		
		
		/// <summary>True if this selector originated from the UA.</summary>
		public bool IsUserAgent{
			get{
				StyleSheet sheet=Sheet;
				return (sheet!=null && sheet.IsUserAgent); 
			}
		}
		
		/// <summary>The stylesheet this selector belongs to.</summary>
		public StyleSheet Sheet{
			get{
				if(Rule==null){
					return null;
				}
				
				return Rule.ParentStyleSheet;
			}
		}
		
		/// <summary>Do these selector entities match?</summary>
		public bool Equals(Selector selector){
			
			// Reference match:
			if(selector==this){
				return true;
			}
			
			// Same # of roots?
			if(selector==null || RootCount!=selector.RootCount){
				return false;
			}
			
			// Try matching each root:
			for(int i=0;i<RootCount;i++){
				
				if(! Roots[i].Equals(selector.Roots[i]) ){
					return false;
				}
				
			}
			
			// Ok!
			return true;
			
		}
		
		/// <summary>Gets the specifity of this selector.</summary>
		public void GetSpecifity(int sheetPriority){
			
			Specifity=0;
			
			for(int i=0;i<Roots.Length;i++){
				
				// Bump up the specifity:
				Specifity+=Roots[i].Specifity;
				
			}
			
			// Account for sheet priority/ user agent (3rd byte):
			Specifity|=sheetPriority<<24;
			
		}
		
		/// <summary>Applies a structurally matched selector to the DOM.
		/// Occurs shortly after StructureMatch.</summary>
		public MatchingSelector BakeToTarget(ComputedStyle cs, CssEvent e){
			
			// Get the node:
			Node node=cs.Element;
			
			// First, generate our instance:
			MatchingSelector ms=new MatchingSelector();
			
			// Update it:
			ms.Selector=this;
			ms.MatchedRoots=new MatchingRoot[RootCount];
			
			// For each root, create a MatchingRoot object.
			
			// Apply target - this helps track which element we're actually testing:
			e.CurrentNode=node;
			e.SelectorTarget=null;
			
			// We always start from the tail and work backwards.
			// If we get a match, then the caller can do whatever it wants to the target.
			
			for(int i=RootCount-1;i>=0;i--){
				
				// Get the matcher:
				RootMatcher rm=Roots[i];
				
				// Try matching this root:
				if(!rm.TryMatch(e.CurrentNode)){
					
					// Failed! If we had a matcher and it has Repeat set true, try again:
					if(rm.NextMatcher!=null && rm.NextMatcher.Repeat){
						
						// Move target:
						rm.NextMatcher.MoveUpwards(e);
						
						// Try matching again:
						i++;
						continue;
						
					}
					
				}else{
					
					// Match! e.CurrentNode is the node to add.
					
					// Create the instance:
					MatchingRoot matchedRoot=new MatchingRoot();
					matchedRoot.Root=rm;
					matchedRoot.Selector=ms;
					matchedRoot.Node=e.CurrentNode;
					
					// Get renderable node:
					IRenderableNode renderable=(e.CurrentNode as IRenderableNode);
					
					// Add to selector:
					ms.MatchedRoots[i]=matchedRoot;
					
					// Add:
					ComputedStyle nodeCs=renderable.ComputedStyle;
					
					// Push the match now into the linked list:
					if(nodeCs.FirstMatch==null){
						nodeCs.FirstMatch=matchedRoot;
						nodeCs.LastMatch=matchedRoot;
					}else{
						matchedRoot.PreviousInStyle=nodeCs.LastMatch;
						nodeCs.LastMatch.NextInStyle=matchedRoot;
						nodeCs.LastMatch=matchedRoot;
					}
					
					if(rm.IsTarget){
						// Update the target now:
						e.SelectorTarget=renderable.RenderData;
					}
					
				}
				
				// If we have a structure matcher, run it now. It'll move CurrentNode for us:
				if(rm.PreviousMatcher!=null){
					
					// Move target:
					rm.PreviousMatcher.MoveUpwards(e);
					
				}
				
			}
			
			// Final pass - if we have a pseudo-element, apply it now:
			if(PseudoElement!=null){
				PseudoElement.Select(e);
			}
			
			// Apply target:
			ms.Target=e.SelectorTarget;
			
			// Finally, refresh all:
			ms.ResetActive();
			
			return ms;
			
		}
		
		/// <summary>True if this selector matches the structure of the DOM where the given CS is.</summary>
		public bool StructureMatch(ComputedStyle cs, CssEvent e){
			
			// Get the node:
			Node node=cs.Element;
			
			if(node==null){
				return false;
			}
			
			// Apply target - this helps track which element we're actually testing:
			e.CurrentNode=node;
			
			// We always start from the tail and work backwards.
			// If we get a match, then the caller can do whatever it wants to the target.
			for(int i=RootCount-1;i>=0;i--){
				
				// Get the matcher:
				RootMatcher rm=Roots[i];
				
				// Try matching this root:
				if(!rm.TryMatch(e.CurrentNode)){
					
					// Failed! If we had a matcher and it has Repeat set true, try again:
					if(rm.NextMatcher!=null && rm.NextMatcher.Repeat){
						
						// Move target:
						rm.NextMatcher.MoveUpwards(e);
						
						// Still got a node?
						if(e.CurrentNode==null){
							return false;
						}
						
						// Try matching again:
						i++;
						continue;
						
					}
					
					return false;
					
				}
				
				// If we have a structure matcher, run it now. It'll move CurrentNode for us:
				if(rm.PreviousMatcher!=null){
					
					// Move target:
					rm.PreviousMatcher.MoveUpwards(e);
					
					// Still got a node?
					if(e.CurrentNode==null){
						return false;
					}
					
				}
				
			}
			
			// If we have a pseudo element, make sure parents haven't also matched this selector.
			if(PseudoElement!=null){
				
				// Have any parents matched this selector?
				Node parent=node.parentNode;
				
				while(parent!=null){
					
					if(parent.getAttribute("spark-virt")!=null){
						// Already on a virtual element - quit there.
						return false;
					}
					
					parent=parent.parentNode;
				}
				
			}
			
			// All clear!
			return true;
			
		}
		
		/// <summary>call this to try to match this selector.
		/// If it matches, it will return the element which style should be applied to.</summary>
		public Node Test(ComputedStyle style,CssEvent e){
			
			if(StructureMatch(style,e)){
				return e.CurrentNode;
			}
			
			return null;
			
		}
		
		/// <summary>Removes this selector from its parent documents caches.</summary>
		public void RemoveFromDocument(StyleRule rule,ReflowDocument document,Css.StyleSheet sheet){
			
			// Get the cached root name:
			string text=LastRoot.StyleText;
			
			List<StyleRule> list=null;
			
			if(text=="*"){
				
				list=document.AnySelectors;
				
			}else{
				
				// Add the rule:
				document.SelectorStubs.TryGetValue(text,out list);
				
			}
			
			// Remove from the set:
			if(list!=null){
				
				for(int i=0;i<list.Count;i++){
					
					if(list[i]==rule){
						list.RemoveAt(i);
						break;
					}
					
				}
				
			}
			
			// For each root, remove their locals:
			for(int r=0;r<RootCount;r++){
				
				// Get locals set:
				LocalMatcher[] locals=Roots[r].LocalMatchers;
				
				if(locals!=null){
					
					// For each one..
					for(int i=0;i<locals.Length;i++){
						
						// Remove it:
						locals[i].RemoveFromDocument(document,sheet);
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Adds this selector to its parent documents caches.
		/// This is used by the default style sheet when the document gets cleared.</summary>
		public void AddToDocument(StyleRule rule,ReflowDocument document,Css.StyleSheet sheet){
			
			// Get the cached root (always the 'last' one):
			string text=LastRoot.StyleText;
			
			List<StyleRule> list=null;
			
			if(text=="*"){
				
				list=document.AnySelectors;
				
			}else{
				
				// Add the rule:
				if(!document.SelectorStubs.TryGetValue(text,out list)){
					
					list=new List<StyleRule>();
					document.SelectorStubs[text]=list;
					
				}
				
			}
			
			// Add the rule to the set:
			list.Add(rule);
			
			// For each root, add their locals:
			for(int r=0;r<RootCount;r++){
				
				// Get locals set:
				LocalMatcher[] locals=Roots[r].LocalMatchers;
				
				if(locals!=null){
					
					// For each one..
					for(int i=0;i<locals.Length;i++){
						
						// Add it:
						locals[i].AddToDocument(document,sheet);
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>The selector part of the rule.</summary>
		public string selectorText{
			get{
				string result="";
				
				for(int i=0;i<Roots.Length;i++){
					
					RootMatcher root=Roots[i];
					
					// The simple selectors:
					result+=root.ToString();
					
					if(root.NextMatcher!=null){
						
						// Combinator:
						result+=root.NextMatcher.ToString();
						
					}
					
				}
				
				return result;
			}
		}
		
		/// <summary>Gets the selector name.</summary>
		public override string ToString(){
			return selectorText;
		}
		
	}
	
}