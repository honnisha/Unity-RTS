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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Css.Units;


namespace Css{
	
	/// <summary>
	/// An @media rule.
	/// </summary>
	
	public class MediaRule : Rule{
		
		/// <summary>The raw CSS value.</summary>
		public Css.Value RawValue;
		/// <summary>The previous status of the media rule.</summary>
		private bool PreviousStatus;
		/// <summary>The rules which activate when this media rule does.</summary>
		public List<Rule> Rules;
		/// <summary>The underlying media query.</summary>
		public MediaQuery Query;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		public MediaRule(StyleSheet sheet,Css.Value rawValue,MediaQuery query,SelectorBlockUnit contents){
			
			ParentSheet=sheet;
			Query=query;
			RawValue=rawValue;
			
			// Load rules from contents:
			// - No document here; we don't want them to get added to the lookup until we evaluate the rule.
			Rules=contents.LoadAsRules();
			
		}
		
		/// <summary>The CSS text of this rule.</summary>
		public string cssText{
			get{
				return RawValue.ToString();
			}
			set{
				throw new NotImplementedException("cssText is read-only on rules. Set it for a whole sheet instead.");
			}
		}
		
		/// <summary>The rules.</summary>
		public List<Rule> cssRules{
			get{
				return Rules;
			}
		}
		
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet parentStyleSheet{
			get{
				return ParentSheet;
			}
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 4;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
			// Require doc.Media now:
			document.RequireMediaType();
			
			if(document.MediaRules==null){
				
				// Create it:
				document.MediaRules=new List<MediaRule>();
				
			}
			
			// Add to rules:
			document.MediaRules.Add(this);
			
			// Attempt to activate it:
			if(!PreviousStatus){
				
				Evaluate();
				
			}
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
			// Contained in media rules?
			if(document.MediaRules==null){
				return;
			}
			
			for(int i=0;i<document.MediaRules.Count;i++){
				
				if(document.MediaRules[i]==this){
					
					// Got it! Remove now:
					document.MediaRules.RemoveAt(i);
					return;
					
				}
				
			}
			
			// Always deactivate:
			if(PreviousStatus){
				
				PreviousStatus=false;
				Deactivate(ParentSheet.document);
				
			}
			
		}
		
		/// <summary>Called when this rule is now active.</summary>
		private void Activate(ReflowDocument document){
			
			// Add selectors to documents lookups:
			foreach(Rule rule in Rules){
				
				// Note that this also auto triggers the RuleAdded method, which is what we want for it to apply!
				rule.AddToDocument(document);
				
			}
			
		}
		
		/// <summary>Called when this rule is no longer active.</summary>
		private void Deactivate(ReflowDocument document){
			
			// Remove it:
			foreach(Rule rule in Rules){
				
				// Remove the rule:
				rule.RemoveFromDocument(document);
				
			}
			
		}
		
		public void Evaluate(){
			
			ReflowDocument document=ParentSheet.document;
			
			// Test the query now:
			bool testQuery=Query.IsTrue(document);
			
			if(testQuery==PreviousStatus){
				
				// Unchanged.
				return;
				
			}
		
			PreviousStatus=testQuery;
			
			if(testQuery){
				
				// This query is now active.
				Activate(document);
				
			}else{
				
				// This query is now no longer active.
				Deactivate(document);
				
			}
			
		}
		
	}
	
}