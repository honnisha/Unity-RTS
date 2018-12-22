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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace Css{
	
	/// <summary>
	/// A sorted list of virtual elements. Stored in an elements computed style.
	/// </summary>
	public class VirtualElements{
		
		/// <summary>Priority offset for elements in the "scrollbar" (beyond after) zone.</summary>
		public const int SCROLLBAR_ZONE=AFTER_ZONE+200;
		/// <summary>Priority offset for elements in the "after" zone.</summary>
		public const int AFTER_ZONE=1000;
		/// <summary>Priority offset for elements in the "during" zone.</summary>
		public const int DURING_ZONE=0;
		/// <summary>Priority offset for elements in the "before" zone.</summary>
		public const int BEFORE_ZONE=-1000;
		
		/// <summary>True if this element should its own childnodes.
		/// Essentially childNodes act like they aren't there at all during 
		/// rendering and the element is handled by virtuals only otherwise.</summary>
		public bool AllowDrawKids=true;
		
		/// <summary>The virtual elements.</summary>
		public SortedDictionary<int,Node> Elements=new SortedDictionary<int,Node>();
		
		/// <summary>Gets by priority (unique for each virtual element type). Null if it doesn't exist.</summary>
		public Node Get(int priority){
			Node result;
			Elements.TryGetValue(priority,out result);
			return result;
		}
		
		/// <summary>True if a virtual element of the given priority is in the set
		/// (priority is unique per type of element, e.g. :after is always x).</summary>
		public bool Has(int priority){
			return Elements.ContainsKey(priority);
		}
		
		public void push(int priority,Node ve){
			Elements[priority]=ve;
		}
		
		/// <summary>Called when all virtual elements are being removed.</summary>
		internal void RemovedAll(){
			
			// Run RemovedFromDOM on all:
			foreach(KeyValuePair<int,Node> kvp in Elements){
				
				kvp.Value.RemovedFromDOM();
				
			}
			
		}
		
		/// <summary>Safely removes a virtual element with the given priority.</summary>
		public bool remove(int priority){
			
			// Try getting it:
			Node ve;
			if(Elements.TryGetValue(priority,out ve)){
				
				// Remove:
				Elements.Remove(priority);
				
				// Removed from the DOM:
				ve.RemovedFromDOM();
				
				return true;
				
			}
			
			return false;
			
		}
		
		public VirtualElements(){}
		
	}
	
}