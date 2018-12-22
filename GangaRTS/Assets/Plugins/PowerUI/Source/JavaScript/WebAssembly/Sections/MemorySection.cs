using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly memory section.</summary>
	public class MemorySection : Section{
		
		/// <summary>All entries in this section.</summary>
		public MemoryType[] Entries;
		
		
		public MemorySection():base(5){}
		
		
		public override void Load(Reader reader,int length){
			
			// Create set:
			Entries=new MemoryType[(int)reader.VarUInt32()];
			
			for(int i=0;i<Entries.Length;i++){
				
				// Load it:
				Entries[i]=new MemoryType(reader);
				
			}
			
		}
		
	}
	
}