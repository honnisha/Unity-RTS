using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>resizable_limits.</summary>
	public struct ResizableLimits{
		
		/// <summary>True if a max is included (Flags).</summary>
		public bool Flags;
		/// <summary>Initial length.</summary>
		public uint Initial;
		/// <summary>Max length. Valid if HasMax is true.</summary>
		public uint Maximum;
		
		
		public ResizableLimits(bool flags,uint init,uint max){
			Flags=flags;
			Initial=init;
			Maximum=max;
		}
		
	}
	
}
	