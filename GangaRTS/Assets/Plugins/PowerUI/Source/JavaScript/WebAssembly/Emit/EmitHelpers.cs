using System;


namespace WebAssembly
{
	/// <summary>
	/// Outputs IL for misc tasks.
	/// </summary>
	internal static class EmitHelpers
	{
		
		/// <summary>
		/// Pushes a reference to the ScriptEngine or WebAssembly module onto the stack.
		/// </summary>
		/// <param name="generator"> The IL generator. </param>
		public static void LoadRuntime(ILGenerator generator){
			
			// Get the static ref to the runtime:
			generator.LoadField(generator.Runtime.ModuleInfo.GlobalRuntime);
			
		}
		
	}

}
