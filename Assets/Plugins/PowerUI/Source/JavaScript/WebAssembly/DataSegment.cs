using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly data_segment.</summary>
	public class DataSegment{
		
		/// <summary>The table index.</summary>
		public uint Index;
		/// <summary>Computes the offset at which to place the elements.</summary>
		public object Offset;
		/// <summary>The data itself.</summary>
		public byte[] Data;
		
		
		public DataSegment(){}
		
		public DataSegment(Reader reader){
			
			// Index:
			Index=reader.VarUInt32();
			
			// Offset:
			Offset=reader.InitExpression();
			
			// Data:
			Data=reader.ReadBytes( (int) reader.VarUInt32() );
			
		}
		
	}
	
}