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
	/// A global lookup of units to handler. E.g. ..px is a unit.
	/// A global one is instanced and copied when it's required.
	/// </summary>
	
	public static class CssUnits{
		
		/// <summary>A mapping which maps first character to a set of CSS units. Applies to the end of a value, e.g. % or px.</summary>
		public static Dictionary<char,CssUnitHandlers> AllEnd;
		/// <summary>A mapping which maps first character to a set of CSS units. Applies to the start of a value, e.g. " or #.</summary>
		public static Dictionary<char,CssUnitHandlers> AllStart;
		
		
		/// <summary>
		/// Adds a unit by its type.
		/// </summary>
		/// <param name='unitType'>The type of the unit to add.</param>
		public static void Add(Type unitType){
			
			if(AllEnd==null){
				// Create the sets:
				AllEnd=new Dictionary<char,CssUnitHandlers>();
				AllStart=new Dictionary<char,CssUnitHandlers>();
			}
			
			// Instance it:
			CssUnit unit=(CssUnit)Activator.CreateInstance(unitType);
			
			string[] pre=unit.PreText;
			string[] post=unit.PostText;
			
			if(pre!=null){
				AddToSet(pre,AllStart,unit);
			}
			
			if(post!=null){
				AddToSet(post,AllEnd,unit);
			}
			
		}
		
		/// <summary>Adds a CSS at rule to the global set.</summary>
		/// <param name="cssUnit">The at rule to add.</param>
		/// <returns>True if adding it was successful.</returns>
		private static void AddToSet(string[] names,Dictionary<char,CssUnitHandlers> set,CssUnit cssUnit){
			
			for(int i=0;i<names.Length;i++){
				
				string text=names[i];
				
				char first=text[0];
				
				CssUnitHandlers handlers;
				if(!set.TryGetValue(first,out handlers)){
					
					// Create it:
					handlers=new CssUnitHandlers();
					handlers.Character=first;
					set[first]=handlers;
					
				}
				
				handlers.Add(text,cssUnit);
				
			}
			
		}
		
	}
	
}