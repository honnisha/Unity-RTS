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
using Dom;


namespace Dom{
	
	/// <summary>
	/// A document fragment.
	/// </summary>

	
	public partial class DocumentFragment:Node{
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			
			DocumentFragment t=other as DocumentFragment;
			
			return t!=null && t==this;
		}
		
		public override string ToString(){
			return "[Object DocumentFragment]";
		}
		
		/// <summary>The nodes full name, accounting for namespace.</summary>
		public override string nodeName{
			get{
				return "#documentFragment";
			}
		}
		
		/// <summary>The node type.</summary>
		public override ushort nodeType{
			get{
				return 11;
			}
		}
		
		/// <summary>The value.</summary>
		public override string nodeValue{
			get{
				return null;
			}
			set{}
		}
		
		/// <summary>The value.</summary>
		public override string textContent{
			get{
				return null;
			}
			set{}
		}
		
	}
	
}
	