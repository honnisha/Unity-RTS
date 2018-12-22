using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly memory_type.</summary>
	public class MemoryType{
		
		/// <summary>Resizable limits.</summary>
		public ResizableLimits Limits;
		
		
		public MemoryType(){}
		
		public MemoryType(Reader reader){
			// Limits:
			Limits=reader.Limits();
		}
		
	}
	
}