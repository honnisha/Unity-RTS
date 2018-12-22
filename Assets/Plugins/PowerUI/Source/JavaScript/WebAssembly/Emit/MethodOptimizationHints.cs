using System;
using System.Collections.Generic;

namespace WebAssembly
{

	/// <summary>
	/// Represents information useful for optimizing a method.
	/// </summary>
	public class MethodOptimizationHints
	{
		private Dictionary<string,bool> names = new Dictionary<string,bool>();
		private bool cached;
		private bool hasArguments;
		
		/// <summary>
		/// Called by the parser whenever a variable is encountered (variable being any identifier
		/// which is not a property name).
		/// </summary>
		/// <param name="name"> The variable name. </param>
		public void EncounteredVariable(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			this.names[name]=true;
			this.cached = false;
		}

		/// <summary>
		/// Determines if the parser encountered the given variable name while parsing the
		/// function, or if the function contains a reference to "eval" or the function contains
		/// nested functions which may reference the variable.
		/// </summary>
		/// <param name="name"> The variable name. </param>
		/// <returns> <c>true</c> if the parser encountered the given variable name or "eval" while
		/// parsing the function; <c>false</c> otherwise. </returns>
		public bool HasVariable(string name)
		{
			return this.names.ContainsKey(name);
		}

		/// <summary>
		/// Gets a value that indicates whether the function being generated contains a reference
		/// to the arguments object.
		/// </summary>
		public bool HasArguments
		{
			get
			{
				CacheResults();
				return this.hasArguments;
			}
		}
		
		/// <summary>
		/// Caches the HasEval and HasArguments property access.
		/// </summary>
		private void CacheResults()
		{
			if (this.cached == false)
			{
				this.hasArguments = this.names.ContainsKey("arguments");
				this.cached = true;
			}
		}
		
	}

}
