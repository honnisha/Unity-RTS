using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly module section.</summary>
	public class Section{
		
		/// <summary>Section ID.</summary>
		public int ID;
		/// <summary>Optional section name. Only applicable when ID is 0.</summary>
		public string Name;
		/// <summary>The module this section belongs to. Always set before Load is called.</summary>
		public Module Module;
		/// <summary>Payload for unrecognised sections.</summary>
		public byte[] UnRecognisedPayload;
		
		
		/// <summary>A numbered section.</summary>
		public Section(int id){
			ID=id;
		}
		
		/// <summary>A named custom section.</summary>
		public Section(string name){
			Name=name;
		}
		
		/// <summary>Loads the sections payload from the given reader.</summary>
		public virtual void Load(Reader reader,int length){
			
			// Default for unrecognised - just read that many bytes:
			UnRecognisedPayload=reader.ReadBytes(length);
			
		}
		
		
		/// <summary>Creates a WebAssembly section with the given ID and optional name.</summary>
		public static Section Create(int id,string name){
			
			switch(id){
				
				case 0:
					
					if(name=="name"){
						// Name section:
						return new NameSection();
					}
					
					// Other/custom
					return new Section(name);
				
				case 1:
					// Function signatures
					return new TypeSection();
				case 2:
					// Imports
					return new ImportSection();
				case 3:
					// Function declarations
					return new FunctionSection();
				case 4:
					// Indirect function table/ other tables
					return new TableSection();
				case 5:
					// Memory attributes
					return new MemorySection();
				case 6:
					// Global declarations
					return new GlobalSection();
				case 7:
					// Exports
					return new ExportSection();
				case 8:
					// Start function declaration
					return new StartSection();
				case 9:
					// Elements section
					return new ElementSection();
				case 10:
					// Function bodies
					return new CodeSection();
				case 11:
					// Data segments
					return new DataSection();
					
				default:
					
					// Unrecognised numbered section
					return new Section(id);
				
			}
			
		}
		
		
	}
	
}