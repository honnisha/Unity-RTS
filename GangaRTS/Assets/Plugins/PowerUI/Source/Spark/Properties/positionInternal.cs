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
	/// Represents the -spark-position-internal: css property. It holds the values of top, right, bottom and left in a single box.
	/// </summary>
	
	public class PositionInternal:CssProperty{
		
		public static PositionInternal GlobalProperty;
		public static CssProperty Top;
		public static CssProperty Right;
		public static CssProperty Bottom;
		public static CssProperty Left;
		
		
		public PositionInternal(){
			GlobalProperty=this;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-position-internal"};
		}
		
		/// <summary>True if this property is for internal use only.</summary>
		public override bool Internal{
			get{
				return true;
			}
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override void Aliases(){
			Alias("top",ValueAxis.Y,0);
			Alias("right",ValueAxis.X,1);
			Alias("bottom",ValueAxis.Y,2);
			Alias("left",ValueAxis.X,3);
			
			// Quick references:
			Top=GetAliased(0);
			Right=GetAliased(1);
			Bottom=GetAliased(2);
			Left=GetAliased(3);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>Computes the box for the given context now.</summary>
		public BoxStyle Compute(Css.Value value,RenderableData context,int position){
			
			// Result:
			BoxStyle result=new BoxStyle();
			
			if(value==null || position==PositionMode.Static){
				// Ignore position for static elements (or if it's not declared).
				result.Top=float.MaxValue;
				result.Bottom=float.MaxValue;
				result.Right=float.MaxValue;
				result.Left=float.MaxValue;
				return result;
			}
			
			// Top and bottom:
			Css.Value a=value[0];
			Css.Value b=value[2];
			
			// Note that the default here is auto:
			bool aAuto=a.IsAuto;
			bool bAuto=b.IsAuto;
			
			if(aAuto || bAuto){
				
				if(!aAuto){
					
					// Bottom is auto, top is a number.
					result.Top=a.GetDecimal(context,Top);
					result.Bottom=float.MaxValue;
					
				}else if(!bAuto){
					
					// Top is auto, bottom is a number.
					result.Bottom=b.GetDecimal(context,Bottom);
					result.Top=float.MaxValue;
					
				}else{
				
					// Otherwise both auto => both undefined (0).
					result.Top=float.MaxValue;
					result.Bottom=float.MaxValue;
					
				}
				
			}else{
				
				// Both have been declared. A special case for height is triggered here (absolute only).
				result.Top=a.GetDecimal(context,Top);
				
				// It'll only work for absolutes though:
				if(position==PositionMode.Absolute){
					result.Bottom=b.GetDecimal(context,Bottom);
				}else{
					// Ignore bottom.
					result.Bottom=float.MaxValue;
				}
				
			}
			
			// Right and left:
			a=value[1];
			b=value[3];
			
			// Note that the default here is auto:
			aAuto=a.IsAuto;
			bAuto=b.IsAuto;
			
			if(aAuto || bAuto){
				
				if(!aAuto){
					
					// Left is auto, right is a number.
					result.Right=a.GetDecimal(context,Right);
					result.Left=float.MaxValue;
					
				}else if(!bAuto){
					
					// Right is auto, left is a number.
					result.Left=b.GetDecimal(context,Left);
					result.Right=float.MaxValue;
					
				}else{
					
					// Otherwise both auto => both undefined (0).
					result.Right=float.MaxValue;
					result.Left=float.MaxValue;
					
				}
				
			}else{
				
				// Both have been declared. A special case for width is triggered here (absolute only).
				result.Left=b.GetDecimal(context,Left);
				
				// It'll only work for absolutes though:
				if(position==PositionMode.Absolute){
					result.Right=a.GetDecimal(context,Right);
				}else{
					// Ignore left.
					result.Right=float.MaxValue;
				}
				
			}
			
			return result;
			
		}
		
	}
	
}



