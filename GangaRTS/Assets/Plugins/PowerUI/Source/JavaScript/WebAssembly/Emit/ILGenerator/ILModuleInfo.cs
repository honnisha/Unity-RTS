using System;
using System.Collections.Generic;
using System.Reflection;


namespace WebAssembly
{
	/// <summary>
	/// Hosts core module functionality such as declaring globals etc.
	/// </summary>
	public abstract class ILModuleInfo
	{
		
		/// <summary>The primary type which static content is added to.
		/// Note that this is usually a builder.</summary>
		public abstract ILTypeBuilder MainBuilder{get;}
		
		/// <summary>The global runtime. Either a ScriptEngine or WebAssembly module.</summary>
		public abstract FieldInfo GlobalRuntime{get;}
		
		/// <summary>A reference to the static field holding the memory void* (WebAssembly only).</summary>
		public abstract FieldInfo Memory{get;set;}
		
		/// <summary>The imported global scope.</summary>
		public abstract FieldInfo GlobalImports{get;}
		
		/// <summary>Sets up a global cache.</summary>
		public abstract void SetupGlobalCache(object imports);
		
		/// <summary>Create a global.</summary>
		public abstract FieldInfo CreateGlobal(Type type);
		
		/// <summary>Creates the main global type.</summary>
		public abstract void CreateMainType(string uniqueID);
		
		/// <summary>Completes the module.</summary>
		public abstract Type Close();
		
		/// <summary>Starts defining a new type in this module.</summary>
		public abstract ILTypeBuilder AllocateType(ref string name,Type baseType,bool isStatic);
		
	}
	
}