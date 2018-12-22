using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly 'memory_immediate'.</summary>
	public struct MemoryImmediate{
		
		public uint Flags;
		public uint Offset;
		
		
		public MemoryImmediate(uint flags,uint offset){
			Flags=flags;
			Offset=offset;
		}
		
	}
	
}