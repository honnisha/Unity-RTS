//--------------------------------------
//         Nitro Script Engine
//          Wrench Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace JavaScript{
	
	/// <summary>
	/// Used for obtaining the current assembly.
	/// </summary>
	
	public static class Assemblies{
		
		/// <summary>The current executing assembly.</summary>
		public static Assembly Current{
			get{
				#if NETFX_CORE
				return typeof(Assemblies).GetTypeInfo().Assembly;
				#else
				return Assembly.GetExecutingAssembly();
				#endif
			}
		}
		
		/// <summary>Gets all available assemblies.</summary>
		public static Assembly[] GetAll(){
			#if NETFX_CORE
			return new Assembly[]{Current};
			#else
			return System.AppDomain.CurrentDomain.GetAssemblies();
			#endif
		}
		
	}
	
}