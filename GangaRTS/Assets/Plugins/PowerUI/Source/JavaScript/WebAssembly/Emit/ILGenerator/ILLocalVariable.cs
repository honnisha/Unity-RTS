using System;
using System.Collections.Generic;

namespace WebAssembly
{

	/// <summary>
	/// Represents a local variable in CIL code.
	/// </summary>
	public abstract class ILLocalVariable
	{
		/// <summary>
		/// Gets the zero-based index of the local variable within the method body.
		/// </summary>
		public abstract int Index
		{
			get;
		}

		/// <summary>
		/// Gets the type of the local variable.
		/// </summary>
		public abstract Type Type
		{
			get;
		}

		/// <summary>
		/// Gets the local variable name, or <c>null</c> if a name was not provided.
		/// </summary>
		public abstract string Name
		{
			get;
		}
	}
	
}
