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
	/// </summary>
	
	public class TextValue:PropertyValue{
		
		public string Value;
		
		
		public TextValue(){}
		
		public TextValue(string value){
			Value=value;
		}
		
		public override int GetID(){
			return 200;
		}
		
		public override PropertyValue Create(){
			return new TextValue();
		}
		
		public override PropertyValue Copy(){
			
			TextValue value=new TextValue();
			value.Value=Value;
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			Value=reader.ReadString();
			
		}
		
		public override void Write(Writer writer){
			
			writer.WriteString(Value);
			
		}
		
	}
	
}