//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Reflection;


namespace Dom{
	
	/// <summary>
	/// Stores information about a supported tag in a particular namespace.
	/// For example, info about 'div' in the 'xhtml' namespace.
	/// See MLNamespace.Tags (where these instances are stored).
	/// </summary>
	
	public class SupportedTagMeta{
		
		/// <summary>The type to instance.</summary>
		public Type TagType;
		/// <summary>See CloseMethod. Cached method.</summary>
		private MethodInfo CloseMethod_;
		/// <summary>The method that the lexer will run when a /close tag of this type is seen.
		/// These are instance methods which must act like they're static.
		/// That's in order to totally avoid the lookup in balanced DOM cases (the vast majority of the time).
		/// I.e. it simply calls the instance method on the open tag instance already on the parsers 'open element stack'.</summary>
		public MethodInfo CloseMethod{
			get{
				
				if(CloseMethod_==null){
					// Pull the close method now:
					CloseMethod_=TagType.GetMethod("OnLexerCloseNode");
				}
				
				return CloseMethod_;
				
			}
		}
		
		
		public SupportedTagMeta(Type tagType){
			TagType=tagType;
			
		}
		
	}
	
}	