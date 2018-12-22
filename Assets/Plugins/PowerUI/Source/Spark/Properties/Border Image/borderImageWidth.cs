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
	/// Represents the border-image-width: css property.
	/// </summary>
	
	public class BorderImageWidth:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-image-width"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[<length> | <number> | <percentage> | auto]{1,4}
			*/
			
			return new Spec.Repeated(
				new Spec.OneOf(
					new Spec.ValueType(typeof(Css.Units.DecimalUnit)),
					new Spec.ValueType(typeof(Css.Units.PercentUnit)),
					new Spec.Literal("auto")
				)
			,1,4);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



