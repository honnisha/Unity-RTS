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
	/// Represents what a property is relative to.
	/// E.g. padding:10% is relative to the parents dimensions (Dimensions).
	/// However, font-size:100% is relative to the parents font-size (Self).
	/// </summary>
	
	public enum ValueRelativity{
		None,
		Dimensions,
		Self,
		FontSize,
		SelfDimensions,
		LineHeight,
		Viewport // The window (or viewBox for SVG)
	}
	
	/// <summary>
	/// The axis that a CSS property is relative to, if any. E.g. width is relative to x. Radius is none.
	/// </summary>
	public enum ValueAxis{
		
		None,
		X,
		Y,
		Z
		
	}
	
}