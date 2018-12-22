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

namespace JavaScript{
	
	/// <summary>
	/// Used for security domains. Represents an assembly that types may be found in.
	/// </summary>
	
	public class CodeAssembly{
		
		/// <summary>The name of the assembly.</summary>
		public string Name;
		/// <summary>True if this is the current assembly.</summary>
		public bool Current;
		/// <summary>True if this is a Nitro assembly.</summary>
		public bool NitroAOT;
		/// <summary>The assembly itself.</summary>
		public Assembly Assembly;
		
		
		/// <summary>Creates a code assembly with the given assembly object.</summary>
		/// <param name="assembly">The assembly to use.</param>
		public CodeAssembly(Assembly assembly,bool current){
			Current=current;
			Assembly=assembly;
			string[] pieces=assembly.FullName.Split(new char[]{','},2);
			Name=pieces[0].ToLower();
			NitroAOT=Name.EndsWith(".ntro");
		}
		
		/// <summary>Attempts to get the named type from this assembly.</summary>
		/// <param name="name">The name of the type to find.</param>
		/// <returns>A system type if it was found; null otherwise.</returns>
		public Type GetType(string name){
			#if UNITY_METRO
			return Assembly.GetType(name);
			#elif UNITY_WP8
			return Assembly.GetType(name,false);
			#else
			return Assembly.GetType(name,false,true);
			#endif
		}
		
	}
	
}