using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly data section.</summary>
	public class DataSection : Section{
		
		/// <summary>Data segments.</summary>
		public DataSegment[] Entries;
		
		
		public DataSection():base(11){}
		
		
		public override void Load(Reader reader,int length){
			
			// Create set:
			Entries=new DataSegment[(int)reader.VarUInt32()];
			
			for(int i=0;i<Entries.Length;i++){
				
				// Load it:
				Entries[i]=new DataSegment(reader);
				
			}
			
		}
		
	}
	
}