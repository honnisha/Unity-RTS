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
	/// Represents an instance of the xx-small keyword.
	/// </summary>
	
	public class XXSmall:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 9f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "xx-small";
			}
		}
		
	}
	
	public class XSmall:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 10f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "x-small";
			}
		}
		
	}
	
	public class Small:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 13f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "small";
			}
		}
		
	}
	
	public class Large:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 18f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "large";
			}
		}
		
	}
	
	public class XLarge:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 24f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "x-large";
			}
		}
		
	}
	
	public class XXLarge:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 32f * context.ValueScale;
		}
		
		public override string Name{
			get{
				return "xx-large";
			}
		}
		
	}
	
	public class Smaller:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return context.ResolveParentDecimal(property) * 0.66f;
		}
		
		public override string Name{
			get{
				return "smaller";
			}
		}
		
	}
	
	public class Larger:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return context.ResolveParentDecimal(property) * 1.33f;
		}
		
		public override string Name{
			get{
				return "larger";
			}
		}
		
	}
	
}