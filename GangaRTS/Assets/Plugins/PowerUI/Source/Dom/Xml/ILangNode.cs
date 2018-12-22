//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;


namespace Dom{
	
	/// <summary>
	/// Contains &customVariables;.
	/// </summary>
	public interface ILangNode{
		
		/// <summary>Sets the runtime argument set. See the Arguments variable.</summary>
		/// <param name="arguments">The new arguments.</param>
		void SetArguments(string[] arguments);
		
		/// <summary>Changes the name of the variable. Thats the text used &Here;.</summary>
		void SetVariableName(string name);
		
		/// <summary>Loads the content of this variable element by looking up the
		/// content for the variables name.</summary>
		void LoadNow();
		
	}
	
	
	
}