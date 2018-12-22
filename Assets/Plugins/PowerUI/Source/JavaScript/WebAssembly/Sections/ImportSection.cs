using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>A WebAssembly import section.</summary>
	public class ImportSection : Section{
		
		/// <summary>Imported tables.</summary>
		public List<TableImportEntry> Tables = new List<TableImportEntry>();
		/// <summary>Imported mem.</summary>
		public List<MemoryImportEntry> Memory = new List<MemoryImportEntry>();
		/// <summary>Imported functions.</summary>
		public List<FunctionImportEntry> Functions = new List<FunctionImportEntry>();
		/// <summary>Imported globals.</summary>
		public List<GlobalImportEntry> Globals = new List<GlobalImportEntry>();
		
		
		public ImportSection():base(2){}
		
		
		public override void Load(Reader reader,int length){
			
			int count=(int)reader.VarUInt32();
			
			// Read all:
			for(int i=0;i<count;i++){
				
				string module=reader.String();
				string field=reader.String();
				ExternalKind kind=(ExternalKind)reader.ReadByte();
				
				ImportEntry ie;
				
				switch(kind){
					
					case ExternalKind.Function:
						ie=new FunctionImportEntry(reader);
						Functions.Add(ie as FunctionImportEntry);
					break;
					case ExternalKind.Table:
						ie=new TableImportEntry(reader);
						Tables.Add(ie as TableImportEntry);
					break;
					case ExternalKind.Memory:
						ie=new MemoryImportEntry(reader);
						Memory.Add(ie as MemoryImportEntry);
					break;
					default:
					case ExternalKind.Global:
						ie=new GlobalImportEntry(reader);
						Globals.Add(ie as GlobalImportEntry);
					break;
					
				}
				
				// Update module etc:
				ie.Module=module;
				ie.Field=field;
				ie.Kind=kind;
				
			}
			
		}
		
	}
	
}