//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//   Kulestar would like to thank the following:
//    PDF.js, Microsoft, Adobe and opentype.js
//    For providing implementation details and
// specifications for the TTF and OTF file formats.
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;


namespace InfiniText{
	
	/// <summary>
	/// A glyph substitution.
	/// It must have already matched on the first character in order to test the rest 
	/// (if there's multiple chars to match, e.g. an ffi ligature would match on 'f' first).
	/// </summary>
	
	public class Substitution{
		
	}
	
	/// <summary>
	/// Used when a character may potentially be substituted in more than one way.
	/// The testing order is as it was defined in the file.
	/// </summary>
	
	public class SubstitutionSet : Substitution{
		
		
		
	}
	
}