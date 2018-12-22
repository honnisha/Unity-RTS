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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the rect() css function.
	/// </summary>
	
	public partial class RectFunction:CssFunction{
		
		
		public RectFunction(){
			
			Name="rect";
			Type=ValueType.Set;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"rect"};
		}
		
		protected override Css.Value Clone(){
			RectFunction result=new RectFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



