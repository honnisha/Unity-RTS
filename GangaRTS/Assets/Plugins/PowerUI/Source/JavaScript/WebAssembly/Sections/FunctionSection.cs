using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly function section.</summary>
	public class FunctionSection : Section{
		
		/// <summary>The available function types.</summary>
		public FuncType[] Types;
		
		
		public FunctionSection():base(3){}
		
		
		public override void Load(Reader reader,int length){
			
			// Get all types from type section:
			FuncType[] allTypes=Module.TypeSection.Types;
			
			// Create types set:
			Types=new FuncType[(int)reader.VarUInt32()];
			
			for(int i=0;i<Types.Length;i++){
				
				// Read the index in the type section:
				Types[i]=allTypes[ (int) reader.VarUInt32() ];
				
			}
			
		}
		
	}
	
}