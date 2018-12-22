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
	
	public class Dashed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return BorderStyle.Dashed;
		}
		
		public override string Name{
			get{
				return "dashed";
			}
		}
		
	}
	
	public class Solid:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return BorderStyle.Solid;
		}
		
		public override string Name{
			get{
				return "solid";
			}
		}
		
	}
	
}