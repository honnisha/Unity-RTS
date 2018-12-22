using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A single-byte unsigned integer indicating the kind of definition being imported or defined.</summary>
	public enum ExternalKind:int{
		
		Function=0,
		Table=1,
		Memory=2,
		Global=3
		
	}
	
}