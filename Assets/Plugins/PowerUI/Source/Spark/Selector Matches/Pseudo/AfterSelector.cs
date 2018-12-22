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


namespace Css{
	
	/// <summary>
	/// Handles the after pseudo selector.
	/// <summary>

	sealed class After:CssKeyword{
		
		public override string Name{
			get{
				return "after";
			}
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			return new AfterSelector();
		}
		
	}
	
	/// <summary>
	/// Describes the after pseudo-selector.
	/// <summary>
	
	public class AfterSelector:PseudoSelectorMatch{
		
		public const int Priority=VirtualElements.AFTER_ZONE;
		
		public override void Select(CssEvent e){
			
			// Get/ create the v-child:
			CreateVirtual(e,Priority);
			
		}
		
	}
	
}