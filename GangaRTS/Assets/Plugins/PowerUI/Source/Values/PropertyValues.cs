//--------------------------------------
//           Property values 
// standard set of referenceable values
//   Used mainly by Blade and Loonim.
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BinaryIO;


namespace Values{
	
	/// <summary>
	/// Manager for selecting anything such as a particular material or point from a blade object.
	/// </summary>
	
	public static class PropertyValues{
		
		/// <summary>All property value handlers.</summary>
		public static Dictionary<int,PropertyValue> All;
		
		
		/// <summary>
		/// Adds a type as a form of property value.
		/// </summary>
		public static void Add(Type pvType){
			
			if(All==null){
				All=new Dictionary<int,PropertyValue>();
			}
			
			// Instance:
			PropertyValue value=(PropertyValue)Activator.CreateInstance(pvType);
			All[value.GetID()]=value;
			
		}
		
		/// <summary>Loads a property value from the given reader.</summary>
		public static PropertyValue ReadPropertyValue(Reader reader){
			
			// What value type is it?
			int type=(int)reader.ReadCompressed();
			
			// Create it:
			PropertyValue value=PropertyValues.Get(type);
			
			if(value==null){
				return null;
			}
			
			// Read it:
			value.Read(reader);
			
			return value;
		}
		
		public static PropertyValue Get(int id){
			
			if(All==null){
				
				// Load them now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(PropertyValue),
					delegate(Type t){
						// Add it as a PV:
						Add(t);
					}
				);
				
			}
			
			PropertyValue result;
			
			if(All.TryGetValue(id,out result)){
				
				return result.Create();
				
			}
			
			return null;
			
		}
		
	}
	
}