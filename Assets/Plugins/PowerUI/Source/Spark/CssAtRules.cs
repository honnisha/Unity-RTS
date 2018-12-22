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
using System.Reflection;
using System.Collections.Generic;
using Dom;


namespace Css{
	
	/// <summary>
	/// A global lookup of at rule name to handler. E.g. @font-face is a CSS at rule.
	/// Css at rules are instanced globally and mapped to the names they use.
	/// Note that functions are not instanced per element.
	/// </summary>
	
	public static class CssAtRules{
		
		/// <summary>The lookup itself. Matches name (e.g. "font-face") to the function that will process it.</summary>
		public static Dictionary<string,CssAtRule> All;
		
		
		/// <summary>Adds a CSS at rule to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="cssRule">The at rule to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Type ruleType){
			
			if(All==null){
				// Create the set:
				All=new Dictionary<string,CssAtRule>();
			}
			
			// Instance it:
			CssAtRule cssRule=(CssAtRule)Activator.CreateInstance(ruleType);
			
			string[] names=cssRule.GetNames();
			
			if(names==null||names.Length==0){
				return false;
			}
			
			for(int i=0;i<names.Length;i++){
				
				// Grab the name:
				string name=names[i];
				
				if(name==null){
					continue;
				}
				
				// Add it to functions:
				All[name.ToLower()]=cssRule;
				
			}
			
			return true;
		}
		
		/// <summary>Creates a local copy of a CSS at rule by the given name. Case insensitive. Null if not found.</summary>
		public static CssAtRule GetLocal(string name){
			CssAtRule localFunction;
			
			if(All.TryGetValue(name.ToLower(),out localFunction)){
				// make a copy:
				return localFunction.Copy();
			}
			
			return null;
		}
		
		/// <summary>Attempts to find the named at rule, returning the global instance if it's found.</summary>
		/// <param name="name">The rule to look for.</param>
		/// <returns>The global CssAtRule if the rule was found; Null otherwise.</returns>
		public static CssAtRule Get(string name){
			CssAtRule globalFunction=null;
			All.TryGetValue(name,out globalFunction);
			return globalFunction;
		}

	}
	
}