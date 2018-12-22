using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly table section.</summary>
	public class TableSection : Section{
		
		/// <summary>The table entries.</summary>
		public TableType[] Entries;
		
		
		public TableSection():base(4){}
		
		
		public override void Load(Reader reader,int length){
			
			// Create set:
			Entries=new TableType[(int)reader.VarUInt32()];
			
			for(int i=0;i<Entries.Length;i++){
				
				// Load it:
				Entries[i]=new TableType(reader);
				
			}
			
		}
		
	}
	
}