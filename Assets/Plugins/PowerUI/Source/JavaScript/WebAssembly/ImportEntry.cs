using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A WebAssembly import entry.</summary>
	public class ImportEntry{
		
		/// <summary>The name of the originating module</summary>
		public string Module;
		/// <summary>The field from the originating module.</summary>
		public string Field;
		/// <summary>The type of import.</summary>
		public ExternalKind Kind;
		
		
		public ImportEntry(){}
		
		public object MapImport(Module module){
			// Get the imported object now!
			throw new NotSupportedException();
		}
		
	}
	
	/// <summary>A 'function' kind import entry.</summary>
	public class FunctionImportEntry : ImportEntry{
		
		/// <summary>Type index of the function signature.</summary>
		public uint Type;
		/// <summary>The imported methods signature.</summary>
		public FuncType Signature;
		/// <summary>The imported method.</summary>
		internal MethodInfo ImportedMethod;
		
		
		public FunctionImportEntry(){}
		
		public FunctionImportEntry(Reader reader){
			// Type index:
			Type=reader.VarUInt32();
		}
		
		/// <summary>Get the imported method.</summary>
		public MethodInfo Get(Module module,out int paramCount,out Type returnType){
			
			if(Signature==null){
				// Get the signature:
				Signature = module.FunctionSection.Types[(int)Type];
			}
			
			paramCount = Signature.ParameterCount;
			returnType = Signature.GetReturnType();
			
			// If we're importing a compiling JS function then we
			// can potentially generate an overload which specifically
			// matches our given signature.
			if(ImportedMethod == null){
				ImportedMethod = MapImport(module) as MethodInfo;
			}
			
			return ImportedMethod;
		}
		
	}
	
	/// <summary>A 'table' kind import entry.</summary>
	public class TableImportEntry : ImportEntry{
		
		/// <summary>Type of elements in the table.</summary>
		public LanguageType ElemType;
		/// <summary>Resizable limits.</summary>
		public ResizableLimits Limits;
		
		
		public TableImportEntry(){}
		
		public TableImportEntry(Reader reader){
			// elem_type:
			ElemType=reader.ValueType();
			
			// Limits:
			Limits=reader.Limits();
		}
		
	}
	
	/// <summary>A 'memory' kind import entry.</summary>
	public class MemoryImportEntry : ImportEntry{
		
		/// <summary>Resizable limits.</summary>
		public ResizableLimits Limits;
		
		
		public MemoryImportEntry(){}
		
		public MemoryImportEntry(Reader reader){
			// Limits:
			Limits=reader.Limits();
		}
		
	}
	
	/// <summary>A 'global' kind import entry.</summary>
	public class GlobalImportEntry : ImportEntry{
		
		/// <summary>Is it mutable?</summary>
		public bool Mutability;
		/// <summary>The imported (static) field.</summary>
		internal FieldInfo ImportedField;
		/// <summary>The field type.</summary>
		private Type FieldType_;
		
		
		public GlobalImportEntry(){}
		
		public GlobalImportEntry(Reader reader){
			
			// Read a value type:
			FieldType_ = reader.ValueTypeConverted();
			
			// Mutability:
			Mutability=reader.VarUInt1();
			
		}
		
		/// <summary>Get the imported global.</summary>
		public FieldInfo GetField(Module module,out Type type){
			if(ImportedField==null){
				ImportedField=MapImport(module) as FieldInfo;
			}
			
			type = FieldType_;
			return ImportedField;
		}
		
	}
	
}