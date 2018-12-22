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
	/// Represents the overflow: css property.
	/// </summary>
	
	public class Overflow:CssProperty{
		
		public static Overflow GlobalProperty;
		
		/// <summary>Gets the overflow modes on x/y of the given value.</summary>
		public static void GetOverflow(Value value,out int x,out int y){
			
			if(value==null){
				x=VisibilityMode.Visible;
				y=VisibilityMode.Visible;
			}else{
				
				// Special case for auto:
				if(value[0].IsAuto){
					x=VisibilityMode.Auto;
				}else{
					x=value[0].GetInteger(null,null);
				}
				
				// Same for y:
				if(value[1].IsAuto){
					y=VisibilityMode.Auto;
				}else{
					y=value[1].GetInteger(null,null);
				}
				
			}
			
		}
		
		public Overflow(){
			GlobalProperty=this;
			InitialValueText="visible";
		}
		
		public override string[] GetProperties(){
			return new string[]{"overflow"};
		}
		
		public override void Aliases(){
			// overflow-x:
			PointAliases2D();
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Resolve overflow:
			int overflowX;
			int overflowY;
			Css.Value overflowValue=style.Resolve(Css.Properties.Overflow.GlobalProperty);
			GetOverflow(overflowValue,out overflowX,out overflowY);
			
			// Add/ remove scrollbars:
			style.ResetScrollbars(overflowX,overflowY);
			
			// Request a layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}