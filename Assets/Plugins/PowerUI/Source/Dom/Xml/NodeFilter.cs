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

using Dom;
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Delegate used by acceptNode().
	/// </summary>
	public delegate ushort NodeFilterDelegate(Node node);
	
	/// <summary>
	/// The various NodeFilter values.
	/// </summary>
	
	public class NodeFilter{
		
		public const ushort FILTER_ACCEPT=1;
		public const ushort FILTER_REJECT=0;
		public const ushort FILTER_SKIP=2;
		
		public const ulong SHOW_ALL=uint.MaxValue;
		public const ulong SHOW_ATTRIBUTE=2;
		public const ulong SHOW_CDATA_SECTION=8;
		public const ulong SHOW_COMMENT=128;
		public const ulong SHOW_DOCUMENT=256;
		public const ulong SHOW_DOCUMENT_FRAGMENT=1024;
		public const ulong SHOW_DOCUMENT_TYPE=512;
		public const ulong SHOW_ELEMENT=1;
		public const ulong SHOW_ENTITY=32;
		public const ulong SHOW_ENTITY_REFERENCE=16;
		public const ulong SHOW_NOTATION=2048;
		public const ulong SHOW_PROCESSING_INSTRUCTION=64;
		public const ulong SHOW_TEXT=4;
		
		
		/// <summary>The acceptNode method.</summary>
		public NodeFilterDelegate acceptNode;
		
	}
	
}