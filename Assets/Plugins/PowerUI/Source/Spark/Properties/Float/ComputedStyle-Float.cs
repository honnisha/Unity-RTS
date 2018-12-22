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
	
	public partial class ComputedStyle{
		
		/// <summary>The CSS float mode. Left, right, none etc.</summary>
		public int FloatMode{
			get{
				
				// Get the float value:
				Css.Value value=this[Css.Properties.Float.GlobalProperty];
				
				if(value==null){
					
					// Assume none if blank:
					return Css.FloatMode.None;
					
				}
				
				// Get as an integer:
				return value.GetInteger(RenderData,Css.Properties.Float.GlobalProperty);
				
			}
		}
		
	}
	
}