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
	/// Represents an instance of the capitalize keyword.
	/// </summary>
	
	public class Capitalize:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return TextTransformMode.Capitalize;
		}
		
		public override string Name{
			get{
				return "capitalize";
			}
		}
		
	}
	
	public class Uppercase:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return TextTransformMode.Uppercase;
		}
		
		public override string Name{
			get{
				return "uppercase";
			}
		}
		
	}
	
	public class Lowercase:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return TextTransformMode.Lowercase;
		}
		
		public override string Name{
			get{
				return "lowercase";
			}
		}
		
	}
	
}