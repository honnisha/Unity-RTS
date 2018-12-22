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
	/// A CSS unit. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally.
	/// </summary>
	
	[Values.Preserve]
	public class CssUnit:Value{
		
		/// <summary>The set of text strings that indicate this unit is about to appear in the CSS stream. #, " etc.</summary>
		public virtual string[] PreText{
			get{
				return null;
			}
		}
		
		/// <summary>The set of text strings that indicate this unit has just appeared in the CSS stream. px,%,cm etc.</summary>
		public virtual string[] PostText{
			get{
				return null;
			}
		}
		
	}
	
}