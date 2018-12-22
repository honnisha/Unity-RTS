using System;
using System.Collections.Generic;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>
	/// Hosts type builder functionality.
	/// </summary>
	public abstract class ILTypeBuilder{
		
		/// <summary>The actual type being built.</summary>
		public abstract Type Type{get;}
		
		/// <summary>Defines the default public constructor.</summary>
		public abstract ConstructorInfo DefineConstructor();
		
		/// <summary>Defines the default public constructor.</summary>
		public abstract ILGenerator DefineConstructor(MethodAttributes attribs,Type[] parameters);
		
		/// <summary>Call this when you're done building the type.</summary>
		public abstract Type Close();
		
		/// <summary>Define a method on this type.</summary>
		public abstract ILGenerator DefineMethod(
			string name,
			MethodAttributes attribs,
			Type returnType,
			Type[] parameters
		);
		
		/// <summary>Define a method on this type.</summary>
		public abstract FieldInfo DefineField(
			string name,
			Type fieldType,
			FieldAttributes attribs
		);
		
	}
	
}