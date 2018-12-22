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
	/// Used to dynamically reference types.
	/// </summary>
	
	public class CodeReference{
		
		/// <summary>The assembly 'this' is defined in.</summary>
		public static Assembly CurrentAssembly;
		/// <summary>All loaded assemblies</summary>
		public static Dictionary<string,CodeAssembly> Assemblies;
		
		
		/// <summary>Gets the first type found with the given name from the set of all assemblies.</summary>
		/// <param name="name">The type to look for.</param>
		/// <returns>The type, if it was found.</returns>
		public static Type GetFirstType(string name){
			
			if(Assemblies==null){
				Setup();
			}
			
			#if !NETFX_CORE
			// Unused on Windows Store.
			
			// For each assembly we have available..
			foreach(KeyValuePair<string,CodeAssembly> kvp in Assemblies){
				// Attempt to get the type from it:
				Type result=kvp.Value.GetType(name);
				
				if(result!=null){
					// Great, we found it!
					return result;
				}
			}
			
			#endif
			
			// Didn't find it.
			return null;
		}
		
		/// <summary>Gets an available assembly by name, if it's found. Null otherwise.</summary>
		/// <param name="name">The name of the assembly to find.</param>
		public static Assembly GetAssembly(string name){
			
			if(Assemblies==null){
				Setup();
			}
			
			#if NETFX_CORE && !UNITY_EDITOR
			// Used to find Nitro DLLs only.
			
			return Assembly.Load(new AssemblyName(name+", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
			
			#else
			CodeAssembly result;
			
			if(Assemblies.TryGetValue(name,out result)){
				return result.Assembly;
			}
			
			return null;
			#endif
			
		}
		
		/// <summary>Called on demand to setup the Assemblies array.</summary>
		public static void Setup(){
			if(Assemblies!=null){
				return;
			}
			
			CurrentAssembly=JavaScript.Assemblies.Current;
			
			#if !NETFX_CORE
			Assembly[] assemblySet=JavaScript.Assemblies.GetAll();
			Assemblies=new Dictionary<string,CodeAssembly>(assemblySet.Length);
			
			for(int i=0;i<assemblySet.Length;i++){
				Assembly assembly=assemblySet[i];
				AddAssembly(assembly);
			}
			
			#endif
		}
		
		/// <summary>Adds an assembly to the code assembly set.</summary>
		public static void AddAssembly(Assembly assembly){
			
			if(Assemblies==null){
				Setup();
			}
			
			CodeAssembly codeAssembly=new CodeAssembly(assembly,(assembly==CurrentAssembly));
			Assemblies[codeAssembly.Name]=codeAssembly;
		}
		
		
		/// <summary>The reference text.</summary>
		public string Reference;
		/// <summary>The assembly this reference points to.</summary>
		public Assembly InAssembly;
		
		
		/// <summary>Creates a new code reference with the given reference text.</summary>
		/// <param name="reference">The reference text. Note that it must include the assembly name at the start. E.g. System.System.Text.</param>
		public CodeReference(string reference){
			Reference=reference;
			if(reference==""){
				return;
			}
			
			Setup();
			
			string[] pieces=reference.Split('.');
			
			if(pieces.Length==1){
				// No assembly given - it's just a single, non-nested namespace.
				return;
			}
			
			string assemblyName=pieces[0].Replace('*','.').ToLower();
			
			// Drop the assembly from the reference:
			Reference="";
			for(int i=1;i<pieces.Length;i++){
				if(i!=1){
					Reference+=".";
				}
				Reference+=pieces[i];
			}
			
			if(assemblyName==""){
				InAssembly=CurrentAssembly;
				return;
			}
			
			// Which assembly is reference (e.g. System.Generics) in?
			InAssembly=GetAssembly(assemblyName);
		}
		
		/// <summary>Attempts to get the type by name through this reference.</summary>
		/// <param name="name">The name of the type to find.</param>
		/// <returns>The system type, if found. Null otherwise.</returns>
		public Type GetType(string name){
			#if NETFX_CORE
			if(InAssembly==null){
				return CurrentAssembly.GetType(Reference+"."+name);
			}else{
				return InAssembly.GetType(Reference+"."+name);
			}
			#elif UNITY_WP8
			if(InAssembly==null){
				return CurrentAssembly.GetType(Reference+"."+name,false);
			}else{
				return InAssembly.GetType(Reference+"."+name,false);
			}
			#else
			if(InAssembly==null){
				return CurrentAssembly.GetType(Reference+"."+name,false,true);
			}else{
				return InAssembly.GetType(Reference+"."+name,false,true);
			}
			#endif
		}
		
	}
	
}