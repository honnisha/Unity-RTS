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


namespace Css{
	
	/// <summary>
	/// Describes if a sibling directly before the element matches a selector.
	/// <summary>

	public class DirectPreviousSiblingMatch:StructureMatcher{
		
		public override void MoveUpwards(CssEvent e){
			
			// Current node:
			Node node=e.CurrentNode;
			
			if(node!=null){
				e.CurrentNode=node.previousElementSibling;
			}
			
		}
		
		public override string ToString(){
			return "+";
		}
		
	}
	
}