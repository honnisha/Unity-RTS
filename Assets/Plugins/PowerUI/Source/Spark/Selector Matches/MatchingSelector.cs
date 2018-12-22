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

namespace Css{
	
	/// <summary>
	/// When a selector structurally matches an element, one of these is created
	/// so we can track when the selector activates/ deactivates.
	/// </summary>
	
	public class MatchingSelector{
		
		/// <summary>True if this selector is active.</summary>
		public bool Active;
		/// <summary>The number of roots that are active.
		/// When this matches the number of roots, the selector as a whole activates.</summary>
		public int ActiveRoots;
		/// <summary>The underlying selector.</summary>
		public Selector Selector;
		/// <summary>The targeted RenderData.</summary>
		public RenderableData Target;
		/// <summary>All the matching roots.</summary>
		public MatchingRoot[] MatchedRoots;
		
		
		/// <summary>Removes this selector from all the computed styles it affects, except for the target.
		/// Use ComputedStyle.RemoveMatch(x) instead of calling this directly.</summary>
		internal void Remove(){
			
			for(int i=0;i<MatchedRoots.Length;i++){
				
				// Remove it (if it's the target and it was active, it'll also remove the style for us too):
				MatchingRoot root=MatchedRoots[i];
				
				// Remove from matches:
				ComputedStyle cs=(root.Node as IRenderableNode).ComputedStyle;
				
				// Remove the node:
				if(root.NextInStyle==null){
					cs.LastMatch=root.PreviousInStyle;
				}else{
					root.NextInStyle.PreviousInStyle=root.PreviousInStyle;
				}
				
				if(root.PreviousInStyle==null){
					cs.FirstMatch=root.NextInStyle;
				}else{
					root.PreviousInStyle.NextInStyle=root.NextInStyle;
				}
				
				if(root.IsTarget && Active){
					// Remove props:
					cs.MatchChanged(Style,false);
				}
				
			}
			
		}
		
		/// <summary>Resets the active state of all roots.</summary>
		public void ResetActive(){
			
			for(int i=0;i<MatchedRoots.Length;i++){
				
				MatchedRoots[i].ResetActive();
				
			}
			
		}
		
		/// <summary>The style being applied.</summary>
		public Css.Style Style{
			get{
				return Selector.Rule.Style;
			}
		}
		
		/// <summary>The number of roots the selector has.</summary>
		public int RootCount{
			get{
				return Selector.RootCount;
			}
		}
		
	}
	
}