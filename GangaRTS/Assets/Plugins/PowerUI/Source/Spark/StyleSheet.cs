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
	/// Holds a set of selector styles. A selector is something like .name or #name.
	/// </summary>
	
	public partial class StyleSheet{
		
		/// <summary>True if this sheet is disabled.</summary>
		private bool IsDisabled;
		/// <summary>The full location of this sheet.</summary>
		public Dom.Location Location;
		/// <summary>The default namespace being used if any.</summary>
		public CssNamespace Namespace;
		/// <summary>The priority (load order) of this stylesheet.</summary>
		internal int Priority;
		/// <summary>True if this is a UA sheet.</summary>
		public bool IsUserAgent{
			get{
				return Priority==0;
			}
		}
		/// <summary>Css namespace prefixes, if any.</summary>
		public Dictionary<string,CssNamespace> Namespaces;
		
		
		/// <summary>Creates an empty stylesheet that belongs to the given document.</summary>
		/// <param name="document">The html document that this sheet belongs to.</param>
		public StyleSheet(Node owner){
			ownerNode=owner;
		}
		
		/// <summary>The rules on this style sheet. Note that CSS lookups don't use this set.
		/// Instead, all CSS lookups route through a single set on the document itself.</summary>
		public List<Css.Rule> cssRules=new List<Css.Rule>();
		
		/// <summary>The document this sheet belongs to, if any.</summary>
		public ReflowDocument document{
			get{
				
				if(ownerNode==null){
					return null;
				}
				
				return ownerNode.document as ReflowDocument;
			}
		}
		
		/// <summary>Is this stylesheet disabled?</summary>
		public bool disabled{
			get{
				return IsDisabled;
			}
			set{
				
				if(IsDisabled==value){
					return;
				}
				
				IsDisabled=value;
				throw new NotImplementedException("Disabled is currently read-only.");
			}
		}
		
		/// <summary>This sheets title.</summary>
		public string title{
			get{
				return ownerNode.getAttribute("title");
			}
		}
		
		/// <summary>The rules on this style sheet.</summary>
		public List<Css.Rule> rules{
			get{
				return cssRules;
			}
		}
		
		/// <summary>The owning element of this style sheet.</summary>
		public Node ownerNode;
		
		/// <summary>The owning element of this style sheet.</summary>
		public Node owningElement{
			get{
				return ownerNode as Element;
			}
		}
		
		/// <summary>The URL of this stylesheet (if it's external).</summary>
		public string href{
			get{
				if(ownerNode==null){
					return null;
				}
				
				return ownerNode.getAttribute("href");
			}
		}
		
		/// <summary>The media.</summary>
		public string media{
			get{
				
				if(ownerNode==null){
					return null;
				}
				
				string media=ownerNode.getAttribute("media");
				
				if(media==null){
					return "screen";
				}
				
				return media;
			}
		}
		
		/// <summary>This style sheets type.</summary>
		public string type{
			get{
				if(ownerNode==null){
					return "text/css";
				}
				
				return ownerNode.getAttribute("type");
			}
		}
		
		/// <summary>Adds a new rule to the set.</summary>
		public void insertRule(string css,int index){
			ParseCss(css);
		}
		
		/// <summary>Removes this stylesheet from the given documents fast lookups.</summary>
		internal void RemoveSheet(ReflowDocument document){
			
			// For each rule..
			foreach(Rule rule in cssRules){
				
				// Add it:
				rule.RemoveFromDocument(document);
				
			}
			
		}
		
		/// <summary>Adds this stylesheet to the documents fast lookups.
		/// This is used by the default stylesheet.</summary>
		internal void ReAddSheet(ReflowDocument document){
			
			// For each rule..
			foreach(Rule rule in cssRules){
				
				// Add it:
				rule.AddToDocument(document);
				
			}
			
		}
		
		public void Add(Rule rule,Rule[] set,bool immediate){
			
			if(rule==null){
				
				if(set!=null){
					
					for(int i=0;i<set.Length;i++){
						
						// Add it:
						cssRules.Add(set[i]);
						
						if(immediate && ownerNode!=null){
							// Add immediately:
							set[i].AddToDocument(ownerNode.document as ReflowDocument);
						}
						
					}
					
				}
				
			}else{
				
				// Add it:
				cssRules.Add(rule);
				
				if(immediate && ownerNode!=null){
					// Add immediately:
					rule.AddToDocument(ownerNode.document as ReflowDocument);
				}
				
			}
			
		}
		
		/// <summary>Keeps reading selectors and their properties until a &gt; or the end of the css is reached.</summary>
		/// <param name="css">The css text to parse.</param>
		public void ParseCss(string css){
			
			CssLexer lexer=new CssLexer(css,ownerNode);
			lexer.Sheet=this;
			
			while(lexer.Peek()!='\0'){
				
				// Read a selector block:
				Rule[] set;
				Rule rule=lexer.ReadRules(out set);
				
				// Add:
				Add(rule,set,true);
				
				// Skip any junk.
				// This is done down here too to avoid creating a blank selector.
				lexer.SkipJunk();
				
			}
			
		}
		
	}
	
}