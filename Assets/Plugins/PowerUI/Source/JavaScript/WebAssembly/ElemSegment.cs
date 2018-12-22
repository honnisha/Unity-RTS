using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly elem_segment.</summary>
	public class ElemSegment{
		
		/// <summary>The table index.</summary>
		public uint Index;
		/// <summary>Computes the offset at which to place the elements.</summary>
		public object Offset;
		/// <summary>A sequence of function indices.</summary>
		public uint[] Elems;
		
		
		public ElemSegment(){}
		
		public ElemSegment(Reader reader){
			
			// Index:
			Index=reader.VarUInt32();
			
			// Offset:
			Offset=reader.InitExpression();
			
			// Elements:
			Elems=new uint[ (int) reader.VarUInt32()];
			
			for(int i=0;i<Elems.Length;i++){
				
				// Read it:
				Elems[i]=reader.VarUInt32();
				
			}
			
		}
		
	}
	
}