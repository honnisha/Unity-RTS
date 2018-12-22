using System;
using System.Collections.Generic;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>
	/// Represents the result of compiling a script.
	/// </summary>
	public sealed class CompiledCode
	{
		private MethodInfo globalMethod;

		internal CompiledCode(MethodInfo globalMethod){
			this.globalMethod = globalMethod;
		}
		
		/// <summary>
		/// Executes the compiled script.
		/// </summary>
		public object Execute(){
			return globalMethod.Invoke(null,null);
		}
		
	}
}
