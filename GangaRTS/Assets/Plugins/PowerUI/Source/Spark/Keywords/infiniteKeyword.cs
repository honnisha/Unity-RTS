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


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the infinite keyword.
	/// </summary>
	
	public class InfiniteKeyword:CssKeyword{
		
		public override string Name{
			get{
				return "infinite";
			}
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return int.MaxValue;
		}
		
	}
	
}