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


namespace Css{
	
	public partial class CssEvent : Dom.Event{
		
		/// <summary>The node we're currently at.</summary>
		public Node CurrentNode;
		/// <summary>The target of the selector.</summary>
		public RenderableData SelectorTarget;
		
	}
	
}