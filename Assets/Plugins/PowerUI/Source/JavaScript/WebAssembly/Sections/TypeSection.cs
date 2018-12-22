using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly type section.</summary>
	public class TypeSection : Section{
		
		/// <summary>The types in this section.</summary>
		public FuncType[] Types;
		
		public TypeSection():base(1){}
		
		
		public override void Load(Reader reader,int length){
			
			// # of types:
			int count=(int)reader.VarUInt32();
			
			// Load the set:
			Types=reader.FuncTypes(count,0);
			
		}
		
	}
	
}