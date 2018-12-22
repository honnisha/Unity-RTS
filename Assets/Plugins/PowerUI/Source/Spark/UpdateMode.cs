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

namespace Css{
	
	/// <summary>
	/// Defines how an element is to be updated.
	/// </summary>
	
	public enum UpdateMode:int{
		
		/// <summary>No update scheduled.</summary>
		None=0,
		/// <summary>A paint event (the fastest).</summary>
		Paint=1,
		/// <summary>Paints an element and all its kids.</summary>
		PaintAll=2,
		/// <summary>A shortform reflow of this node and all its kids.
		/// These ones don't update CSS and are primarily used by scrolling.</summary>
		FastReflow=3,
		/// <summary>Just does a render call.</summary>
		Render=4,
		/// <summary>A full reflow of this node and all its kids.</summary>
		Reflow=5
		
	}
	
}