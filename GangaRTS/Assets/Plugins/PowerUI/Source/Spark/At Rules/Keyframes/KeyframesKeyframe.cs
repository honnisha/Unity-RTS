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
using Css;
using Css.Units;
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	/// <summary>
	/// A particular frame in an @keyframes animation.
	/// </summary>
	
	public class KeyframesKeyframe{
		
		/// <summary>The underlying style block.</summary>
		public Style Style;
		/// <summary>The time, in "percent", that this frame occurs at.</summary>
		public float Time;
		/// <summary>The time, in "percent" between this and the frame after it.</summary>
		public float AfterDelay;
		/// <summary>The time, in "percent" between this and the frame before it.</summary>
		public float BeforeDelay;
		
		
		public KeyframesKeyframe(Style style){
			
			// Apply the style:
			Style=style;
			
		}
		
	}
	
}