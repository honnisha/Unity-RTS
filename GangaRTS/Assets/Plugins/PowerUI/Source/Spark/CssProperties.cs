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
	/// A global lookup of property name to property.
	/// CssHandlers are instanced globally and mapped to the property names they accept.
	/// Note that properties are not instanced per element.
	/// </summary>
	
	public static class CssProperties{
		
		/// <summary>The lookup itself. Matches property name (e.g. "color") to the property that will process it.</summary>
		public static Dictionary<string,CssProperty> All;
		/// <summary>All properties which apply directly to text. E.g. color,font-size etc.</summary>
		public static Dictionary<string,CssProperty> AllText;
		
		
		/// <summary>Gets the default CSS property.</summary>
		public static CssProperty Default(){
			return new CssProperty();
		}
		
		/// <summary>Adds a CSS property to the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="propertyType">The type of the property to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Type propertyType){
			
			if(All==null){
				
				// Create sets:
				All=new Dictionary<string,CssProperty>();
				AllText=new Dictionary<string,CssProperty>();
			
			}
			
			// Create an instance now:
			CssProperty property=(CssProperty)Activator.CreateInstance(propertyType);
			
			string[] tags=property.GetProperties();
			
			if(tags==null||tags.Length==0){
				return false;
			}
			
			for(int i=0;i<tags.Length;i++){
				// Grab the property:
				string propertyName=tags[i].ToLower();
				
				// Is it the first? If so, set the name:
				if(i==0){
					property.Name=propertyName;
				}
				
				// Add it to properties:
				All[propertyName]=property;
				
				if(property.IsTextual){
					// Add it to text properties too:
					AllText[propertyName]=property;
				}
				
				// Make sure initial value has low specif:
				property.InitialValue.Specifity=-1;
				
				// Add aliases:
				property.Aliases();
				
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the property with the given name.
		/// If it's not found, a default property which is known to exist can be returned instead.
		/// For example, property "color".</summary>
		/// <param name="property">The property to look for.</param>
		/// <param name="defaultProperty">If the given property is not found, this is used instead.</param>
		/// <returns>The global property object.</returns>
		public static CssProperty Get(string property,string defaultProperty){
			CssProperty globalProperty=Get(property);
			
			if(globalProperty==null){
				globalProperty=Get(defaultProperty);
			}
			
			return globalProperty;
		}
		
		/// <summary>Attempts to find the named property, returning the global property if it's found.</summary>
		/// <param name="property">The property to look for.</param>
		/// <returns>The global CssProperty if the property was found; Null otherwise.</returns>
		public static CssProperty Get(string property){
			CssProperty globalProperty=null;
			All.TryGetValue(property,out globalProperty);
			return globalProperty;
		}

	}
	
}