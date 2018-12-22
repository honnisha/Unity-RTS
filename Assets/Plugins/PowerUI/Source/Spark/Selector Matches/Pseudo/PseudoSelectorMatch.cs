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
using Css.Units;
using Dom;


namespace Css{
	
	/// <summary>
	/// Handles all pseudo elements like ::after.
	/// <summary>
	
	public class PseudoSelectorMatch : SelectorMatcher{
		
		public virtual void Select(CssEvent e){}
		
		/// <summary>Doesn't create the virtual if it doesn't exist.</summary>
		internal void GetVirtual(CssEvent e,int priority){
			
			// Get the CS:
			ComputedStyle cs=e.SelectorTarget.computedStyle;
			
			VirtualElements virts=cs.RenderData.Virtuals;
			
			IRenderableNode node=null;
			
			if(virts!=null){
				// Get and apply:
				node=virts.Get(priority) as IRenderableNode;
			}
			
			// Update the target:
			e.SelectorTarget=(node==null) ? null : node.RenderData;
			
		}
		
		/// <summary>Gets or creates the virtual.</summary>
		internal void CreateVirtual(CssEvent e,int priority){
			
			// Get the CS:
			ComputedStyle cs=e.SelectorTarget.computedStyle;
			
			// Create and apply:
			Node node=cs.GetOrCreateVirtual(priority,"span",true);
			e.SelectorTarget=(node as IRenderableNode).RenderData;
			
		}
		
	}
	
}