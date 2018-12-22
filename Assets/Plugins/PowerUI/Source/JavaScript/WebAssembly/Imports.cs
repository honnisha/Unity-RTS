using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>Imports for the currently loading WebAssembly module.
	/// They're static so we can hopefully make use of JIT optimizations
	/// on platforms that use the JIT runtime (as usual, iOS loses out on this huge optimization).</summary>
	public static class Imports{
		
		/// <summary>Imported memory.</summary>
		public static Memory Memory;
		
		
		/// <summary>Gets the imported memory as an IntPtr.</summary>
		public static IntPtr GetMemory(){
			if(Memory==null){
				return default(IntPtr);
			}
			
			return Memory.ToPointer();
		}
		
	}
	
}