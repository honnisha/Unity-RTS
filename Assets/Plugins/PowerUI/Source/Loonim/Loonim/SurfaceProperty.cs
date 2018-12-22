using System;
using System.Collections;
using System.Collections.Generic;
using Values;


namespace Loonim{
	
	/// <summary>
	/// An instance property on a surface texture.
	/// </summary>
	
	public class SurfaceProperty{
		
		/// <summary>The ID of the property. Index in the Properties set.</summary>
		public int ID;
		/// <summary>The property name. Optional and rarely used.</summary>
		public string Name;
		/// <summary>The actual value of this property.</summary>
		public PropertyValue Value;
		
	}
	
}