using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A WebAssembly export entry.</summary>
	public class ExportEntry{
		
		/// <summary>Gets the external kind of the given value.</summary>
		public static ExternalKind GetKind(object value){
			
			if(value is MethodInfo){
				return ExternalKind.Function;
			}else if(value is FieldInfo){
				return ExternalKind.Global;
			}else if(value is Memory){
				return ExternalKind.Memory;
			}
			
			//	return ExternalKind.Table;
			throw new NotSupportedException("MethodInfo, FieldInfo, Table and Memory objects only.");
		}
		
		/// <summary>The field name it's exported as.</summary>
		public string Field;
		/// <summary>The raw index.</summary>
		private int Index_;
		/// <summary>The raw kind.</summary>
		private ExternalKind Kind_;
		/// <summary>The exported object. MethodInfo, a FieldInfo or a Memory object.</summary>
		private object Exported_;
		/// <summary>The module it's been exported from.</summary>
		private Module Module_;
		/// <summary>The exported object. MethodInfo, a FieldInfo or a Memory object.</summary>
		public object Exported{
			get{
				
				if(Exported_==null){
					
					// Resolve it now:
					switch(Kind_){
						case ExternalKind.Function:
							Exported = Module_.GetFunction(Index_);
						break;
						case ExternalKind.Table:
							throw new NotImplementedException("Can't export tables at the moment.");
						case ExternalKind.Memory:
							Exported = Module_.GetMemory(Index_);
						break;
						case ExternalKind.Global:
							Exported = Module_.GetGlobal(Index_);
						break;
					}
					
				}
				
				return Exported_;
			}
			set{
				Exported_=value;
				Kind_ = GetKind(value);
			}
		}
		
		
		public ExportEntry(){}
		
		/// <summary>The type of this export.</summary>
		public ExternalKind Kind{
			get{
				return Kind_;
			}
		}
		
		public ExportEntry(Reader reader,Module module){
			
			Module_ = module;
			
			// Field:
			Field=reader.String();
			
			// Kind:
			Kind_=(ExternalKind)reader.ReadByte();
			
			// Index:
			Index_=(int)reader.VarUInt32();
			
		}
		
	}
	
}