using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>Attributes for opcodes.</summary>
	public enum OpCodeAttributes : int{
		
		None = 0,
		OneInput = 1,
		TwoInputs = 2,
		ThreeInputs = 3,
		FourInputs = 4,
		
		AllowedInInit = 16,
		
		BlockStart = 32,
		BlockEnd = 64,
		BlockPart = 32 | 64
		
	}
	
}