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
	/// Represents the padding: css property.
	/// </summary>
	
	public class Padding:CssProperty{
		
		public static Padding GlobalProperty;
		public static CssProperty Top;
		public static CssProperty Right;
		public static CssProperty Bottom;
		public static CssProperty Left;
		
		
		public Padding(){
			GlobalProperty=this;
			InitialValue=ZERO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"padding"};
		}
		
		public override void Aliases(){
			// E.g. padding-top:
			SquareAliases();
			
			// Quick references:
			Top=GetAliased(0);
			Right=GetAliased(1);
			Bottom=GetAliased(2);
			Left=GetAliased(3);
			
			// text-indent maps to padding-left, aka index 3 of padding:
			Alias("text-indent",ValueAxis.X,3);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>A bitmask for all the display types that padding isn't allowed on.</summary>
		private const int NoPaddingAllowed=
			(int)DisplayMode.TableRowGroup | (int)DisplayMode.TableHeaderGroup | 
			(int)DisplayMode.TableFooterGroup | (int)DisplayMode.TableRow | 
			(int)DisplayMode.TableColumnGroup | (int)DisplayMode.TableColumn;
		
		/// <summary>Computes the box for the given element now.</summary>
		public BoxStyle Compute(Css.Value value,RenderableData context,int display){
			
			// Result:
			BoxStyle result=new BoxStyle();
			
			if(value==null){
				return result;
			}
			
			// Using a display mode that doesn't allow padding?
			if((display & NoPaddingAllowed)!=0){
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



