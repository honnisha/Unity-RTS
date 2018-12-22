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
using PowerUI;
using Json;
using System.Reflection;


namespace PowerSlide{
	
	/// <summary>
	/// A raw slide action. E.g. "start cutscene x" or "open door y". These are mapped as event listeners.
	/// Note that the SlideEvent can hold extra parameters to pass through to your target.
	/// </summary>
	
	public class Action{
		
		/// <summary>The slide this is an action for.</summary>
		public Slide Slide;
		/// <summary>A custom action ID.</summary>
		public string ID;
		/// <summary>The method to run. Must accept a SlideEvent.</summary>
		public MethodInfo Method;
		
		
		public Action(Slide slide){
			Slide=slide;
		}
		
		/// <summary>Loads the action meta from the given JSON data.</summary>
		public Action(Slide slide,JSObject data){
			Slide=slide;
			Load(data);
		}
		
		/// <summary>Loads the action meta from the given JSON data.</summary>
		public void Load(JSObject data){
			
			// Method and an ID:
			string methodName=data.String("method");
			ID=data.String("id");
			
			int index=methodName.LastIndexOf('.');
			
			if(index==-1){
				throw new Exception("Class name and method required in slide action methods (\"method\":\"Class.Method\").");
			}
			
			// Grab the class name:
			string className=methodName.Substring(0,index);
			
			// Get the type:
			Type type=JavaScript.CodeReference.GetFirstType(className);
			
			if(type==null){
				throw new Exception("Slide action method type not found: "+className);
			}
			
			// Update the method name:
			methodName=methodName.Substring(index+1);
			
			// Grab the method info:
			
			#if NETFX_CORE
			Method=type.GetTypeInfo().GetDeclaredMethod(methodName);
			#else
			Method=type.GetMethod(methodName);
			#endif
			
			if(Method==null){
				throw new Exception("Slide action method not found in '"+type.ToString()+"': "+methodName);
			}
			
		}
		
	}
	
}