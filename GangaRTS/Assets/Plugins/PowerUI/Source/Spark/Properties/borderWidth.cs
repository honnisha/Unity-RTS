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
	/// Represents the border-width: css property.
	/// </summary>
	
	public class BorderWidth:CssProperty{
		
		public static BorderWidth GlobalProperty;
		public static CssProperty Top;
		public static CssProperty Right;
		public static CssProperty Bottom;
		public static CssProperty Left;
		
		
		public BorderWidth(){
			GlobalProperty=this;
			InitialValueText="medium";
		}
		
		public override string[] GetProperties(){
			return new string[]{"border-width"};
		}
		
		public override void Aliases(){
			Alias("border-top-width",ValueAxis.Y,0);
			Alias("border-right-width",ValueAxis.X,1);
			Alias("border-bottom-width",ValueAxis.Y,2);
			Alias("border-left-width",ValueAxis.X,3);
			
			// Quick references:
			Top=GetAliased(0);
			Right=GetAliased(1);
			Bottom=GetAliased(2);
			Left=GetAliased(3);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			// Does the border have any corners? If so, we need to update them:
			if(border.Corners!=null){
				border.Corners.ClearCorners();
			}
			
			// Request a layout:
			border.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>Computes the box for the given element now.</summary>
		public BoxStyle Compute(Css.Value value,Css.Value style,RenderableData context){
			
			// Result:
			BoxStyle result=new BoxStyle();
			
			if(value==null || style==null){
				return result;
			}
			
			// Top:
			result.Top=value[0].GetDecimal(context,Top);
			
			// Right:
			result.Right=value[1].GetDecimal(context,Right);
			
			// Bottom:
			result.Bottom=value[2].GetDecimal(context,Bottom);
			
			// Left:
			result.Left=value[3].GetDecimal(context,Left);
			
			// Account for style, dropping widths to 0 when 'none':
			for(int i=0;i<4;i++){
				
				// If null or 'none'..
				if(style[i].IsType(typeof(Css.Keywords.None))){
					
					// Clear it:
					result[i]=0f;
					
				}
				
			}
			
			// Negative values aren't allowed - enforce that now:
			if(result.Top<0f){
				result.Top=0f;
			}
			
			if(result.Right<0f){
				result.Right=0f;
			}
			
			if(result.Bottom<0f){
				result.Bottom=0f;
			}
			
			if(result.Left<0f){
				result.Left=0f;
			}
			
			return result;
			
		}
		
	}
	
}



