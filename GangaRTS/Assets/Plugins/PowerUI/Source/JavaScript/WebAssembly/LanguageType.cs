using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>Various types of values. 
	/// The integer representation is the raw VarInt7 byte.</summary>
	public enum LanguageType:int{
		
		i32 = 0x7f, // -0x01,
		i64 = 0x7e, // -0x02,
		f32 = 0x7d, // -0x03,
		f64 = 0x7c, // -0x04,
		anyfunc = 0x70, // -0x10,
		func = 0x60, // -0x20,
		empty_block_type = 0x40 // -0x40
		
	}
	
	public static class LanguageTypes{
		
		/// <summary>Converts a language type to a (signed) type.</summary>
		public static Type ToType(LanguageType type){
			
			switch(type){
				case LanguageType.i32:
					return typeof(int);
				case LanguageType.i64:
					return typeof(long);
				case LanguageType.f32:
					return typeof(float);
				case LanguageType.f64:
					return typeof(double);
			}
			
			return typeof(void);
			
		}
		
	}
	
}