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
using PowerUI;
using Dom;


namespace Css{
	
	/// <summary>
	/// Describes if an element currently is moused over.
	/// <summary>

	sealed class IsHover:CssKeyword{
		
		public override string Name{
			get{
				return "hover";
			}
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			return new HoverMatcher();
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for :hover.
	/// </summary>
	
	sealed class HoverMatcher:LocalMatcher{
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			// The element must be one of the active pointed ones or one of their parents:
			for(int i=0;i<InputPointer.PointerCount;i++){
				
				InputPointer pointer=InputPointer.AllRaw[i];
				
				if(pointer.ActiveOverTarget==node){
					// Great, got it!
					return true;
				}else if(pointer.ActiveOverTarget!=null){
					
					// Is our node one of its parents?
					if(node.isParentOf(pointer.ActiveOverTarget)){
						return true;
					}
					
				}
				
			}
			
			// Nope!
			return false;
			
		}
		
	}
	
}