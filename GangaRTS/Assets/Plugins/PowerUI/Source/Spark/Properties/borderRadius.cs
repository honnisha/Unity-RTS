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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the border-radius: css property.
	/// </summary>
	
	public class BorderRadius:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-radius"};
		}
		
		public override void Aliases(){
			// Map the border aliases:
			Alias("border-top-left-radius",ValueAxis.None,0);
			Alias("border-top-right-radius",ValueAxis.None,1);
			Alias("border-bottom-right-radius",ValueAxis.None,2);
			Alias("border-bottom-left-radius",ValueAxis.None,3);
			
			// Backwards compat aliases:
			Alias("border-top-radius",ValueAxis.None,0);
			Alias("border-right-radius",ValueAxis.None,1);
			Alias("border-bottom-radius",ValueAxis.None,2);
			Alias("border-left-radius",ValueAxis.None,3);
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			if(value==null){
				// No corners:
				border.Corners=null;
			}else{
				
				// Apply top left:
				border.SetCorner(RoundCornerPosition.TopLeft,value[0].GetDecimal(style.RenderData,GetAliased(0)));
				
				// Apply top right:
				border.SetCorner(RoundCornerPosition.TopRight,value[1].GetDecimal(style.RenderData,GetAliased(1)));
				
				// Apply bottom right:
				border.SetCorner(RoundCornerPosition.BottomLeft,value[2].GetDecimal(style.RenderData,GetAliased(2)));
				
				// Apply bottom left:
				border.SetCorner(RoundCornerPosition.BottomRight,value[3].GetDecimal(style.RenderData,GetAliased(3)));
				
			}
			
			// Request a layout:
			border.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



