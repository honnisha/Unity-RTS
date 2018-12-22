using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>
	/// A JavaScript runtime engine. Either a ScriptEngine instance or a WebAssembly module.
	/// </summary>
	
	public partial class Runtime{
		
		/// <summary>The path where compiled WebAssembly DLL's are stored.</summary>
		public string CachePath = "WasmCache";
		
		/// <summary>Invoked when WebAssembly hits a trap line.</summary>
		public static void Trap(){
			throw new Exception("The WebAssembly runtime encountered a non-recoverable problem.");
		}
		
		/// <summary>The engine name. 'WebAssembly'.</summary>
		public virtual string EngineName{
			get{
				throw new NotImplementedException();
			}
		}
		
		/// <summary>Sets up the given assembly.</summary>
		protected virtual void SetupScopes(Assembly asm){
		
		}
		
		/// <summary>Loads an already compiled asm by its unique ID.</summary>
		public Assembly LoadAssembly(string uniqueID){
			
			if(uniqueID==null){
				return null;
			}
			
			// The full name:
			string asmName = (EngineName+"-"+uniqueID).ToLower();
			
			// Get the assembly:
			return Assembly.Load(new AssemblyName(asmName+", Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
			
		}
		
		/// <summary>Loads an already compiled blob by its unique ID into this engine.</summary>
		public CompiledCode Load(string uniqueID){
			
			// Get the assembly:
			Assembly assembly=LoadAssembly(uniqueID);
			
			// Get the main method:
			return GetMain(assembly);
		}
		
		/// <summary>Gets the main method from the given assembly.</summary>
		protected CompiledCode GetMain(Assembly assembly){
			
			if(assembly==null){
				return null;
			}
			
			// Great - got the assembly. Grab the entry point:
			Type scriptType=assembly.GetType(EngineName+"_EntryPoint");
			
			if(scriptType==null){
				return null;
			}
			
			// Get the (static) entry method called __.main:
			MethodInfo main=scriptType.GetMethod("__.main");
			
			if(main==null){
				return null;
			}
			
			// Next, we'll setup the rest of the scopes:
			SetupScopes(assembly);
			
			// Ok!
			return new CompiledCode(main);
		}
		
	}
	
}