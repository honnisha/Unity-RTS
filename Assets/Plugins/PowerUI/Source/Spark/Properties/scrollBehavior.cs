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
using Dom;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// The scroll-behavior CSS property. auto | smooth
	/// </summary>
	
	public class ScrollBehavior:CssProperty{
		
		public static ScrollBehavior GlobalProperty;
		
		
		public ScrollBehavior(){
			GlobalProperty=this;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"scroll-behavior"};
		}
		
	}
	
}