using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A WebAssembly global_variable.</summary>
	public class GlobalVariable{
		
		public int Index;
		/// <summary>The type of the global.</summary>
		public Type Type;
		/// <summary>The initialiser which must be followed by the end opcode.</summary>
		public object Init;
		/// <summary>The compiling static field.</summary>
		internal FieldInfo Field;
		
		
		/// <summary>Gets the field info for this global.</summary>
		internal FieldInfo GetField(Module module,out Type type){
			
			if(Field==null){
				Field = module.DefineField("global_"+Index,Type);
			}
			
			type = Type;
			return Field;
		}
		
		
		public GlobalVariable(){}
		
		public GlobalVariable(Reader reader,int index){
			
			Type = reader.ValueTypeConverted();
			reader.VarUInt1(); // Mutability
			Init = reader.InitExpression();
			Index = index;
		}
		
	}
	
}
