using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly element section.</summary>
	public class ElementSection : Section{
		
		/// <summary>All exports in this section.</summary>
		public ElemSegment[] Entries;
		
		
		public ElementSection():base(9){}
		
		
		public override void Load(Reader reader,int length){
			
			// Create set:
			Entries=new ElemSegment[(int)reader.VarUInt32()];
			
			for(int i=0;i<Entries.Length;i++){
				
				// Load it:
				Entries[i]=new ElemSegment(reader);
				
			}
			
		}
		
	}
	
}