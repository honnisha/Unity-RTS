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
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use PowerUI.Document.innerHTML to set it's content.
	/// </summary>

	public partial class ReflowDocument{
		
		// Functionality related to finding a CSS selector.
		
		/// <summary>The set of all * rules. It's a set because there may be some being specific, like *[type].</summary>
		public List<Css.StyleRule> AnySelectors=new List<Css.StyleRule>();
		
		/// <summary>A set of attributes used by any selectors. (e.g. input[type=..] is a "type" attrib).</summary>
		public Dictionary<string,int> SelectorAttributes=new Dictionary<string,int>();
		
		/// <summary>A lookup of the selector "stubs". For example, ul li{} has a "stub" of ul.
		/// Always a #id, .class or tag. Usually the most leftward component of a selector.</summary>
		public Dictionary<string,List<Css.StyleRule>> SelectorStubs=new Dictionary<string,List<Css.StyleRule>>();
		
		/// <summary>The style sheets on the document.</summary>
		public List<Css.StyleSheet> styleSheets=new List<Css.StyleSheet>();
		
		/// <summary>The CSS variables within this document.</summary>
		public Dictionary<string,Css.Value> CssVariables=new Dictionary<string,Css.Value>();
		
		
		/// <summary>Adds the given css style to the document. Used by style tags.</summary>
		/// <param name="ele">The scope element to use.</param>
		/// <param name="css">The css to add to the document.</param>
		/// <param name="index">The index in the style buffer to add the css into.</param>
		public void AddStyle(Css.StyleSheet sheet,string css){
			
			// Create if needed:
			if(styleSheets==null){
				styleSheets=new List<Css.StyleSheet>();
			}
			
			// Add it:
			styleSheets.Add(sheet);
			
			// Set the priority (Priority 0 is the UA sheet):
			sheet.Priority=styleSheets.Count;
			
			if(css!=null){
				// Parse the CSS now!
				sheet.ParseCss(css);
			}
			
		}
		
		/// <summary>Gets a stylesheet by its href.</summary>
		public Css.StyleSheet GetStyle(string href){
			
			foreach(Css.StyleSheet s in styleSheets){
				if(s.href==href){
					return s;
				}
			}
			
			return null;
			
		}
		
		/// <summary>Clears all css style definitions from this document.</summary>
		public void ClearStyle(){
			
			// Clear all the lookups:
			AnySelectors.Clear();
			SelectorAttributes.Clear();
			SelectorStubs.Clear();
			
			// Clear the sheets:
			styleSheets.Clear();
			
			// Nothing embedded:
			EmbeddedNamespaces=null;
			
			// Re-add the default sheet (into the 3 lookups):
			Namespace.DefaultStyleSheet.ReAddSheet(this);
			
			// Empty variables:
			CssVariables.Clear();
			
		}
		
		/// <summary>Adds the given css style to the document. Used by style tags.</summary>
		/// <param name="ele">The scope element to use.</param>
		/// <param name="css">The css to add to the document.</param>
		/// <param name="index">The index in the style buffer to add the css into.</param>
		public Css.StyleSheet AddStyle(Node ele,string css){
			
			// Create a style sheet:
			Css.StyleSheet sheet=new Css.StyleSheet(ele);
			
			// Add it:
			AddStyle(sheet,css);
			
			return sheet;
			
		}
		
		/// <summary>Gets a style definition by css selector from the StyleSheet.
		/// If it's not found in the style sheet, the default stylesheet is checked.</summary>
		/// <param name="selector">The css selector to search for.</param>
		/// <returns>If found, a selector style definition; null otherwise.</returns>
		public Css.Style getStyleBySelector(string selectorText){
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selectorText,documentElement);
			
			// Read a selector:
			Css.Selector selector=lexer.ReadSelector();
			
			return getStyleBySelector(selector);
			
		}
		
		/// <summary>Gets a style definition by css selector from the StyleSheet.
		/// If it's not found in the style sheet, the default stylesheet is checked.</summary>
		/// <param name="selector">The css selector to search for.</param>
		/// <returns>If found, a selector style definition; null otherwise.</returns>
		public List<Css.Style> getStylesBySelector(string selectorText){
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selectorText,documentElement);
			
			// Read a selector:
			Css.Selector selector=lexer.ReadSelector();
			
			return getStylesBySelector(selector);
			
		}
		
		/// <summary>Selects the first element from this document that matches the given CSS selector.
		/// Do note that this is not designed to be used in high-performance situations and
		/// you should cache the results as much as possible.</summary>
		public Element querySelector(string selector){
			Dom.NodeList results=querySelectorAll(selector,true);
			
			if(results==null || results.length==0){
				return null;
			}
			
			return results[0] as Element;
		}
		
		/// <summary>Selects elements from this document using the given CSS selector.
		/// Do note that this is not designed to be used in high-performance situations and
		/// you should cache the results as much as possible.</summary>
		public Dom.NodeList querySelectorAll(string selector){
			return querySelectorAll(selector,false);
		}
		
		/// <summary>Selects elements from this document using the given CSS selector.
		/// Do note that this is not designed to be used in high-performance situations and
		/// you should cache the results as much as possible.</summary>
		public Dom.NodeList querySelectorAll(string selector,bool one){
			
			// Create results set:
			Dom.NodeList results=new Dom.NodeList();
			
			if(string.IsNullOrEmpty(selector)){
				// Empty set:
				return results;
			}
			
			Node root=documentElement;
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selector,root);
			
			List<Css.Selector> all=new List<Css.Selector>();
			
			// Read selectors:
			lexer.ReadSelectors(all);
			
			if(all.Count==0){
				// Invalid selector:
				return results;
			}
			
			// Create a blank event to store the targets, if any:
			CssEvent e=new CssEvent();
			
			// Perform the selection process:
			(root as IRenderableNode).querySelectorAll(all.ToArray(),results,e,one);
			
			return results;
		}
		
		/// <summary>Attempts to get a style from this sheet by selector.</summary>
		/// <param name="selector">The selector to lookup.</param>
		/// <returns>A style for the selector if its found; null otherwise.</returns>
		public Css.Style getStyleBySelector(Css.Selector selector){
			
			if(selector==null){
				// None
				return null;
			}
			
			// NOTE: The given selector is *not* an object we can match by reference (via ==).
			// We must compare it for its actual components.
			
			string styleText=selector.LastRoot.StyleText;
			
			List<Css.StyleRule> selectors;
			
			if(styleText=="*"){
				selectors=AnySelectors;
			}else{
				
				// Stubs provide our first (and often sizeable) filtering.
				if(!SelectorStubs.TryGetValue(styleText,out selectors)){
					return null;
				}
				
			}
			
			if(selectors.Count==0){
				return null;
			}
			
			// For each potential matcher..
			foreach(Css.StyleRule compare in selectors){
				
				if(compare.Selector.Equals(selector)){
					return compare.Style;
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Attempts to get one or more styles from this sheet by selector.</summary>
		/// <param name="selector">The selector to lookup.</param>
		/// <returns>A style for the selector if its found; null otherwise.</returns>
		public List<Css.Style> getStylesBySelector(Css.Selector selector){
			
			// NOTE: The given selector is *not* an object we can match by reference (via ==).
			// We must compare it for it's actual components.
			
			string styleText=selector.LastRoot.StyleText;
			
			List<Css.StyleRule> selectors;
			
			if(styleText=="*"){
				selectors=AnySelectors;
			}else{
				
				// Stubs provide our first (and often sizeable) filtering.
				if(!SelectorStubs.TryGetValue(styleText,out selectors)){
					return null;
				}
				
			}
			
			if(selectors.Count==0){
				return null;
			}
			
			List<Css.Style> res=new List<Css.Style>();
			
			// For each potential matcher..
			foreach(Css.StyleRule compare in selectors){
				
				if(compare.Selector.Equals(selector)){
					res.Add(compare.Style);
				}
				
			}
			
			if(res.Count==0){
				return null;
			}
			
			return res;
			
		}
		
	}
	
}