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
	/// A single floating point value.
	/// </summary>
	
	public class FloatValue:NumericValue{
		
		internal float Value_;
		
		
		public float Value{
			get{
				return Value_;
			}
			set{
				if(value==Value_){
					return;
				}
				
				Value_=value;
				Changed=true;
			}
		}
		
		public FloatValue(){}
		
		public FloatValue(float value){
			Value_=value;
		}
		
		public override int GetID(){
			return 202;
		}
		
		public override PropertyValue Create(){
			return new FloatValue();
		}
		
		public override PropertyValue Copy(){
			
			FloatValue value=new FloatValue();
			value.Value_=Value;
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			Value_=reader.ReadSingle();
			
		}
		
		public override void Write(Writer writer){
			
			writer.Write(Value_);
			
		}
		
		public override double ToDouble(){
			return Value_;
		}
		
		public override float ToFloat(){
			return Value_;
		}
		
		public override float ZeroOneRange(){
			return Value_;
		}
		
		public override long ToLong(){
			
			return (long)Value_;
			
		}
		
		public override ulong ToULong(){
			
			return (ulong)Value_;
			
		}
		
	}
	
}