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
	/// A global lookup of keyword name to handler. E.g. auto or inherit.
	/// Css keywords are instanced globally and mapped to the names they use.
	/// </summary>
	
	public static class CssKeywords{
		
		/// <summary>The lookup itself. Matches name (e.g. "auto") to the function that will process it.</summary>
		public static Dictionary<string,CssKeyword> All;
		
		
		/// <summary>Adds a CSS keyword to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="cssKeyword">The keyword to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Type keywordType){
			
			if(All==null){
				// Create the set:
				All=new Dictionary<string,CssKeyword>();
			}
			
			// Instance it:
			CssKeyword cssKeyword=(CssKeyword)Activator.CreateInstance(keywordType);
			
			string name=cssKeyword.Name;
			
			if(string.IsNullOrEmpty(name)){
				return false;
			}
			
			// Lowercase it:
			name=name.ToLower();
			
			// Add it to keywords:
			All[name]=cssKeyword;
			
			return true;
		}
		
		/// <summary>Attempts to find the named keyword, returning the global instance if it's found.</summary>
		/// <param name="name">The rule to look for.</param>
		/// <returns>The global CssKeyword if the rule was found; Null otherwise.</returns>
		public static CssKeyword Get(string name){
			CssKeyword globalFunction=null;
			All.TryGetValue(name,out globalFunction);
			return globalFunction;
		}

	}
	
}