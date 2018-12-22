using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>A WebAssembly export section.</summary>
	public class ExportSection : Section{
		
		/// <summary>All exports in this section (export name must be unique).</summary>
		public Dictionary<string,ExportEntry> Entries = new Dictionary<string,ExportEntry>();
		
		
		public ExportSection():base(7){}
		
		
		/// <summary>Gets the export entry with the given name.</summary>
		public ExportEntry Get(string name){
			ExportEntry entry;
			Entries.TryGetValue(name,out entry);
			return entry;
		}
		
		public override void Load(Reader reader,int length){
			
			// Export count:
			int count=(int)reader.VarUInt32();
			
			for(int i=0;i<count;i++){
				
				// Load it:
				ExportEntry ee=new ExportEntry(reader,Module);
				
				// Add it:
				Entries[ee.Field]=ee;
				
			}
			
		}
		
	}
	
}