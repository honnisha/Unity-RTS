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
	/// A global lookup of function name to function. E.g. rgba() is a CSS function.
	/// Css functions are instanced globally and mapped to the names they use.
	/// Note that functions are not instanced per element.
	/// </summary>
	
	public static class CssFunctions{
		
		/// <summary>The lookup itself. Matches name (e.g. "rgba") to the function that will process it.</summary>
		public static Dictionary<string,CssFunction> All;
		
		
		/// <summary>Adds a CSS function to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="functionType">The type of the function to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Type functionType){
			
			if(All==null){
				// Create the set:
				All=new Dictionary<string,CssFunction>();
			}
			
			// Instance it:
			CssFunction cssFunction=(CssFunction)Activator.CreateInstance(functionType);
			
			string[] names=cssFunction.GetNames();
			
			if(names==null||names.Length==0){
				return false;
			}
			
			for(int i=0;i<names.Length;i++){
				
				// Grab the name:
				string name=names[i].ToLower();
				
				// Add it to functions:
				All[name]=cssFunction;
				
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the named function, returning the global function if it's found.</summary>
		/// <param name="name">The function to look for.</param>
		/// <returns>The global CssFunction if the function was found; Null otherwise.</returns>
		public static CssFunction Get(string name){
			CssFunction globalFunction=null;
			All.TryGetValue(name.ToLower(),out globalFunction);
			return globalFunction;
		}

	}
	
}