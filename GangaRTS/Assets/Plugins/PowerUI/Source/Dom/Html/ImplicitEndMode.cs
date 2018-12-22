using System;


namespace Dom{
	
	/// <summary>
	/// Defines the different tokenization content models.
	/// </summary>
	public enum ImplicitEndMode:int{
		/// <summary>
		/// No implicit ends.
		/// </summary>
		None=0,
		/// <summary>
		/// Closes in normal and thorough modes.
		/// </summary>
		Normal=3,
		/// <summary>
		/// Closes in thorough mode only.
		/// </summary>
		Thorough=1
	}
	
}