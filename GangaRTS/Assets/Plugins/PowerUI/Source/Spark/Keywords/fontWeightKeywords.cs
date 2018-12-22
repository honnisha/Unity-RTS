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
	/// Represents an instance of the bold keyword.
	/// </summary>
	
	public class Bold:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			return 700;
			
		}
		
		public override string Name{
			get{
				return "bold";
			}
		}
		
	}
	
	public class Bolder:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			// Get the parent:
			float parent=context.ResolveParentDecimal(Css.Properties.FontWeight.GlobalProperty);
			
			// (see https://developer.mozilla.org/en-US/docs/Web/CSS/font-weight):
			
			if(parent<400f){
				return 400f;
			}else if(parent<600f){
				return 700f;
			}
			
			return 900f;
			
		}
		
		public override string Name{
			get{
				return "bolder";
			}
		}
		
	}
	
	public class Lighter:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			// Get the parent:
			float parent=context.ResolveParentDecimal(Css.Properties.FontWeight.GlobalProperty);
			
			// (see https://developer.mozilla.org/en-US/docs/Web/CSS/font-weight):
			
			if(parent<600f){
				return 100f;
			}else if(parent<800f){
				return 400f;
			}
			
			return 700f;
			
		}
		
		public override string Name{
			get{
				return "lighter";
			}
		}
		
	}
	
}