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
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Thrown whenever DOM actions fail.
	/// </summary>
	
	public class DOMException : Exception{
		
		/// <summary>The exception code.</summary>
		public ushort code;
		/// <summary>More specific code.</summary>
		public ushort subCode;
		
		
		public DOMException(ushort c):base(GetString(c,0)){
			code=c;
		}
		
		public DOMException(ushort c,ushort sub):base(GetString(c,sub)){
			code=c;
			subCode=sub;
		}
		
		public const ushort INDEX_SIZE_ERR=1;
		public const ushort DOMSTRING_SIZE_ERR=2;
		public const ushort HIERARCHY_REQUEST_ERR=3;
		public const ushort WRONG_DOCUMENT_ERR=4;
		public const ushort INVALID_CHARACTER_ERR=5; 
		public const ushort NO_DATA_ALLOWED_ERR=6;
		public const ushort NO_MODIFICATION_ALLOWED_ERR=7;
		public const ushort NOT_FOUND_ERR=8;
		public const ushort NOT_SUPPORTED_ERR=9;
		public const ushort INUSE_ATTRIBUTE_ERR=10;
		public const ushort INVALID_STATE_ERR=11;
		public const ushort SYNTAX_ERR=12;
		public const ushort INVALID_MODIFICATION_ERR=13;
		public const ushort NAMESPACE_ERR=14;
		public const ushort INVALID_ACCESS_ERR=15;
		public const ushort VALIDATION_ERR=16;
		public const ushort TYPE_MISMATCH_ERR=17;
		
		
		public static string GetString(ushort code,ushort subCode){
			
			switch(code){
				case INDEX_SIZE_ERR:
					return "Index size error";
				case DOMSTRING_SIZE_ERR:
					return "Domstring size error";
				case HIERARCHY_REQUEST_ERR:
					return "Hierarchy request error";
				case WRONG_DOCUMENT_ERR:
					return "Wrong document";
				case INVALID_CHARACTER_ERR:
					return "Invalid character";
				case NO_DATA_ALLOWED_ERR:
					return "No data allowed";
				case NO_MODIFICATION_ALLOWED_ERR:
					return "No modification allowed";
				case NOT_FOUND_ERR:
					return "Not found";
				case NOT_SUPPORTED_ERR:
					return "Not supported";
				case INUSE_ATTRIBUTE_ERR:
					return "Inuse attribute";
				case INVALID_STATE_ERR:
					return "Invalid state";
				case SYNTAX_ERR:
					return "Syntax error ("+subCode+")";
				case INVALID_MODIFICATION_ERR:
					return "Invalid modification";
				case NAMESPACE_ERR:
					return "Namespace error";
				case INVALID_ACCESS_ERR:
					return "Invalid access";
				case VALIDATION_ERR:
					return "Validation error";
				case TYPE_MISMATCH_ERR:
					return "Type mismatch";
			}
			
			return "Unknown";
			
		}
		
	}

}