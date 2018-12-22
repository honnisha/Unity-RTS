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
	/// A single value for a particular mapped property.
	/// Range is 0+.
	/// </summary>
	
	public class UnsignedNumberValue:NumericValue{
		
		public ulong Value;
		
		
		public UnsignedNumberValue(){}
		
		public UnsignedNumberValue(ulong value){
			Value=value;
		}
		
		public override int GetID(){
			return 203;
		}
		
		public override PropertyValue Create(){
			return new UnsignedNumberValue();
		}
		
		public override PropertyValue Copy(){
			
			UnsignedNumberValue value=new UnsignedNumberValue();
			value.Value=Value;
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			Value=reader.ReadCompressed();
			
		}
		
		public override void Write(Writer writer){
			
			writer.WriteCompressed(Value);
			
		}
		
		public override double ToDouble(){
			return (double)Value;
		}
		
		public override float ToFloat(){
			return (float)Value;
		}
		
		public override float ZeroOneRange(){
			// Single byte caps out at 250:
			return (float)Value / 250f;
		}
		
		public override long ToLong(){
			
			return (long)Value;
			
		}
		
		public override ulong ToULong(){
			
			return Value;
			
		}
		
	}
	
}