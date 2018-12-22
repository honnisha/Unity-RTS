using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>A WebAssembly module.</summary>
	public partial class Module : Runtime{
		
		/// <summary>True if IL should be visible when compiling.</summary>
		public static bool EnableILAnalysis;
		
		/// <summary>The current reader. Used only during loading.</summary>
		internal Reader Reader;
		/// <summary>Module version.</summary>
		public uint Version;
		/// <summary>Module sections.</summary>
		public List<Section> Sections;
		
		
		/// <summary>Minimum module.</summary>
		public Module(){
			
			// Setup section list:
			Sections=new List<Section>();
			Version = 0xd;
			
		}
		
		/// <summary>Logs messages.</summary>
		public static void Log(string message){
			UnityEngine.Debug.Log(message);
		}
		
		/// <summary>Queries the current memory size in WebAssembly pages (64kb).</summary>
		public int QueryMemory(){
			return Memory.Pages;
		}
		
		/// <summary>Grows memory by the given number of pages.</summary>
		public int GrowMemory(int pages){
			return Memory.Grow(pages);
		}
		
		/// <summary>Loads a module from the file path.</summary>
		public Module(string path) : this(new Reader(File.ReadAllBytes(path))){}
		
		/// <summary>Loads a module from the given bytes.</summary>
		public Module(byte[] data) : this(new Reader(data)){}
		
		/// <summary>Loads a module from the given reader.</summary>
		public Module(Reader reader){
			
			reader.Module = this;
			Reader = reader;
			
			// Magic number:
			if(reader.UInt32() != 0x6d736100){
				throw new Exception("Not a WebAssembly file.");
			}
			
			// Version:
			Version=reader.UInt32();
			
			if(Version!=0xd && Version!=1){
				throw new Exception("This loader can't load that WebAssembly version ("+Version+").");
			}
			
			// Setup section list:
			Sections=new List<Section>();
			
			// Load the sections next:
			while(reader.IsMore){
				
				// Section id:
				int id=(int)reader.VarUInt7();
				
				// Payload length:
				int size=(int)reader.VarUInt32();
				
				// Section name (optional):
				string name=null;
				
				if(id==0){
					
					// Section start:
					int sectionStart=(int)reader.BaseStream.Position;
					
					// Name:
					name=reader.String();
					
					// Already read off this many bytes:
					size-=( sectionStart-(int)reader.BaseStream.Position );
					
				}
				
				// Create a section:
				Section s=Section.Create(id,name);
				
				// Set module:
				s.Module=this;
				
				// Load the section now:
				s.Load(reader,size);
				
				// Add it:
				Add(s);
				
			}
			
		}
		
		/// <summary>Gets linear memory by index. mIndex must be 0 in the MVP.</summary>
		public Memory GetMemory(int mIndex){
			
			if(mIndex!=0){
				throw new NotSupportedException("WebAssembly MVP can only use one linear memory.");
			}
			
			return Memory;
		}
		
		/// <summary>Convenience method for invoking the named export.</summary>
		public object Invoke(string exportName,params object[] args){
			
			// Should be a method info:
			MethodInfo mInfo = GetExport(exportName) as MethodInfo;
			
			if(mInfo == null){
				throw new Exception("Export method '"+exportName+"' doesn't exist (or isn't a function).");
			}
			
			return mInfo.Invoke(null,args);
		}
		
		/// <summary>Gets the named export (case sensitive).</summary>
		public object GetExport(string name){
			
			if(ExportSection_==null){
				return null;
			}
			
			// Get the entry:
			ExportEntry ee=ExportSection_.Get(name);
			
			if(ee==null){
				return null;
			}
			
			return ee.Exported;
		}
		
		/// <summary>Gets a global by index.</summary>
		public FieldInfo GetGlobal(int gIndex){
			Type type;
			return GetGlobal(gIndex,out type);
		}
		
		/// <summary>Gets a global by index.</summary>
		public FieldInfo GetGlobal(int gIndex,out Type fieldType){
			
			int importCount = ImportSection_.Globals.Count;
			
			if(gIndex >= importCount){
				// Locally defined.
				return GlobalSection_.Globals[gIndex - importCount].GetField(this,out fieldType);
			}
			
			if(gIndex<0){
				// Out of range.
				throw new Exception("A WebAssembly global index was out of range.");
			}
			
			// Imported:
			return ImportSection_.Globals[gIndex].GetField(this,out fieldType);
		}
		
		/// <summary>Gets a function by index.</summary>
		public MethodInfo GetFunction(int functionIndex){
			int pCount;
			Type returnType;
			return GetFunction(functionIndex,out pCount,out returnType);
		}
		
		/// <summary>Gets a function by index.</summary>
		public MethodInfo GetFunction(int functionIndex,out int paramCount,out Type returnType){
			
			int importCount = ImportSection_==null ? 0 : ImportSection_.Functions.Count;
			
			if(functionIndex >= importCount){
				// Locally defined.
				return FunctionSection_.Types[functionIndex - importCount].GetSignature(this,out paramCount,out returnType);
			}
			
			if(functionIndex<0){
				// Out of range.
				throw new Exception("A WebAssembly function index was out of range.");
			}
			
			// Imported:
			return ImportSection_.Functions[functionIndex].Get(this,out paramCount,out returnType);
		}
		
		/// <summary>Adds a section to this module.</summary>
		public void Add(Section section){
			
			// Add to lookup:
			Sections.Add(section);
			section.Module=this;
			
			// Handle quick refs:
			switch(section.ID){
				case 1:
					TypeSection_ = section as TypeSection;
				break;
				case 2:
					ImportSection_ = section as ImportSection;
				break;
				case 3:
					FunctionSection_ = section as FunctionSection;
				break;
				case 4:
					TableSection_ = section as TableSection;
				break;
				case 5:
					MemorySection_ = section as MemorySection;
				break;
				case 6:
					GlobalSection_ = section as GlobalSection;
				break;
				case 7:
					ExportSection_ = section as ExportSection;
				break;
				case 8:
					StartSection_ = section as StartSection;
				break;
				case 9:
					ElementSection_ = section as ElementSection;
				break;
				case 10:
					CodeSection_ = section as CodeSection;
				break;
				case 11:
					DataSection_ = section as DataSection;
				break;
			}
			
		}
		
		/// <summary>The engine name.</summary>
		public override string EngineName{
			get{
				return "WebAssembly";
			}
		}
		
		/// <summary>Gets the first section with the given ID.
		/// Null if it wasn't found.</summary>
		public Section GetSection(int id){
			
			for(int i=0;i<Sections.Count;i++){
				
				if(Sections[i].ID==id){
					return Sections[i];
				}
				
			}
			
			return null;
			
		}
		
		private TypeSection TypeSection_;
		private ImportSection ImportSection_;
		private FunctionSection FunctionSection_;
		private TableSection TableSection_;
		private MemorySection MemorySection_;
		private GlobalSection GlobalSection_;
		private ExportSection ExportSection_;
		private StartSection StartSection_;
		private ElementSection ElementSection_;
		private CodeSection CodeSection_;
		private DataSection DataSection_;
		
		/// <summary>Ensures each globals FieldInfo and each functions MethodInfo is correct.</summary>
		public void CollectMethods(){
			
			// Get all the fields and methods from the main type:
			Type type = Compiled.GetType(EngineName+"_EntryPoint");
			
			if(GlobalSection_!=null){
				
				FieldInfo[] globalFields = type.GetFields();
				
				// Quick ref to globals array:
				GlobalVariable[] globals = GlobalSection_.Globals; 
				
				// For each global..
				for(int i=0;i<globalFields.Length;i++){
					
					// Get the ID from the name:
					string name = globalFields[i].Name;
					
					if(name.StartsWith("global_")){
						
						// Get the index as a number:
						int index = int.Parse(name.Substring(7));
						
						// Apply the fieldInfo:
						globals[index].Field = globalFields[i];
						
					}
					
				}
				
			}
			
			if(TypeSection_!=null){
				
				FuncType[] methods = TypeSection_.Types;
				
				// Get all the methods:
				MethodInfo[] availableMethods = type.GetMethods();
				
				for(int i=0;i<availableMethods.Length;i++){
					
					// Get the ID from the name:
					string name = availableMethods[i].Name;
					
					if(name.StartsWith("Func_")){
						
						// Get the index as a number:
						int index = int.Parse(name.Substring(5));
						
						// Apply the MethodInfo:
						methods[index].Signature_ = availableMethods[i];
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>The type section (1).</summary>
		public TypeSection TypeSection{
			get{
				return TypeSection_;
			}
		}
		
		/// <summary>The import section (2).</summary>
		public ImportSection ImportSection{
			get{
				return ImportSection_;
			}
		}
		
		/// <summary>The function section (3).</summary>
		public FunctionSection FunctionSection{
			get{
				return FunctionSection_;
			}
		}
		
		/// <summary>The table section (4).</summary>
		public TableSection TableSection{
			get{
				return TableSection_;
			}
		}
		
		/// <summary>The memory section (5).</summary>
		public MemorySection MemorySection{
			get{
				return MemorySection_;
			}
		}
		
		/// <summary>The global section (6).</summary>
		public GlobalSection GlobalSection{
			get{
				return GlobalSection_;
			}
		}
		
		/// <summary>The export section (7).</summary>
		public ExportSection ExportSection{
			get{
				return ExportSection_;
			}
		}
		
		/// <summary>The start section (8).</summary>
		public StartSection StartSection{
			get{
				return StartSection_;
			}
		}
		
		/// <summary>The element section (9).</summary>
		public ElementSection ElementSection{
			get{
				return ElementSection_;
			}
		}
		
		/// <summary>The code section (10).</summary>
		public CodeSection CodeSection{
			get{
				return CodeSection_;
			}
		}
		
		/// <summary>The data section (11).</summary>
		public DataSection DataSection{
			get{
				return DataSection_;
			}
		}
		
	}
	
}