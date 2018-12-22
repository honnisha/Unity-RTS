using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly table_type.</summary>
	public class TableType{
		
		/// <summary>The type of elements in the table.</summary>
		public LanguageType ElemType;
		/// <summary>Resizable limits.</summary>
		public ResizableLimits Limits;
		
		
		public TableType(){}
		
		public TableType(Reader reader){
			// elem_type:
			ElemType=reader.ValueType();
			
			// Limits:
			Limits=reader.Limits();
		}
		
	}
	
}