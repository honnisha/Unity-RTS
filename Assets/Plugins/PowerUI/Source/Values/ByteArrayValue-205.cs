//--------------------------------------
//           Property values 
// standard set of referenceable values
//   Used mainly by Blade and Loonim.
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using BinaryIO;


namespace Values{
	
	/// <summary>
	/// A byte[] block.
	/// </summary>
	
	public class ByteArrayValue:NumericValue{
		
		/// <summary>The max size of these (100MB).</summary>
		public const int MaxSize=100000000;
		
		public byte[] Value;
		
		
		public ByteArrayValue(){}
		
		public ByteArrayValue(byte[] value){
			Value=value;
		}
		
		public override int GetID(){
			return 205;
		}
		
		public override PropertyValue Create(){
			return new ByteArrayValue();
		}
		
		public override PropertyValue Copy(){
			
			ByteArrayValue value=new ByteArrayValue();
			value.Value=Value;
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			// Get the size:
			int size=(int)reader.ReadCompressed();
			
			if(size>MaxSize){
				return;
			}
			
			// Create now:
			Value=new byte[size];
			
		}
		
		public override void Write(Writer writer){
			
			if(Value!=null){
				
				// Write the length:
				writer.WriteCompressed((ulong)Value.Length);
				
				// Write the bytes:
				writer.Write(Value);
				
			}else{
				
				// Empty block:
				writer.WriteCompressed(0);
			}
			
		}
		
	}
	
}