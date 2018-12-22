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
	/// Represents an instance of the ultra-condensed keyword.
	/// </summary>
	
	public class UltraCondensed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.UltraCondensed;
		}
		
		public override string Name{
			get{
				return "ultra-condensed";
			}
		}
		
	}
	
	public class ExtraCondensed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.ExtraCondensed;
		}
		
		public override string Name{
			get{
				return "extra-condensed";
			}
		}
		
	}
	
	public class Condensed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.Condensed;
		}
		
		public override string Name{
			get{
				return "condensed";
			}
		}
		
	}
	
	public class SemiCondensed:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.SemiCondensed;
		}
		
		public override string Name{
			get{
				return "semi-condensed";
			}
		}
		
	}
	
	public class SemiExpanded:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.SemiExpanded;
		}
		
		public override string Name{
			get{
				return "semi-expanded";
			}
		}
		
	}
	
	public class Expanded:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.Expanded;
		}
		
		public override string Name{
			get{
				return "expanded";
			}
		}
		
	}
	
	public class ExtraExpanded:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.ExtraExpanded;
		}
		
		public override string Name{
			get{
				return "extra-expanded";
			}
		}
		
	}
	
	public class UltraExpanded:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return FontStretchMode.UltraExpanded;
		}
		
		public override string Name{
			get{
				return "ultra-expanded";
			}
		}
		
	}
	
}