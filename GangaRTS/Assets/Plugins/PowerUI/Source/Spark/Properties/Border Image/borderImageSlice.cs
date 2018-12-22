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
	/// Represents the border-image-slice: css property.
	/// </summary>
	
	public class BorderImageSlice:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-image-slice"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[<number> | <percentage>]{1,4} && fill?
			*/
			
			return new Spec.AllAnyOrder(
				new Spec.Repeated(
					new Spec.OneOf(
						new Spec.ValueType(typeof(Css.Units.DecimalUnit)),
						new Spec.ValueType(typeof(Css.Units.PercentUnit))
					)
				,1,4),
				new Spec.Optional(new Spec.Literal("fill"))
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// set background image if "fill" is presented
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			// Request a layout:
			if(border!=null){
				border.RequestLayout();
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



