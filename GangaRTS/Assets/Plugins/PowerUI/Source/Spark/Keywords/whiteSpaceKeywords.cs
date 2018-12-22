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
	/// Represents an instance of the pre keyword.
	/// </summary>
	
	public class Pre:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return WhiteSpaceMode.Pre;
		}
		
		public override string Name{
			get{
				return "pre";
			}
		}
		
	}
	
	public class PreLine:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return WhiteSpaceMode.PreLine;
		}
		
		public override string Name{
			get{
				return "pre-line";
			}
		}
		
	}
	
	public class PreWrap:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return WhiteSpaceMode.PreWrap;
		}
		
		public override string Name{
			get{
				return "pre-wrap";
			}
		}
		
	}
	
	public class NoWrap:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return WhiteSpaceMode.NoWrap;
		}
		
		public override string Name{
			get{
				return "nowrap";
			}
		}
		
	}
	
}