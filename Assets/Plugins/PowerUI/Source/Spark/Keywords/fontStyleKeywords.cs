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
using InfiniText;


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the italic keyword.
	/// </summary>
	
	public class Italic:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontFaceFlags.Italic;
		}
		
		public override string Name{
			get{
				return "italic";
			}
		}
		
	}
	
	public class Oblique:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontFaceFlags.Oblique;
		}
		
		public override string Name{
			get{
				return "oblique";
			}
		}
		
	}
	
}